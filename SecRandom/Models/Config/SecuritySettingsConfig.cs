using CommunityToolkit.Mvvm.ComponentModel;

namespace SecRandom.Models.Config;

/// <summary>
/// 安全设置配置模型
/// </summary>
public partial class SecuritySettingsConfig : ObservableObject
{
    [ObservableProperty] private bool _safetySwitch = false;
    [ObservableProperty] private bool _totpSwitch = false;
    [ObservableProperty] private bool _usbSwitch = false;
    [ObservableProperty] private int _verificationProcess = 0;
    [ObservableProperty] private bool _showHideFloatingWindowSwitch = false;
    [ObservableProperty] private bool _restartSwitch = false;
    [ObservableProperty] private bool _exitSwitch = false;
    [ObservableProperty] private bool _openSettingsSwitch = false;
    [ObservableProperty] private bool _previewSettingsSwitch = false;

    public bool IsSafetyEnabled => SafetySwitch;
}
