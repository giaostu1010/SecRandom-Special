using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using SecRandom.Core;
using SecRandom.Core.Abstraction;

namespace SecRandom.Models.Config;

public partial class RootConfigModel : ConfigBase
{
    [JsonIgnore]
    public override string ConfigFilePath => Utils.GetFilePath("Config.json");

    [ObservableProperty] private BasicSettingsConfig _basicSettings = new();
    [ObservableProperty] private DrawSettingsConfig _drawSettings = new();
    [ObservableProperty] private FloatingWindowSettingsConfig _floatingWindowSettings = new();
    [ObservableProperty] private LinkageSettingsConfig _linkageSettings = new();
}
