using CommunityToolkit.Mvvm.ComponentModel;

namespace SecRandom.Models.Config;

/// <summary>
/// 联动设置配置模型
/// </summary>
public partial class LinkageSettingsConfig : ObservableObject
{
    [ObservableProperty] private bool _isClassBreakEnabled;
    [ObservableProperty] private int _preClassEnableTimeSeconds;
    [ObservableProperty] private int _postClassDisableDelaySeconds;
    [ObservableProperty] private bool _isVerificationFlowEnabled = true;

    [ObservableProperty] private bool _isPreClassResetEnabled;
    [ObservableProperty] private int _preClassResetTimeSeconds = 120;

    [ObservableProperty] private bool _isSubjectHistoryFilterEnabled;
    [ObservableProperty] private int _subjectHistoryBreakAssignment = 1;

    [ObservableProperty] private int _dataSourceMode;
    [ObservableProperty] private bool _isHideFloatingWindowOnClassEndEnabled;
}
