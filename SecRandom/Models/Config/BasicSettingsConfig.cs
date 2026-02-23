using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using SecRandom.Core.Enums;

namespace SecRandom.Models.Config;

/// <summary>
/// 基础设置配置模型
/// </summary>
public partial class BasicSettingsConfig : ObservableObject
{
    [ObservableProperty] private bool _isAutoStartEnabled = false;
    [ObservableProperty] private bool _isShowMainWindowOnStartupEnabled = true;
    [ObservableProperty] private bool _isAutoSaveWindowSizeEnabled = true;
    [ObservableProperty] private int _mainWindowTopmostMode = 0;
    [ObservableProperty] private bool _isBackgroundResidentEnabled = true;
    [ObservableProperty] private bool _isUrlProtocolAndIpcEnabled = true;
    [ObservableProperty] private UiLanguageMode _uiLanguageMode = UiLanguageMode.FollowSystem;
    [ObservableProperty] private int _uiThemeModeIndex = 0;
    [ObservableProperty] private int _uiFontFamilyIndex = 0;
    [ObservableProperty] private string _uiFontFamilyName = "HarmonyOS Sans SC";
    [ObservableProperty] private int _uiFontWeightIndex = 0;
    [ObservableProperty] private int _uiDpiScaleIndex = 5;
}
