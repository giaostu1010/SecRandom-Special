using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SecRandom.Core.Attributes;

namespace SecRandom.Views.SettingsPages;

[PageInfo("settings.rosterManagement", "\ueabf")]
public partial class RosterManagementPage : UserControl
{
    public RosterManagementPage()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
