using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SecRandom.Core.Abstraction;
using SecRandom.Core.Attributes;
using SecRandom.Services.Config;

namespace SecRandom.Views.SettingsPages;

[PageInfo("settings.draw.rollCall", "\ue07c")]
public partial class RollCallSettingsSubPage : UserControl
{
    public RollCallSettingsSubPage()
    {
        DataContext = IAppHost.GetService<MainConfigHandler>().Data.DrawSettings.RollCallSettings;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
