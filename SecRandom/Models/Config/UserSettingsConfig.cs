using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SecRandom.Models.Config;

public partial class UserSettingsConfig : ObservableObject
{
    [ObservableProperty] private string _userId = Guid.NewGuid().ToString();
    [ObservableProperty] private string _firstUseTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    [ObservableProperty] private int _totalRuntimeSeconds = 0;
    [ObservableProperty] private int _totalDrawCount = 0;
    [ObservableProperty] private int _rollCallTotalCount = 0;
    [ObservableProperty] private int _lotteryTotalCount = 0;
}
