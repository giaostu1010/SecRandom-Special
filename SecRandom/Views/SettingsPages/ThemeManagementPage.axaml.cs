using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SecRandom.Core.Attributes;

namespace SecRandom.Views.SettingsPages;

[PageInfo("settings.themeManagement", "\uec49")]
public partial class ThemeManagementPage : UserControl
{
    public ThemeManagementPage()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
