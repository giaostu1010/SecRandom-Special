using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using SecRandom.Core;
using SecRandom.Core.Abstraction;
using SecRandom.Models.Config;

namespace SecRandom.Models;

public partial class MainConfigModel : ConfigBase
{
    [JsonIgnore]
    public override string ConfigFilePath => Utils.GetFilePath("Config.json");

    [ObservableProperty] private FloatPositionConfig _floatPosition = new();
    
    [ObservableProperty] private BasicSettingsConfig _basicSettings = new();
    [ObservableProperty] private DrawSettingsConfig _drawSettings = new();
    [ObservableProperty] private FloatingWindowSettingsConfig _floatingWindowSettings = new();
    [ObservableProperty] private LinkageSettingsConfig _linkageSettings = new();
    [ObservableProperty] private SecuritySettingsConfig _securitySettings = new();
    
    [ObservableProperty] private UserSettingsConfig _userSettings = new();
}
