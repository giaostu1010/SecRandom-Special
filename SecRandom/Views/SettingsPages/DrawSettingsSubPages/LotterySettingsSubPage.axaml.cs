using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SecRandom.Core.Abstraction;
using SecRandom.Core.Attributes;
using SecRandom.Services.Config;

namespace SecRandom.Views.SettingsPages;

[PageInfo("settings.draw.lottery", "\ue07c")]
public partial class LotterySettingsSubPage : UserControl
{
    public LotterySettingsSubPage()
    {
        DataContext = IAppHost.GetService<RootConfigHandler>().Data.DrawSettings.LotterySettings;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
