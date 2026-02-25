using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace SecRandom.Models.Config;

/// <summary>
/// 悬浮窗设置配置模型
/// </summary>
public partial class FloatingWindowSettingsConfig : ObservableObject
{
    [ObservableProperty] private bool _isShowFloatingWindowOnStartupEnabled = true;
    [ObservableProperty] private double _floatingWindowOpacity = 0.6;
    [ObservableProperty] private int _floatingWindowTopmostMode = 1;
    [ObservableProperty] private bool _isExtendQuickDrawComponentEnabled = false;
    [ObservableProperty] private bool _isAcrylicBackgroundEnabled = true;

    [ObservableProperty] private ObservableCollection<string> _floatingWindowButtonControl = ["roll_call", "quick_draw"];

    [ObservableProperty] private int _floatingWindowPlacement = 1;
    [ObservableProperty] private int _floatingWindowDisplayStyle = 0;
    [ObservableProperty] private int _floatingWindowSize = 3;
    [ObservableProperty] private int _floatingWindowThemeMode = 0;

    [ObservableProperty] private bool _isFloatingWindowStickToEdgeEnabled = true;
    [ObservableProperty] private int _floatingWindowStickToEdgeRecoverSeconds = 3;
    [ObservableProperty] private int _floatingWindowStickToEdgeDisplayStyle = 1;

    [ObservableProperty] private bool _isFloatingWindowDraggableEnabled = true;
    [ObservableProperty] private int _floatingWindowLongPressDuration = 100;

    [ObservableProperty] private bool _isDoNotStealFocusEnabled = true;

    [ObservableProperty] private bool _isHideFloatingWindowOnForegroundEnabled = false;
    [ObservableProperty] private string _hideFloatingWindowOnForegroundWindowTitles = string.Empty;
    [ObservableProperty] private string _hideFloatingWindowOnForegroundProcessNames = string.Empty;

    [ObservableProperty] private int _floatingWindowPositionX = 100;
    [ObservableProperty] private int _floatingWindowPositionY = 100;

    [JsonIgnore]
    public int FloatingWindowOpacityPercent
    {
        get => (int)Math.Round(FloatingWindowOpacity * 100, MidpointRounding.AwayFromZero);
        set => FloatingWindowOpacity = Math.Clamp(value, 0, 100) / 100.0;
    }

    partial void OnFloatingWindowOpacityChanged(double value)
    {
        OnPropertyChanged(nameof(FloatingWindowOpacityPercent));
    }
}
