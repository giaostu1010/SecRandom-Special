using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SecRandom.Core.Attributes;
using SecRandom.Core.Abstraction;
using SecRandom.Models.Config;
using SecRandom.Services.Config;

namespace SecRandom.Views.SettingsPages.NotificationSettingsSubPages;

[PageInfo("settings.rollCallNotificationSettings", "\uE7E3", "settings.notification")]
public partial class RollCallNotificationSettingsPage : UserControl
{
    public NotificationSettingsConfig ViewModel { get; } =
        IAppHost.GetService<MainConfigHandler>().Data.NotificationSettings;

    public ObservableCollection<string> MonitorList { get; } = ["Display 1"];

    public RollCallNotificationSettingsPage()
    {
        InitializeComponent();
        DataContext = this;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
