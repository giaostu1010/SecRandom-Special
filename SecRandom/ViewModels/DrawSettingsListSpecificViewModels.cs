using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using SecRandom.Core;
using SecRandom.Models.Config;
using SecRandom.Services.Config;

namespace SecRandom.ViewModels;

public sealed partial class ListNamesSource : ObservableObject, IDisposable
{
    private readonly string _directoryPath;
    private readonly string _searchPattern;
    private readonly FileSystemWatcher? _watcher;
    private readonly Timer _refreshTimer;

    [ObservableProperty] private List<string> _names = [];

    public ListNamesSource(string directoryPath, string searchPattern = "*.json")
    {
        _directoryPath = directoryPath;
        _searchPattern = searchPattern;

        _refreshTimer = new Timer(_ => Dispatcher.UIThread.Post(Refresh), null, Timeout.Infinite, Timeout.Infinite);

        Directory.CreateDirectory(_directoryPath);
        _watcher = new FileSystemWatcher(_directoryPath, _searchPattern)
        {
            IncludeSubdirectories = false,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.CreationTime
        };

        _watcher.Created += Watcher_OnChanged;
        _watcher.Changed += Watcher_OnChanged;
        _watcher.Deleted += Watcher_OnChanged;
        _watcher.Renamed += Watcher_OnRenamed;
        _watcher.EnableRaisingEvents = true;

        Refresh();
    }

    private void Watcher_OnChanged(object sender, FileSystemEventArgs e)
    {
        _refreshTimer.Change(300, Timeout.Infinite);
    }

    private void Watcher_OnRenamed(object sender, RenamedEventArgs e)
    {
        _refreshTimer.Change(300, Timeout.Infinite);
    }

    public void Refresh()
    {
        if (!Directory.Exists(_directoryPath))
        {
            if (Names.Count != 0)
            {
                Names = [];
            }
            return;
        }

        var next = Directory.EnumerateFiles(_directoryPath, _searchPattern)
            .Select(path => Path.GetFileNameWithoutExtension(path) ?? string.Empty)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.Ordinal)
            .OrderBy(x => x, StringComparer.Ordinal)
            .ToList();

        if (Names.Count == next.Count)
        {
            var same = true;
            for (var i = 0; i < next.Count; i++)
            {
                if (!string.Equals(Names[i], next[i], StringComparison.Ordinal))
                {
                    same = false;
                    break;
                }
            }

            if (same)
            {
                return;
            }
        }

        Names = next;
    }

    public void Dispose()
    {
        _watcher?.Dispose();
        _refreshTimer.Dispose();
    }
}

public abstract partial class ListSpecificSettingsViewModelBase : ObservableObject, IDisposable
{
    protected MainConfigHandler MainConfigHandler { get; }
    protected ListNamesSource NamesSource { get; }

    protected ListSpecificSettingsViewModelBase(MainConfigHandler mainConfigHandler, string directoryPath)
    {
        MainConfigHandler = mainConfigHandler;
        NamesSource = new ListNamesSource(directoryPath);
        NamesSource.PropertyChanged += NamesSource_OnPropertyChanged;
    }

    public IReadOnlyList<string> ListNames => NamesSource.Names;

    private void NamesSource_OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ListNamesSource.Names))
        {
            OnPropertyChanged(nameof(ListNames));
        }
    }

    public virtual void Dispose()
    {
        NamesSource.PropertyChanged -= NamesSource_OnPropertyChanged;
        NamesSource.Dispose();
        GC.SuppressFinalize(this);
    }
}

public sealed partial class RollCallListSpecificSettingsViewModel(MainConfigHandler mainConfigHandler)
    : ListSpecificSettingsViewModelBase(mainConfigHandler, Utils.GetFilePath("list", "roll_call_list"))
{
    [ObservableProperty] private string _selectedClassName = string.Empty;
    [ObservableProperty] private bool _isSyncEnabled;
    [ObservableProperty] private RollCallSettingsOverrideProxy _settings = new(mainConfigHandler, string.Empty);

    partial void OnSelectedClassNameChanged(string value)
    {
        Settings = new RollCallSettingsOverrideProxy(MainConfigHandler, value);
        IsSyncEnabled = HasOverrides(value);
    }

    private bool HasOverrides(string listName)
    {
        if (string.IsNullOrWhiteSpace(listName))
        {
            return false;
        }

        return MainConfigHandler.Data.DrawSettings.RollCallListSpecificOverrides.ContainsKey(listName);
    }

    public void SyncWithGlobal()
    {
        var listName = SelectedClassName;
        if (string.IsNullOrWhiteSpace(listName))
        {
            return;
        }

        var current = MainConfigHandler.Data.DrawSettings.RollCallListSpecificOverrides;
        if (!current.ContainsKey(listName))
        {
            IsSyncEnabled = false;
            Settings = new RollCallSettingsOverrideProxy(MainConfigHandler, listName);
            return;
        }

        var next = new Dictionary<string, RollCallSettingsOverrideConfig>(current, StringComparer.Ordinal);
        next.Remove(listName);
        MainConfigHandler.Data.DrawSettings.RollCallListSpecificOverrides = next;

        IsSyncEnabled = false;
        Settings = new RollCallSettingsOverrideProxy(MainConfigHandler, listName);
    }
}

public sealed partial class QuickDrawListSpecificSettingsViewModel(MainConfigHandler mainConfigHandler)
    : ListSpecificSettingsViewModelBase(mainConfigHandler, Utils.GetFilePath("list", "roll_call_list"))
{
    [ObservableProperty] private string _selectedListName = string.Empty;
    [ObservableProperty] private bool _isSyncEnabled;
    [ObservableProperty] private QuickDrawSettingsOverrideProxy _settings = new(mainConfigHandler, string.Empty);

    partial void OnSelectedListNameChanged(string value)
    {
        Settings = new QuickDrawSettingsOverrideProxy(MainConfigHandler, value);
        IsSyncEnabled = HasOverrides(value);
    }

    private bool HasOverrides(string listName)
    {
        if (string.IsNullOrWhiteSpace(listName))
        {
            return false;
        }

        return MainConfigHandler.Data.DrawSettings.QuickDrawListSpecificOverrides.ContainsKey(listName);
    }

    public void SyncWithGlobal()
    {
        var listName = SelectedListName;
        if (string.IsNullOrWhiteSpace(listName))
        {
            return;
        }

        var current = MainConfigHandler.Data.DrawSettings.QuickDrawListSpecificOverrides;
        if (!current.ContainsKey(listName))
        {
            IsSyncEnabled = false;
            Settings = new QuickDrawSettingsOverrideProxy(MainConfigHandler, listName);
            return;
        }

        var next = new Dictionary<string, QuickDrawSettingsOverrideConfig>(current, StringComparer.Ordinal);
        next.Remove(listName);
        MainConfigHandler.Data.DrawSettings.QuickDrawListSpecificOverrides = next;

        IsSyncEnabled = false;
        Settings = new QuickDrawSettingsOverrideProxy(MainConfigHandler, listName);
    }
}

public sealed partial class LotteryListSpecificSettingsViewModel(MainConfigHandler mainConfigHandler)
    : ListSpecificSettingsViewModelBase(mainConfigHandler, Utils.GetFilePath("list", "lottery_list"))
{
    [ObservableProperty] private string _selectedPoolName = string.Empty;
    [ObservableProperty] private bool _isSyncEnabled;
    [ObservableProperty] private LotterySettingsOverrideProxy _settings = new(mainConfigHandler, string.Empty);

    partial void OnSelectedPoolNameChanged(string value)
    {
        Settings = new LotterySettingsOverrideProxy(MainConfigHandler, value);
        IsSyncEnabled = HasOverrides(value);
    }

    private bool HasOverrides(string poolName)
    {
        if (string.IsNullOrWhiteSpace(poolName))
        {
            return false;
        }

        return MainConfigHandler.Data.DrawSettings.LotteryListSpecificOverrides.ContainsKey(poolName);
    }

    public void SyncWithGlobal()
    {
        var poolName = SelectedPoolName;
        if (string.IsNullOrWhiteSpace(poolName))
        {
            return;
        }

        var current = MainConfigHandler.Data.DrawSettings.LotteryListSpecificOverrides;
        if (!current.ContainsKey(poolName))
        {
            IsSyncEnabled = false;
            Settings = new LotterySettingsOverrideProxy(MainConfigHandler, poolName);
            return;
        }

        var next = new Dictionary<string, LotterySettingsOverrideConfig>(current, StringComparer.Ordinal);
        next.Remove(poolName);
        MainConfigHandler.Data.DrawSettings.LotteryListSpecificOverrides = next;

        IsSyncEnabled = false;
        Settings = new LotterySettingsOverrideProxy(MainConfigHandler, poolName);
    }
}

public sealed class RollCallSettingsOverrideProxy(MainConfigHandler mainConfigHandler, string listName)
{
    private readonly string _listName = listName?.Trim() ?? string.Empty;

    private RollCallSettingsConfig Global => mainConfigHandler.Data.DrawSettings.RollCallSettings;

    private RollCallSettingsOverrideConfig? TryGetOverride()
    {
        if (string.IsNullOrWhiteSpace(_listName))
        {
            return null;
        }

        var dict = mainConfigHandler.Data.DrawSettings.RollCallListSpecificOverrides;
        return dict.TryGetValue(_listName, out var v) ? v : null;
    }

    private static RollCallSettingsOverrideConfig Clone(RollCallSettingsOverrideConfig? source)
    {
        if (source is null)
        {
            return new RollCallSettingsOverrideConfig();
        }

        return new RollCallSettingsOverrideConfig
        {
            DrawMode = source.DrawMode,
            ClearRecord = source.ClearRecord,
            HalfRepeat = source.HalfRepeat,
            DrawType = source.DrawType,
            DefaultClass = source.DefaultClass,
            FontSize = source.FontSize,
            UseGlobalFont = source.UseGlobalFont,
            CustomFont = source.CustomFont,
            DisplayFormat = source.DisplayFormat,
            DisplayStyle = source.DisplayStyle,
            ShowRandom = source.ShowRandom,
            ShowTags = source.ShowTags,
            Animation = source.Animation,
            AnimationInterval = source.AnimationInterval,
            AutoplayCount = source.AutoplayCount,
            ResultFlowAnimationStyle = source.ResultFlowAnimationStyle,
            ResultFlowAnimationDuration = source.ResultFlowAnimationDuration,
            AnimationColorTheme = source.AnimationColorTheme,
            AnimationFixedColor = source.AnimationFixedColor,
            StudentImage = source.StudentImage,
            StudentImagePosition = source.StudentImagePosition
        };
    }

    private void SetOverride(Func<RollCallSettingsOverrideConfig, RollCallSettingsOverrideConfig> updater)
    {
        if (string.IsNullOrWhiteSpace(_listName))
        {
            return;
        }

        var drawSettings = mainConfigHandler.Data.DrawSettings;
        var current = drawSettings.RollCallListSpecificOverrides;
        current.TryGetValue(_listName, out var existing);
        var nextEntry = updater(Clone(existing));

        var next = new Dictionary<string, RollCallSettingsOverrideConfig>(current, StringComparer.Ordinal)
        {
            [_listName] = nextEntry
        };
        drawSettings.RollCallListSpecificOverrides = next;
    }

    public int DrawMode
    {
        get => TryGetOverride()?.DrawMode ?? Global.DrawMode;
        set => SetOverride(o => { o.DrawMode = value; return o; });
    }

    public int ClearRecord
    {
        get => TryGetOverride()?.ClearRecord ?? Global.ClearRecord;
        set => SetOverride(o => { o.ClearRecord = value; return o; });
    }

    public int HalfRepeat
    {
        get => TryGetOverride()?.HalfRepeat ?? Global.HalfRepeat;
        set => SetOverride(o => { o.HalfRepeat = value; return o; });
    }

    public int DrawType
    {
        get => TryGetOverride()?.DrawType ?? Global.DrawType;
        set => SetOverride(o => { o.DrawType = value; return o; });
    }

    public string DefaultClass
    {
        get => TryGetOverride()?.DefaultClass ?? Global.DefaultClass;
        set => SetOverride(o => { o.DefaultClass = value; return o; });
    }

    public int FontSize
    {
        get => TryGetOverride()?.FontSize ?? Global.FontSize;
        set => SetOverride(o => { o.FontSize = value; return o; });
    }

    public int UseGlobalFont
    {
        get => TryGetOverride()?.UseGlobalFont ?? Global.UseGlobalFont;
        set => SetOverride(o => { o.UseGlobalFont = value; return o; });
    }

    public string CustomFont
    {
        get => TryGetOverride()?.CustomFont ?? Global.CustomFont;
        set => SetOverride(o => { o.CustomFont = value; return o; });
    }

    public int DisplayFormat
    {
        get => TryGetOverride()?.DisplayFormat ?? Global.DisplayFormat;
        set => SetOverride(o => { o.DisplayFormat = value; return o; });
    }

    public int DisplayStyle
    {
        get => TryGetOverride()?.DisplayStyle ?? Global.DisplayStyle;
        set => SetOverride(o => { o.DisplayStyle = value; return o; });
    }

    public int ShowRandom
    {
        get => TryGetOverride()?.ShowRandom ?? Global.ShowRandom;
        set => SetOverride(o => { o.ShowRandom = value; return o; });
    }

    public bool ShowTags
    {
        get => TryGetOverride()?.ShowTags ?? Global.ShowTags;
        set => SetOverride(o => { o.ShowTags = value; return o; });
    }

    public int Animation
    {
        get => TryGetOverride()?.Animation ?? Global.Animation;
        set => SetOverride(o => { o.Animation = value; return o; });
    }

    public int AnimationInterval
    {
        get => TryGetOverride()?.AnimationInterval ?? Global.AnimationInterval;
        set => SetOverride(o => { o.AnimationInterval = value; return o; });
    }

    public int AutoplayCount
    {
        get => TryGetOverride()?.AutoplayCount ?? Global.AutoplayCount;
        set => SetOverride(o => { o.AutoplayCount = value; return o; });
    }

    public bool ResultFlowAnimationStyle
    {
        get => TryGetOverride()?.ResultFlowAnimationStyle ?? Global.ResultFlowAnimationStyle;
        set => SetOverride(o => { o.ResultFlowAnimationStyle = value; return o; });
    }

    public int ResultFlowAnimationDuration
    {
        get => TryGetOverride()?.ResultFlowAnimationDuration ?? Global.ResultFlowAnimationDuration;
        set => SetOverride(o => { o.ResultFlowAnimationDuration = value; return o; });
    }

    public int AnimationColorTheme
    {
        get => TryGetOverride()?.AnimationColorTheme ?? Global.AnimationColorTheme;
        set => SetOverride(o => { o.AnimationColorTheme = value; return o; });
    }

    public string AnimationFixedColor
    {
        get => TryGetOverride()?.AnimationFixedColor ?? Global.AnimationFixedColor;
        set => SetOverride(o => { o.AnimationFixedColor = value; return o; });
    }

    public bool StudentImage
    {
        get => TryGetOverride()?.StudentImage ?? Global.StudentImage;
        set => SetOverride(o => { o.StudentImage = value; return o; });
    }

    public int StudentImagePosition
    {
        get => TryGetOverride()?.StudentImagePosition ?? Global.StudentImagePosition;
        set => SetOverride(o => { o.StudentImagePosition = value; return o; });
    }
}

public sealed class QuickDrawSettingsOverrideProxy(MainConfigHandler mainConfigHandler, string listName)
{
    private readonly string _listName = listName?.Trim() ?? string.Empty;

    private QuickDrawSettingsConfig Global => mainConfigHandler.Data.DrawSettings.QuickDrawSettings;

    private QuickDrawSettingsOverrideConfig? TryGetOverride()
    {
        if (string.IsNullOrWhiteSpace(_listName))
        {
            return null;
        }

        var dict = mainConfigHandler.Data.DrawSettings.QuickDrawListSpecificOverrides;
        return dict.TryGetValue(_listName, out var v) ? v : null;
    }

    private static QuickDrawSettingsOverrideConfig Clone(QuickDrawSettingsOverrideConfig? source)
    {
        if (source is null)
        {
            return new QuickDrawSettingsOverrideConfig();
        }

        return new QuickDrawSettingsOverrideConfig
        {
            DrawMode = source.DrawMode,
            HalfRepeat = source.HalfRepeat,
            DrawType = source.DrawType,
            DefaultClass = source.DefaultClass,
            DrawCount = source.DrawCount,
            DisableAfterClick = source.DisableAfterClick,
            FontSize = source.FontSize,
            UseGlobalFont = source.UseGlobalFont,
            CustomFont = source.CustomFont,
            DisplayFormat = source.DisplayFormat,
            ShowRandom = source.ShowRandom,
            ShowTags = source.ShowTags,
            Animation = source.Animation,
            AnimationInterval = source.AnimationInterval,
            AutoplayCount = source.AutoplayCount,
            ResultFlowAnimationStyle = source.ResultFlowAnimationStyle,
            ResultFlowAnimationDuration = source.ResultFlowAnimationDuration,
            AnimationColorTheme = source.AnimationColorTheme,
            AnimationFixedColor = source.AnimationFixedColor,
            StudentImage = source.StudentImage,
            StudentImagePosition = source.StudentImagePosition
        };
    }

    private void SetOverride(Func<QuickDrawSettingsOverrideConfig, QuickDrawSettingsOverrideConfig> updater)
    {
        if (string.IsNullOrWhiteSpace(_listName))
        {
            return;
        }

        var drawSettings = mainConfigHandler.Data.DrawSettings;
        var current = drawSettings.QuickDrawListSpecificOverrides;
        current.TryGetValue(_listName, out var existing);
        var nextEntry = updater(Clone(existing));

        var next = new Dictionary<string, QuickDrawSettingsOverrideConfig>(current, StringComparer.Ordinal)
        {
            [_listName] = nextEntry
        };
        drawSettings.QuickDrawListSpecificOverrides = next;
    }

    public int DrawMode
    {
        get => TryGetOverride()?.DrawMode ?? Global.DrawMode;
        set => SetOverride(o => { o.DrawMode = value; return o; });
    }

    public int HalfRepeat
    {
        get => TryGetOverride()?.HalfRepeat ?? Global.HalfRepeat;
        set => SetOverride(o => { o.HalfRepeat = value; return o; });
    }

    public int DrawType
    {
        get => TryGetOverride()?.DrawType ?? Global.DrawType;
        set => SetOverride(o => { o.DrawType = value; return o; });
    }

    public string DefaultClass
    {
        get => TryGetOverride()?.DefaultClass ?? Global.DefaultClass;
        set => SetOverride(o => { o.DefaultClass = value; return o; });
    }

    public int DrawCount
    {
        get => TryGetOverride()?.DrawCount ?? Global.DrawCount;
        set => SetOverride(o => { o.DrawCount = value; return o; });
    }

    public int DisableAfterClick
    {
        get => TryGetOverride()?.DisableAfterClick ?? Global.DisableAfterClick;
        set => SetOverride(o => { o.DisableAfterClick = value; return o; });
    }

    public int FontSize
    {
        get => TryGetOverride()?.FontSize ?? Global.FontSize;
        set => SetOverride(o => { o.FontSize = value; return o; });
    }

    public int UseGlobalFont
    {
        get => TryGetOverride()?.UseGlobalFont ?? Global.UseGlobalFont;
        set => SetOverride(o => { o.UseGlobalFont = value; return o; });
    }

    public string CustomFont
    {
        get => TryGetOverride()?.CustomFont ?? Global.CustomFont;
        set => SetOverride(o => { o.CustomFont = value; return o; });
    }

    public int DisplayFormat
    {
        get => TryGetOverride()?.DisplayFormat ?? Global.DisplayFormat;
        set => SetOverride(o => { o.DisplayFormat = value; return o; });
    }

    public int ShowRandom
    {
        get => TryGetOverride()?.ShowRandom ?? Global.ShowRandom;
        set => SetOverride(o => { o.ShowRandom = value; return o; });
    }

    public bool ShowTags
    {
        get => TryGetOverride()?.ShowTags ?? Global.ShowTags;
        set => SetOverride(o => { o.ShowTags = value; return o; });
    }

    public int Animation
    {
        get => TryGetOverride()?.Animation ?? Global.Animation;
        set => SetOverride(o => { o.Animation = value; return o; });
    }

    public int AnimationInterval
    {
        get => TryGetOverride()?.AnimationInterval ?? Global.AnimationInterval;
        set => SetOverride(o => { o.AnimationInterval = value; return o; });
    }

    public int AutoplayCount
    {
        get => TryGetOverride()?.AutoplayCount ?? Global.AutoplayCount;
        set => SetOverride(o => { o.AutoplayCount = value; return o; });
    }

    public bool ResultFlowAnimationStyle
    {
        get => TryGetOverride()?.ResultFlowAnimationStyle ?? Global.ResultFlowAnimationStyle;
        set => SetOverride(o => { o.ResultFlowAnimationStyle = value; return o; });
    }

    public int ResultFlowAnimationDuration
    {
        get => TryGetOverride()?.ResultFlowAnimationDuration ?? Global.ResultFlowAnimationDuration;
        set => SetOverride(o => { o.ResultFlowAnimationDuration = value; return o; });
    }

    public int AnimationColorTheme
    {
        get => TryGetOverride()?.AnimationColorTheme ?? Global.AnimationColorTheme;
        set => SetOverride(o => { o.AnimationColorTheme = value; return o; });
    }

    public string AnimationFixedColor
    {
        get => TryGetOverride()?.AnimationFixedColor ?? Global.AnimationFixedColor;
        set => SetOverride(o => { o.AnimationFixedColor = value; return o; });
    }

    public bool StudentImage
    {
        get => TryGetOverride()?.StudentImage ?? Global.StudentImage;
        set => SetOverride(o => { o.StudentImage = value; return o; });
    }

    public int StudentImagePosition
    {
        get => TryGetOverride()?.StudentImagePosition ?? Global.StudentImagePosition;
        set => SetOverride(o => { o.StudentImagePosition = value; return o; });
    }
}

public sealed class LotterySettingsOverrideProxy(MainConfigHandler mainConfigHandler, string poolName)
{
    private readonly string _poolName = poolName?.Trim() ?? string.Empty;

    private LotterySettingsConfig Global => mainConfigHandler.Data.DrawSettings.LotterySettings;

    private LotterySettingsOverrideConfig? TryGetOverride()
    {
        if (string.IsNullOrWhiteSpace(_poolName))
        {
            return null;
        }

        var dict = mainConfigHandler.Data.DrawSettings.LotteryListSpecificOverrides;
        return dict.TryGetValue(_poolName, out var v) ? v : null;
    }

    private static LotterySettingsOverrideConfig Clone(LotterySettingsOverrideConfig? source)
    {
        if (source is null)
        {
            return new LotterySettingsOverrideConfig();
        }

        return new LotterySettingsOverrideConfig
        {
            DrawMode = source.DrawMode,
            ClearRecord = source.ClearRecord,
            HalfRepeat = source.HalfRepeat,
            DrawType = source.DrawType,
            DefaultPool = source.DefaultPool,
            FontSize = source.FontSize,
            UseGlobalFont = source.UseGlobalFont,
            CustomFont = source.CustomFont,
            DisplayFormat = source.DisplayFormat,
            DisplayStyle = source.DisplayStyle,
            ShowRandom = source.ShowRandom,
            ShowTags = source.ShowTags,
            Animation = source.Animation,
            AnimationInterval = source.AnimationInterval,
            AutoplayCount = source.AutoplayCount,
            ResultFlowAnimationStyle = source.ResultFlowAnimationStyle,
            ResultFlowAnimationDuration = source.ResultFlowAnimationDuration,
            AnimationColorTheme = source.AnimationColorTheme,
            AnimationFixedColor = source.AnimationFixedColor,
            LotteryImage = source.LotteryImage,
            LotteryImagePosition = source.LotteryImagePosition
        };
    }

    private void SetOverride(Func<LotterySettingsOverrideConfig, LotterySettingsOverrideConfig> updater)
    {
        if (string.IsNullOrWhiteSpace(_poolName))
        {
            return;
        }

        var drawSettings = mainConfigHandler.Data.DrawSettings;
        var current = drawSettings.LotteryListSpecificOverrides;
        current.TryGetValue(_poolName, out var existing);
        var nextEntry = updater(Clone(existing));

        var next = new Dictionary<string, LotterySettingsOverrideConfig>(current, StringComparer.Ordinal)
        {
            [_poolName] = nextEntry
        };
        drawSettings.LotteryListSpecificOverrides = next;
    }

    public int DrawMode
    {
        get => TryGetOverride()?.DrawMode ?? Global.DrawMode;
        set => SetOverride(o => { o.DrawMode = value; return o; });
    }

    public int ClearRecord
    {
        get => TryGetOverride()?.ClearRecord ?? Global.ClearRecord;
        set => SetOverride(o => { o.ClearRecord = value; return o; });
    }

    public int HalfRepeat
    {
        get => TryGetOverride()?.HalfRepeat ?? Global.HalfRepeat;
        set => SetOverride(o => { o.HalfRepeat = value; return o; });
    }

    public int DrawType
    {
        get => TryGetOverride()?.DrawType ?? Global.DrawType;
        set => SetOverride(o => { o.DrawType = value; return o; });
    }

    public string DefaultPool
    {
        get => TryGetOverride()?.DefaultPool ?? Global.DefaultPool;
        set => SetOverride(o => { o.DefaultPool = value; return o; });
    }

    public int FontSize
    {
        get => TryGetOverride()?.FontSize ?? Global.FontSize;
        set => SetOverride(o => { o.FontSize = value; return o; });
    }

    public int UseGlobalFont
    {
        get => TryGetOverride()?.UseGlobalFont ?? Global.UseGlobalFont;
        set => SetOverride(o => { o.UseGlobalFont = value; return o; });
    }

    public string CustomFont
    {
        get => TryGetOverride()?.CustomFont ?? Global.CustomFont;
        set => SetOverride(o => { o.CustomFont = value; return o; });
    }

    public int DisplayFormat
    {
        get => TryGetOverride()?.DisplayFormat ?? Global.DisplayFormat;
        set => SetOverride(o => { o.DisplayFormat = value; return o; });
    }

    public int DisplayStyle
    {
        get => TryGetOverride()?.DisplayStyle ?? Global.DisplayStyle;
        set => SetOverride(o => { o.DisplayStyle = value; return o; });
    }

    public int ShowRandom
    {
        get => TryGetOverride()?.ShowRandom ?? Global.ShowRandom;
        set => SetOverride(o => { o.ShowRandom = value; return o; });
    }

    public bool ShowTags
    {
        get => TryGetOverride()?.ShowTags ?? Global.ShowTags;
        set => SetOverride(o => { o.ShowTags = value; return o; });
    }

    public int Animation
    {
        get => TryGetOverride()?.Animation ?? Global.Animation;
        set => SetOverride(o => { o.Animation = value; return o; });
    }

    public int AnimationInterval
    {
        get => TryGetOverride()?.AnimationInterval ?? Global.AnimationInterval;
        set => SetOverride(o => { o.AnimationInterval = value; return o; });
    }

    public int AutoplayCount
    {
        get => TryGetOverride()?.AutoplayCount ?? Global.AutoplayCount;
        set => SetOverride(o => { o.AutoplayCount = value; return o; });
    }

    public bool ResultFlowAnimationStyle
    {
        get => TryGetOverride()?.ResultFlowAnimationStyle ?? Global.ResultFlowAnimationStyle;
        set => SetOverride(o => { o.ResultFlowAnimationStyle = value; return o; });
    }

    public int ResultFlowAnimationDuration
    {
        get => TryGetOverride()?.ResultFlowAnimationDuration ?? Global.ResultFlowAnimationDuration;
        set => SetOverride(o => { o.ResultFlowAnimationDuration = value; return o; });
    }

    public int AnimationColorTheme
    {
        get => TryGetOverride()?.AnimationColorTheme ?? Global.AnimationColorTheme;
        set => SetOverride(o => { o.AnimationColorTheme = value; return o; });
    }

    public string AnimationFixedColor
    {
        get => TryGetOverride()?.AnimationFixedColor ?? Global.AnimationFixedColor;
        set => SetOverride(o => { o.AnimationFixedColor = value; return o; });
    }

    public bool LotteryImage
    {
        get => TryGetOverride()?.LotteryImage ?? Global.LotteryImage;
        set => SetOverride(o => { o.LotteryImage = value; return o; });
    }

    public int LotteryImagePosition
    {
        get => TryGetOverride()?.LotteryImagePosition ?? Global.LotteryImagePosition;
        set => SetOverride(o => { o.LotteryImagePosition = value; return o; });
    }
}
