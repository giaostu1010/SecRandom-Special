using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Text.Json.Serialization;

namespace SecRandom.Models.Config;

public partial class NotificationSettingsConfig : ObservableObject
{
    [ObservableProperty] private bool _callNotificationService = false;
    [ObservableProperty] private bool _animation = true;
    [ObservableProperty] private string _floatingWindowEnabledMonitor = "OFF";
    [ObservableProperty] private int _floatingWindowPosition = 0;
    [ObservableProperty] private int _floatingWindowHorizontalOffset = 0;
    [ObservableProperty] private int _floatingWindowVerticalOffset = 0;
    [ObservableProperty] private double _floatingWindowTransparency = 0.8;
    [ObservableProperty] private int _floatingWindowAutoCloseTime = 5;
    [ObservableProperty] private bool _useMainWindowWhenExceedThreshold = true;
    [ObservableProperty] private int _mainWindowDisplayThreshold = 5;
    [ObservableProperty] private int _notificationServiceType = 0;
    [ObservableProperty] private int _notificationDisplayDuration = 5;

    [JsonIgnore]
    public int FloatingWindowTransparencyPercent
    {
        get => (int)Math.Round(FloatingWindowTransparency * 100, MidpointRounding.AwayFromZero);
        set => FloatingWindowTransparency = Math.Clamp(value, 0, 100) / 100.0;
    }

    partial void OnFloatingWindowTransparencyChanged(double value)
    {
        OnPropertyChanged(nameof(FloatingWindowTransparencyPercent));
    }
}
