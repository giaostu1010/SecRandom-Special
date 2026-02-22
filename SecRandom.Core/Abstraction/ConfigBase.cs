using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SecRandom.Core.Abstraction;

public abstract class ConfigBase : ObservableObject
{
    [JsonIgnore]
    public abstract string ConfigFilePath { get; }
}