using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SecRandom.Core.Attributes;
using SecRandom.Core.Enums;

namespace SecRandom.Views.SettingsPages;

[PageInfo("settings.more", "\uebab", PageLocation.Bottom)]
public partial class MoreSettingsPage : UserControl
{
    public MoreSettingsPage()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
