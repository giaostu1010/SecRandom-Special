using CommunityToolkit.Mvvm.ComponentModel;

namespace SecRandom.Models.Config;

public partial class FloatPositionConfig : ObservableObject
{
    [ObservableProperty] private int _x = 100;
    [ObservableProperty] private int _y = 100;
}