using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SecRandom.Core.Attributes;

namespace SecRandom.Views.SettingsPages;

[PageInfo("settings.notificationSettings", "\ue558")]
public partial class NotificationSettingsPage : UserControl
{
    public NotificationSettingsPage()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
