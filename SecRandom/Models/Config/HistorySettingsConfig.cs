using CommunityToolkit.Mvvm.ComponentModel;

namespace SecRandom.Models.Config;

public partial class HistorySettingsConfig : ObservableObject
{
    [ObservableProperty] private bool _showRollCallHistory = true;
    [ObservableProperty] private bool _showLotteryHistory = true;
    [ObservableProperty] private int _selectClassName = 0;
    [ObservableProperty] private int _selectPoolName = 0;
    [ObservableProperty] private bool _showWeight = true;
}
