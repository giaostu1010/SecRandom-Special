using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SecRandom.Core.Attributes;

namespace SecRandom.Views.SettingsPages.ListManagementSubPages.RollCallSubPages;

[PageInfo("settings.listManagement.rollCall.nameSettings", "\uE8A1", "settings.listManagement")]
public partial class NameSettingsPage : UserControl
{
    public NameSettingsPage()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
