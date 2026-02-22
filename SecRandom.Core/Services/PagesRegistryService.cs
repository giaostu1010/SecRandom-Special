using System.Collections.ObjectModel;
using SecRandom.Core.Attributes;

namespace SecRandom.Core.Services;

public static class PagesRegistryService
{
    public static ObservableCollection<PageInfo> MainItems { get; } = [];
    public static ObservableCollection<PageInfo> SettingsItems { get; } = [];
}