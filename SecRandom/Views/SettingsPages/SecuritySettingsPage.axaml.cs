using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SecRandom.Core.Attributes;

namespace SecRandom.Views.SettingsPages;

[PageInfo("settings.securitySettings", "\uef4e")]
public partial class SecuritySettingsPage : UserControl
{
    public SecuritySettingsPage()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
