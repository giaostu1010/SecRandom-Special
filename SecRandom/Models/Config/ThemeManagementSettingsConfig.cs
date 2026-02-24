using System.IO;
using System.Text.Json.Serialization;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SecRandom.Models.Config;

public partial class ThemeManagementSettingsConfig : ObservableObject
{
    [ObservableProperty] private int _mainWindowBackgroundMode = 0;
    [ObservableProperty] private Color _mainWindowBackgroundColor = Color.Parse("#FFFFFF");
    [ObservableProperty] private int _mainWindowBackgroundGradientDirection = 0;
    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(MainWindowBackgroundImageFileName))]
    private string _mainWindowBackgroundImage = "";
    [ObservableProperty] private int _mainWindowBackgroundBrightness = 100;

    [JsonIgnore]
    public string MainWindowBackgroundImageFileName => string.IsNullOrEmpty(MainWindowBackgroundImage) ? "" : Path.GetFileName(MainWindowBackgroundImage);

    [ObservableProperty] private int _settingsWindowBackgroundMode = 0;
    [ObservableProperty] private Color _settingsWindowBackgroundColor = Color.Parse("#FFFFFF");
    [ObservableProperty] private int _settingsWindowBackgroundGradientDirection = 0;
    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(SettingsWindowBackgroundImageFileName))]
    private string _settingsWindowBackgroundImage = "";
    [ObservableProperty] private int _settingsWindowBackgroundBrightness = 100;

    [JsonIgnore]
    public string SettingsWindowBackgroundImageFileName => string.IsNullOrEmpty(SettingsWindowBackgroundImage) ? "" : Path.GetFileName(SettingsWindowBackgroundImage);

    [ObservableProperty] private int _notificationFloatingWindowBackgroundMode = 0;
    [ObservableProperty] private Color _notificationFloatingWindowBackgroundColor = Color.Parse("#FFFFFF");
    [ObservableProperty] private int _notificationFloatingWindowBackgroundGradientDirection = 0;
    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(NotificationFloatingWindowBackgroundImageFileName))]
    private string _notificationFloatingWindowBackgroundImage = "";
    [ObservableProperty] private int _notificationFloatingWindowBackgroundBrightness = 100;

    [JsonIgnore]
    public string NotificationFloatingWindowBackgroundImageFileName => string.IsNullOrEmpty(NotificationFloatingWindowBackgroundImage) ? "" : Path.GetFileName(NotificationFloatingWindowBackgroundImage);
}
