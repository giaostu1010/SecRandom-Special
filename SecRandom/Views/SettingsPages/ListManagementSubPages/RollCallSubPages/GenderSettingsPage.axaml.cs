using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SecRandom.Core.Attributes;

namespace SecRandom.Views.SettingsPages.ListManagementSubPages.RollCallSubPages;

[PageInfo("settings.listManagement.rollCall.genderSettings", "\uE7C3", "settings.listManagement")]
public partial class GenderSettingsPage : UserControl
{
    public GenderSettingsPage()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
