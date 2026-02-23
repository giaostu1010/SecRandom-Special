using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SecRandom.Models.Config;

/// <summary>
/// 抽取设置配置模型
/// </summary>
public partial class DrawSettingsConfig : ObservableObject
{
    [ObservableProperty] private RollCallSettingsConfig _rollCallSettings = new();
    [ObservableProperty] private QuickDrawSettingsConfig _quickDrawSettings = new();
    [ObservableProperty] private LotterySettingsConfig _lotterySettings = new();
    [ObservableProperty] private FaceDetectorSettingsConfig _faceDetectorSettings = new();

    [ObservableProperty] private Dictionary<string, RollCallSettingsOverrideConfig> _rollCallListSpecificOverrides = [];
    [ObservableProperty] private Dictionary<string, QuickDrawSettingsOverrideConfig> _quickDrawListSpecificOverrides = [];
    [ObservableProperty] private Dictionary<string, LotterySettingsOverrideConfig> _lotteryListSpecificOverrides = [];
}
