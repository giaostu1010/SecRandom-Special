using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SecRandom.Core.Attributes;
using SecRandom.Core.Enums;

namespace SecRandom.Views.SettingsPages;

[PageInfo("settings.update", "\ue160", PageLocation.Bottom)]
public partial class UpdateSettingsPage : UserControl
{
    public UpdateSettingsPage()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
