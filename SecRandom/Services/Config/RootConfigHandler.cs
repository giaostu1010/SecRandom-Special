using System;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SecRandom.Core.Abstraction;
using SecRandom.Core.Enums;
using SecRandom.Models.Config;

namespace SecRandom.Services.Config;

public class RootConfigHandler : ConfigHandlerBase<RootConfigModel>
{
    private DrawSettingsConfig? _drawSettings;
    private RollCallSettingsConfig? _rollCallSettings;
    private QuickDrawSettingsConfig? _quickDrawSettings;
    private LotterySettingsConfig? _lotterySettings;
    private FaceDetectorSettingsConfig? _faceDetectorSettings;

    public RootConfigHandler(ILogger<RootConfigHandler> logger, ConfigServiceBase configService)
        : base(logger, configService, () => new RootConfigModel())
    {
        TryMigrateLegacyFlatConfig();
        AttachBasicSettingsHandlers();
        AttachDrawSettingsHandlers();
        AttachFloatingWindowSettingsHandlers();
    }

    protected override void Reload()
    {
        DetachBasicSettingsHandlers();
        DetachDrawSettingsHandlers();
        DetachFloatingWindowSettingsHandlers();
        base.Reload();
        AttachBasicSettingsHandlers();
        AttachDrawSettingsHandlers();
        AttachFloatingWindowSettingsHandlers();
    }

    protected override void Data_OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(RootConfigModel.BasicSettings))
        {
            DetachBasicSettingsHandlers();
            AttachBasicSettingsHandlers();
        }
        else if (e.PropertyName == nameof(RootConfigModel.FloatingWindowSettings))
        {
            DetachFloatingWindowSettingsHandlers();
            AttachFloatingWindowSettingsHandlers();
        }
        else if (e.PropertyName == nameof(RootConfigModel.DrawSettings))
        {
            DetachDrawSettingsHandlers();
            AttachDrawSettingsHandlers();
        }

        base.Data_OnPropertyChanged(sender, e);
    }

    private void AttachBasicSettingsHandlers()
    {
        Data.BasicSettings.PropertyChanged += BasicSettings_OnPropertyChanged;
    }

    private void DetachBasicSettingsHandlers()
    {
        Data.BasicSettings.PropertyChanged -= BasicSettings_OnPropertyChanged;
    }

    private void BasicSettings_OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(BasicSettingsConfig.UiLanguageMode))
        {
            App.ApplyUiLanguageMode(Data.BasicSettings.UiLanguageMode);
        }
        else if (e.PropertyName == nameof(BasicSettingsConfig.UiThemeModeIndex))
        {
            App.ApplyUiThemeModeIndex(Data.BasicSettings.UiThemeModeIndex);
        }
        else if (e.PropertyName == nameof(BasicSettingsConfig.UiFontFamilyName) ||
                 e.PropertyName == nameof(BasicSettingsConfig.UiFontFamilyIndex) ||
                 e.PropertyName == nameof(BasicSettingsConfig.UiFontWeightIndex))
        {
            App.ApplyUiFont(
                Data.BasicSettings.UiFontFamilyName,
                Data.BasicSettings.UiFontFamilyIndex,
                Data.BasicSettings.UiFontWeightIndex);
        }

        Save();
    }

    private void AttachDrawSettingsHandlers()
    {
        _drawSettings = Data.DrawSettings;
        _drawSettings.PropertyChanged += DrawSettings_OnPropertyChanged;

        _rollCallSettings = _drawSettings.RollCallSettings;
        _rollCallSettings.PropertyChanged += DrawSettingsChild_OnPropertyChanged;

        _quickDrawSettings = _drawSettings.QuickDrawSettings;
        _quickDrawSettings.PropertyChanged += DrawSettingsChild_OnPropertyChanged;

        _lotterySettings = _drawSettings.LotterySettings;
        _lotterySettings.PropertyChanged += DrawSettingsChild_OnPropertyChanged;

        _faceDetectorSettings = _drawSettings.FaceDetectorSettings;
        _faceDetectorSettings.PropertyChanged += DrawSettingsChild_OnPropertyChanged;
    }

    private void DetachDrawSettingsHandlers()
    {
        if (_drawSettings is not null)
        {
            _drawSettings.PropertyChanged -= DrawSettings_OnPropertyChanged;
            _drawSettings = null;
        }

        if (_rollCallSettings is not null)
        {
            _rollCallSettings.PropertyChanged -= DrawSettingsChild_OnPropertyChanged;
            _rollCallSettings = null;
        }

        if (_quickDrawSettings is not null)
        {
            _quickDrawSettings.PropertyChanged -= DrawSettingsChild_OnPropertyChanged;
            _quickDrawSettings = null;
        }

        if (_lotterySettings is not null)
        {
            _lotterySettings.PropertyChanged -= DrawSettingsChild_OnPropertyChanged;
            _lotterySettings = null;
        }

        if (_faceDetectorSettings is not null)
        {
            _faceDetectorSettings.PropertyChanged -= DrawSettingsChild_OnPropertyChanged;
            _faceDetectorSettings = null;
        }
    }

    private void DrawSettings_OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(DrawSettingsConfig.RollCallSettings) or
            nameof(DrawSettingsConfig.QuickDrawSettings) or
            nameof(DrawSettingsConfig.LotterySettings) or
            nameof(DrawSettingsConfig.FaceDetectorSettings))
        {
            DetachDrawSettingsHandlers();
            AttachDrawSettingsHandlers();
        }

        Save();
    }

    private void DrawSettingsChild_OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        Save();
    }

    private void AttachFloatingWindowSettingsHandlers()
    {
        Data.FloatingWindowSettings.PropertyChanged += FloatingWindowSettings_OnPropertyChanged;
    }

    private void DetachFloatingWindowSettingsHandlers()
    {
        Data.FloatingWindowSettings.PropertyChanged -= FloatingWindowSettings_OnPropertyChanged;
    }

    private void FloatingWindowSettings_OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        Save();
    }

    private void TryMigrateLegacyFlatConfig()
    {
        var path = Data.ConfigFilePath;
        if (!File.Exists(path))
        {
            return;
        }

        try
        {
            using var stream = File.OpenRead(path);
            using var doc = JsonDocument.Parse(stream);
            if (TryGetPropertyIgnoreCase(doc.RootElement, "BasicSettings", out var basicSettingsElement) &&
                basicSettingsElement.ValueKind == JsonValueKind.Object)
            {
                return;
            }

            var legacy = doc.RootElement;
            if (!TryGetPropertyIgnoreCase(legacy, "UiLanguageMode", out _) &&
                !TryGetPropertyIgnoreCase(legacy, "UiThemeModeIndex", out _) &&
                !TryGetPropertyIgnoreCase(legacy, "IsAutoStartEnabled", out _))
            {
                return;
            }

            if (TryGetPropertyIgnoreCase(legacy, "IsAutoStartEnabled", out var isAutoStartEnabled) &&
                isAutoStartEnabled.ValueKind is JsonValueKind.True or JsonValueKind.False)
            {
                Data.BasicSettings.IsAutoStartEnabled = isAutoStartEnabled.GetBoolean();
            }

            if (TryGetPropertyIgnoreCase(legacy, "IsShowMainWindowOnStartupEnabled", out var isShowMainWindowOnStartupEnabled) &&
                isShowMainWindowOnStartupEnabled.ValueKind is JsonValueKind.True or JsonValueKind.False)
            {
                Data.BasicSettings.IsShowMainWindowOnStartupEnabled = isShowMainWindowOnStartupEnabled.GetBoolean();
            }

            if (TryGetPropertyIgnoreCase(legacy, "IsAutoSaveWindowSizeEnabled", out var isAutoSaveWindowSizeEnabled) &&
                isAutoSaveWindowSizeEnabled.ValueKind is JsonValueKind.True or JsonValueKind.False)
            {
                Data.BasicSettings.IsAutoSaveWindowSizeEnabled = isAutoSaveWindowSizeEnabled.GetBoolean();
            }

            if (TryGetPropertyIgnoreCase(legacy, "MainWindowTopmostMode", out var mainWindowTopmostMode) &&
                mainWindowTopmostMode.ValueKind == JsonValueKind.Number &&
                mainWindowTopmostMode.TryGetInt32(out var topmostMode))
            {
                Data.BasicSettings.MainWindowTopmostMode = topmostMode;
            }

            if (TryGetPropertyIgnoreCase(legacy, "IsBackgroundResidentEnabled", out var isBackgroundResidentEnabled) &&
                isBackgroundResidentEnabled.ValueKind is JsonValueKind.True or JsonValueKind.False)
            {
                Data.BasicSettings.IsBackgroundResidentEnabled = isBackgroundResidentEnabled.GetBoolean();
            }

            if (TryGetPropertyIgnoreCase(legacy, "IsUrlProtocolAndIpcEnabled", out var isUrlProtocolAndIpcEnabled) &&
                isUrlProtocolAndIpcEnabled.ValueKind is JsonValueKind.True or JsonValueKind.False)
            {
                Data.BasicSettings.IsUrlProtocolAndIpcEnabled = isUrlProtocolAndIpcEnabled.GetBoolean();
            }

            if (TryGetPropertyIgnoreCase(legacy, "UiLanguageMode", out var uiLanguageMode) &&
                uiLanguageMode.ValueKind == JsonValueKind.Number &&
                uiLanguageMode.TryGetInt32(out var languageMode))
            {
                Data.BasicSettings.UiLanguageMode = (UiLanguageMode)languageMode;
            }

            if (TryGetPropertyIgnoreCase(legacy, "UiThemeModeIndex", out var uiThemeModeIndex) &&
                uiThemeModeIndex.ValueKind == JsonValueKind.Number &&
                uiThemeModeIndex.TryGetInt32(out var themeModeIndex))
            {
                Data.BasicSettings.UiThemeModeIndex = themeModeIndex;
            }

            if (TryGetPropertyIgnoreCase(legacy, "UiFontFamilyIndex", out var uiFontFamilyIndex) &&
                uiFontFamilyIndex.ValueKind == JsonValueKind.Number &&
                uiFontFamilyIndex.TryGetInt32(out var fontFamilyIndex))
            {
                Data.BasicSettings.UiFontFamilyIndex = fontFamilyIndex;
            }

            if (TryGetPropertyIgnoreCase(legacy, "UiFontFamilyName", out var uiFontFamilyName) &&
                uiFontFamilyName.ValueKind == JsonValueKind.String)
            {
                Data.BasicSettings.UiFontFamilyName = uiFontFamilyName.GetString() ?? Data.BasicSettings.UiFontFamilyName;
            }

            if (TryGetPropertyIgnoreCase(legacy, "UiFontWeightIndex", out var uiFontWeightIndex) &&
                uiFontWeightIndex.ValueKind == JsonValueKind.Number &&
                uiFontWeightIndex.TryGetInt32(out var fontWeightIndex))
            {
                Data.BasicSettings.UiFontWeightIndex = fontWeightIndex;
            }

            if (TryGetPropertyIgnoreCase(legacy, "UiDpiScaleIndex", out var uiDpiScaleIndex) &&
                uiDpiScaleIndex.ValueKind == JsonValueKind.Number &&
                uiDpiScaleIndex.TryGetInt32(out var dpiScaleIndex))
            {
                Data.BasicSettings.UiDpiScaleIndex = dpiScaleIndex;
            }

            Save();
        }
        catch
        {
        }
    }

    private static bool TryGetPropertyIgnoreCase(JsonElement element, string name, out JsonElement value)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals(name) || property.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    value = property.Value;
                    return true;
                }
            }
        }

        value = default;
        return false;
    }
}
