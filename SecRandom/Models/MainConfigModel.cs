using CommunityToolkit.Mvvm.ComponentModel;
using SecRandom.Core;
using SecRandom.Core.Abstraction;

namespace SecRandom.Models;

public partial class MainConfigModel : ConfigBase
{
    public override string ConfigFilePath => Utils.GetFilePath("Config.json");
    
    [ObservableProperty] private string _test = "hello";
}