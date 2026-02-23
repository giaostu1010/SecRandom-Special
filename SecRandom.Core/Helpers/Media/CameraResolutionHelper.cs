using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace SecRandom.Core.Helpers.Media;

public static class CameraResolutionHelper
{
    public readonly record struct CameraResolution(int Width, int Height)
    {
        public override string ToString() => $"{Width}x{Height}";
    }

    public static IReadOnlyList<string> GetCameraNames()
    {
        if (!OperatingSystem.IsWindows())
        {
            return [];
        }

        var devEnumType = Type.GetTypeFromCLSID(Clsid.SystemDeviceEnum);
        if (devEnumType is null)
        {
            return [];
        }

        var createDevEnum = (ICreateDevEnum?)Activator.CreateInstance(devEnumType);
        if (createDevEnum is null)
        {
            return [];
        }

        IEnumMoniker? enumMoniker = null;
        try
        {
            var category = FilterCategory.VideoInputDevice;
            var hr = createDevEnum.CreateClassEnumerator(in category, out enumMoniker, 0);
            if (hr != 0 || enumMoniker is null)
            {
                return [];
            }

            var result = new List<string>();
            var current = new IMoniker[1];
            var index = 0;
            while (enumMoniker.Next(1, current, IntPtr.Zero) == 0)
            {
                var moniker = current[0];
                try
                {
                    var name = TryGetMonikerFriendlyName(moniker);
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        result.Add($"{index}");
                    }
                    else
                    {
                        result.Add($"{index}: {name}");
                    }
                }
                finally
                {
                    Marshal.ReleaseComObject(moniker);
                }

                index++;
                current = new IMoniker[1];
            }

            return result;
        }
        catch
        {
            return [];
        }
        finally
        {
            if (enumMoniker is not null)
            {
                Marshal.ReleaseComObject(enumMoniker);
            }
            Marshal.ReleaseComObject(createDevEnum);
        }
    }

    public static IReadOnlyList<CameraResolution> GetResolutionsByIndex(int deviceIndex)
    {
        if (!OperatingSystem.IsWindows())
        {
            return [];
        }

        if (deviceIndex < 0)
        {
            return [];
        }

        try
        {
            return EnumerateDirectShowResolutions(deviceIndex);
        }
        catch
        {
            return [];
        }
    }

    private static IReadOnlyList<CameraResolution> EnumerateDirectShowResolutions(int deviceIndex)
    {
        var devEnumType = Type.GetTypeFromCLSID(Clsid.SystemDeviceEnum);
        if (devEnumType is null)
        {
            return [];
        }

        var createDevEnum = (ICreateDevEnum?)Activator.CreateInstance(devEnumType);
        if (createDevEnum is null)
        {
            return [];
        }

        IEnumMoniker? enumMoniker = null;
        IBaseFilter? filter = null;
        IEnumPins? enumPins = null;
        IPin? pin = null;
        try
        {
            var category = FilterCategory.VideoInputDevice;
            var hr = createDevEnum.CreateClassEnumerator(in category, out enumMoniker, 0);
            if (hr != 0 || enumMoniker is null)
            {
                return [];
            }

            var targetMoniker = GetMonikerByIndex(enumMoniker, deviceIndex);
            if (targetMoniker is null)
            {
                return [];
            }

            try
            {
                var filterGuid = typeof(IBaseFilter).GUID;
                targetMoniker.BindToObject(null, null, in filterGuid, out var filterObj);
                filter = filterObj as IBaseFilter;
                if (filter is null)
                {
                    return [];
                }

                filter.EnumPins(out enumPins);
                if (enumPins is null)
                {
                    return [];
                }

                var resolutions = new HashSet<CameraResolution>();
                var pins = new IPin[1];
                while (enumPins.Next(1, pins, IntPtr.Zero) == 0)
                {
                    pin = pins[0];
                    try
                    {
                        pin.QueryDirection(out var direction);
                        if (direction != PinDirection.Output)
                        {
                            continue;
                        }

                        if (!TryGetStreamConfig(pin, out var streamConfig) || streamConfig is null)
                        {
                            continue;
                        }

                        try
                        {
                            streamConfig.GetNumberOfCapabilities(out var count, out var size);
                            if (count <= 0 || size <= 0)
                            {
                                continue;
                            }

                            for (var i = 0; i < count; i++)
                            {
                                var capsPtr = Marshal.AllocCoTaskMem(size);
                                try
                                {
                                    var getHr = streamConfig.GetStreamCaps(i, out var mediaType, capsPtr);
                                    if (getHr != 0)
                                    {
                                        continue;
                                    }

                                    try
                                    {
                                        if (TryParseResolution(mediaType, out var resolution))
                                        {
                                            if (resolution.Width > 0 && resolution.Height > 0)
                                            {
                                                resolutions.Add(resolution);
                                            }
                                        }
                                    }
                                    finally
                                    {
                                        FreeMediaType(ref mediaType);
                                    }
                                }
                                finally
                                {
                                    Marshal.FreeCoTaskMem(capsPtr);
                                }
                            }
                        }
                        finally
                        {
                            Marshal.ReleaseComObject(streamConfig);
                        }
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(pin);
                        pin = null;
                    }
                }

                return resolutions
                    .OrderByDescending(r => r.Width * r.Height)
                    .ThenByDescending(r => r.Width)
                    .ThenByDescending(r => r.Height)
                    .ToList();
            }
            finally
            {
                Marshal.ReleaseComObject(targetMoniker);
            }
        }
        finally
        {
            if (pin is not null)
            {
                Marshal.ReleaseComObject(pin);
            }
            if (enumPins is not null)
            {
                Marshal.ReleaseComObject(enumPins);
            }
            if (filter is not null)
            {
                Marshal.ReleaseComObject(filter);
            }
            if (enumMoniker is not null)
            {
                Marshal.ReleaseComObject(enumMoniker);
            }
            Marshal.ReleaseComObject(createDevEnum);
        }
    }

    private static IMoniker? GetMonikerByIndex(IEnumMoniker enumMoniker, int index)
    {
        enumMoniker.Reset();
        var current = new IMoniker[1];
        var fetched = IntPtr.Zero;
        var i = 0;
        while (enumMoniker.Next(1, current, fetched) == 0)
        {
            if (i == index)
            {
                return current[0];
            }

            Marshal.ReleaseComObject(current[0]);
            current = new IMoniker[1];
            i++;
        }

        return null;
    }

    private static bool TryGetStreamConfig(IPin pin, out IAMStreamConfig? streamConfig)
    {
        streamConfig = null;
        var iid = typeof(IAMStreamConfig).GUID;
        var unk = Marshal.GetIUnknownForObject(pin);
        int hr;
        IntPtr ptr;
        try
        {
            hr = Marshal.QueryInterface(unk, ref iid, out ptr);
        }
        finally
        {
            Marshal.Release(unk);
        }
        if (hr != 0 || ptr == IntPtr.Zero)
        {
            return false;
        }

        try
        {
            streamConfig = (IAMStreamConfig)Marshal.GetObjectForIUnknown(ptr);
            return true;
        }
        finally
        {
            Marshal.Release(ptr);
        }
    }

    private static string? TryGetMonikerFriendlyName(IMoniker moniker)
    {
        IPropertyBag? bag = null;
        try
        {
            var bagGuid = typeof(IPropertyBag).GUID;
            moniker.BindToStorage(null, null, in bagGuid, out var bagObj);
            bag = bagObj as IPropertyBag;
            if (bag is null)
            {
                if (bagObj is not null)
                {
                    Marshal.ReleaseComObject(bagObj);
                }
                return null;
            }

            var hr = bag.Read("FriendlyName", out var value, IntPtr.Zero);
            if (hr != 0)
            {
                return null;
            }

            return value as string;
        }
        catch
        {
            return null;
        }
        finally
        {
            if (bag is not null)
            {
                Marshal.ReleaseComObject(bag);
            }
        }
    }

    private static bool TryParseResolution(AmMediaType mediaType, out CameraResolution resolution)
    {
        resolution = default;
        if (mediaType.formatPtr == IntPtr.Zero)
        {
            return false;
        }

        if (mediaType.formatType == FormatType.VideoInfo)
        {
            var vih = Marshal.PtrToStructure<VideoInfoHeader>(mediaType.formatPtr);
            var width = vih.BmiHeader.Width;
            var height = Math.Abs(vih.BmiHeader.Height);
            resolution = new CameraResolution(width, height);
            return true;
        }

        if (mediaType.formatType == FormatType.VideoInfo2)
        {
            var vih2 = Marshal.PtrToStructure<VideoInfoHeader2>(mediaType.formatPtr);
            var width = vih2.BmiHeader.Width;
            var height = Math.Abs(vih2.BmiHeader.Height);
            resolution = new CameraResolution(width, height);
            return true;
        }

        return false;
    }

    private static void FreeMediaType(ref AmMediaType mediaType)
    {
        if (mediaType.formatPtr != IntPtr.Zero)
        {
            Marshal.FreeCoTaskMem(mediaType.formatPtr);
            mediaType.formatPtr = IntPtr.Zero;
        }

        if (mediaType.unkPtr != IntPtr.Zero)
        {
            Marshal.Release(mediaType.unkPtr);
            mediaType.unkPtr = IntPtr.Zero;
        }
    }

    private static class Clsid
    {
        public static readonly Guid SystemDeviceEnum = new("62BE5D10-60EB-11d0-BD3B-00A0C911CE86");
    }

    private static class FilterCategory
    {
        public static readonly Guid VideoInputDevice = new("860BB310-5D01-11d0-BD3B-00A0C911CE86");
    }

    private static class FormatType
    {
        public static readonly Guid VideoInfo = new("05589F80-C356-11CE-BF01-00AA0055595A");
        public static readonly Guid VideoInfo2 = new("F72A76A0-EB0A-11D0-ACE4-0000C0CC16BA");
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct AmMediaType
    {
        public Guid majorType;
        public Guid subType;
        [MarshalAs(UnmanagedType.Bool)] public bool fixedSizeSamples;
        [MarshalAs(UnmanagedType.Bool)] public bool temporalCompression;
        public int sampleSize;
        public Guid formatType;
        public IntPtr unkPtr;
        public int formatSize;
        public IntPtr formatPtr;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct BitmapInfoHeader
    {
        public int Size;
        public int Width;
        public int Height;
        public short Planes;
        public short BitCount;
        public int Compression;
        public int ImageSize;
        public int XPelsPerMeter;
        public int YPelsPerMeter;
        public int ClrUsed;
        public int ClrImportant;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct VideoInfoHeader
    {
        public Rect TargetRect;
        public Rect SourceRect;
        public int BitRate;
        public int BitErrorRate;
        public long AvgTimePerFrame;
        public BitmapInfoHeader BmiHeader;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct VideoInfoHeader2
    {
        public Rect TargetRect;
        public Rect SourceRect;
        public int BitRate;
        public int BitErrorRate;
        public long AvgTimePerFrame;
        public int InterlaceFlags;
        public int CopyProtectFlags;
        public int PictAspectRatioX;
        public int PictAspectRatioY;
        public int ControlFlags;
        public int Reserved2;
        public BitmapInfoHeader BmiHeader;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    private enum PinDirection
    {
        Input = 0,
        Output = 1
    }

    [ComImport]
    [Guid("29840822-5B84-11D0-BD3B-00A0C911CE86")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface ICreateDevEnum
    {
        [PreserveSig]
        int CreateClassEnumerator(in Guid pType, out IEnumMoniker? ppEnumMoniker, int dwFlags);
    }

    [ComImport]
    [Guid("00000102-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IEnumMoniker
    {
        [PreserveSig]
        int Next(int celt, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] IMoniker[] rgelt, IntPtr pceltFetched);

        [PreserveSig]
        int Skip(int celt);

        [PreserveSig]
        int Reset();

        [PreserveSig]
        int Clone(out IEnumMoniker? ppenum);
    }

    [ComImport]
    [Guid("0000000F-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IMoniker
    {
        void GetClassID(out Guid pClassId);
        void IsDirty();
        void Load(object pStm);
        void Save(object pStm, bool fClearDirty);
        void GetSizeMax(out long pcbSize);
        void BindToObject([MarshalAs(UnmanagedType.Interface)] object? pbc, [MarshalAs(UnmanagedType.Interface)] object? pmkToLeft, in Guid riidResult, [MarshalAs(UnmanagedType.Interface)] out object ppvResult);
        void BindToStorage([MarshalAs(UnmanagedType.Interface)] object? pbc, [MarshalAs(UnmanagedType.Interface)] object? pmkToLeft, in Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppvObj);
        void Reduce([MarshalAs(UnmanagedType.Interface)] object pbc, int dwReduceHowFar, [MarshalAs(UnmanagedType.Interface)] ref object ppmkToLeft, [MarshalAs(UnmanagedType.Interface)] out object ppmkReduced);
        void ComposeWith([MarshalAs(UnmanagedType.Interface)] object pmkRight, bool fOnlyIfNotGeneric, [MarshalAs(UnmanagedType.Interface)] out object ppmkComposite);
        void Enum(bool fForward, [MarshalAs(UnmanagedType.Interface)] out object ppenumMoniker);
        void IsEqual([MarshalAs(UnmanagedType.Interface)] object pmkOtherMoniker);
        void Hash(out int pdwHash);
        void IsRunning([MarshalAs(UnmanagedType.Interface)] object pbc, [MarshalAs(UnmanagedType.Interface)] object pmkToLeft, [MarshalAs(UnmanagedType.Interface)] object pmkNewlyRunning);
        void GetTimeOfLastChange([MarshalAs(UnmanagedType.Interface)] object pbc, [MarshalAs(UnmanagedType.Interface)] object pmkToLeft, out long pFileTime);
        void Inverse([MarshalAs(UnmanagedType.Interface)] out object ppmk);
        void CommonPrefixWith([MarshalAs(UnmanagedType.Interface)] object pmkOther, [MarshalAs(UnmanagedType.Interface)] out object ppmkPrefix);
        void RelativePathTo([MarshalAs(UnmanagedType.Interface)] object pmkOther, [MarshalAs(UnmanagedType.Interface)] out object ppmkRelPath);
        void GetDisplayName([MarshalAs(UnmanagedType.Interface)] object pbc, [MarshalAs(UnmanagedType.Interface)] object pmkToLeft, [MarshalAs(UnmanagedType.LPWStr)] out string ppszDisplayName);
        void ParseDisplayName([MarshalAs(UnmanagedType.Interface)] object pbc, [MarshalAs(UnmanagedType.Interface)] object pmkToLeft, [MarshalAs(UnmanagedType.LPWStr)] string pszDisplayName, out int pchEaten, [MarshalAs(UnmanagedType.Interface)] out object ppmkOut);
        void IsSystemMoniker(out int pdwMksys);
    }

    [ComImport]
    [Guid("55272A00-42CB-11CE-8135-00AA004BB851")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IPropertyBag
    {
        [PreserveSig]
        int Read([MarshalAs(UnmanagedType.LPWStr)] string propName, [MarshalAs(UnmanagedType.Struct)] out object? pVar, IntPtr pErrorLog);

        [PreserveSig]
        int Write([MarshalAs(UnmanagedType.LPWStr)] string propName, [MarshalAs(UnmanagedType.Struct)] ref object? pVar);
    }

    [ComImport]
    [Guid("56A86895-0AD4-11CE-B03A-0020AF0BA770")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IBaseFilter
    {
        [PreserveSig]
        int GetClassID(out Guid pClassId);

        [PreserveSig]
        int Stop();

        [PreserveSig]
        int Pause();

        [PreserveSig]
        int Run(long tStart);

        [PreserveSig]
        int GetState(int dwMilliSecsTimeout, out int filtState);

        [PreserveSig]
        int SetSyncSource([MarshalAs(UnmanagedType.Interface)] object pClock);

        [PreserveSig]
        int GetSyncSource([MarshalAs(UnmanagedType.Interface)] out object pClock);

        [PreserveSig]
        int EnumPins(out IEnumPins? ppEnum);

        [PreserveSig]
        int FindPin([MarshalAs(UnmanagedType.LPWStr)] string id, out IPin? ppPin);

        [PreserveSig]
        int QueryFilterInfo(out FilterInfo pInfo);

        [PreserveSig]
        int JoinFilterGraph([MarshalAs(UnmanagedType.Interface)] object pGraph, [MarshalAs(UnmanagedType.LPWStr)] string pName);

        [PreserveSig]
        int QueryVendorInfo([MarshalAs(UnmanagedType.LPWStr)] out string pVendorInfo);
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct FilterInfo
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string achName;

        public IntPtr pGraph;
    }

    [ComImport]
    [Guid("56A86892-0AD4-11CE-B03A-0020AF0BA770")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IEnumPins
    {
        [PreserveSig]
        int Next(int cPins, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] IPin[] ppPins, IntPtr pcFetched);

        [PreserveSig]
        int Skip(int cPins);

        [PreserveSig]
        int Reset();

        [PreserveSig]
        int Clone(out IEnumPins? ppEnum);
    }

    [ComImport]
    [Guid("56A86891-0AD4-11CE-B03A-0020AF0BA770")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IPin
    {
        [PreserveSig]
        int Connect([MarshalAs(UnmanagedType.Interface)] IPin pReceivePin, IntPtr pmt);

        [PreserveSig]
        int ReceiveConnection([MarshalAs(UnmanagedType.Interface)] IPin pConnector, IntPtr pmt);

        [PreserveSig]
        int Disconnect();

        [PreserveSig]
        int ConnectedTo(out IPin ppPin);

        [PreserveSig]
        int ConnectionMediaType(IntPtr pmt);

        [PreserveSig]
        int QueryPinInfo(out PinInfo pInfo);

        [PreserveSig]
        int QueryDirection(out PinDirection pPinDir);

        [PreserveSig]
        int QueryId([MarshalAs(UnmanagedType.LPWStr)] out string id);

        [PreserveSig]
        int QueryAccept(IntPtr pmt);

        [PreserveSig]
        int EnumMediaTypes(out IntPtr ppEnum);

        [PreserveSig]
        int QueryInternalConnections(IntPtr apPin, ref int nPin);

        [PreserveSig]
        int EndOfStream();

        [PreserveSig]
        int BeginFlush();

        [PreserveSig]
        int EndFlush();

        [PreserveSig]
        int NewSegment(long tStart, long tStop, double dRate);
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct PinInfo
    {
        public IntPtr filter;
        public PinDirection dir;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string name;
    }

    [ComImport]
    [Guid("C6E13340-30AC-11D0-A18C-00A0C9118956")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IAMStreamConfig
    {
        [PreserveSig]
        int SetFormat(IntPtr pmt);

        [PreserveSig]
        int GetFormat(out IntPtr pmt);

        [PreserveSig]
        int GetNumberOfCapabilities(out int piCount, out int piSize);

        [PreserveSig]
        int GetStreamCaps(int iIndex, out AmMediaType pmt, IntPtr pScc);
    }
}
