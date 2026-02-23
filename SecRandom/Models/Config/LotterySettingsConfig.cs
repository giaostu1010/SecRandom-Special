using CommunityToolkit.Mvvm.ComponentModel;

namespace SecRandom.Models.Config;

/// <summary>
/// 抽奖设置配置模型
/// </summary>
public partial class LotterySettingsConfig : ObservableObject
{
    [ObservableProperty] private int _drawMode = 0;
    [ObservableProperty] private int _clearRecord = 0;
    [ObservableProperty] private int _halfRepeat = 1;
    [ObservableProperty] private int _drawType = 0;
    [ObservableProperty] private string _defaultPool = string.Empty;
    [ObservableProperty] private int _fontSize = 48;
    [ObservableProperty] private int _useGlobalFont = 0;
    [ObservableProperty] private string _customFont = string.Empty;
    [ObservableProperty] private int _displayFormat = 0;
    [ObservableProperty] private int _displayStyle = 0;
    [ObservableProperty] private int _showRandom = 0;
    [ObservableProperty] private bool _showTags = false;
    [ObservableProperty] private int _animation = 1;
    [ObservableProperty] private int _animationInterval = 80;
    [ObservableProperty] private int _autoplayCount = 10;
    [ObservableProperty] private bool _resultFlowAnimationStyle = true;
    [ObservableProperty] private int _resultFlowAnimationDuration = 300;
    [ObservableProperty] private int _animationColorTheme = 1;
    [ObservableProperty] private string _animationFixedColor = "#FFFFFF";
    [ObservableProperty] private bool _lotteryImage = false;
    [ObservableProperty] private int _lotteryImagePosition = 0;
}
