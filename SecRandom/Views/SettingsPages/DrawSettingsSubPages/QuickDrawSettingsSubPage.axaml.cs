using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SecRandom.Core.Abstraction;
using SecRandom.Core.Attributes;
using SecRandom.Services.Config;

namespace SecRandom.Views.SettingsPages;

[PageInfo("settings.draw.quickDraw", "\ue07c")]
public partial class QuickDrawSettingsSubPage : UserControl
{
    public QuickDrawSettingsSubPage()
    {
        DataContext = IAppHost.GetService<MainConfigHandler>().Data.DrawSettings.QuickDrawSettings;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
