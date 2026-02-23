using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SecRandom.Core.Attributes;

namespace SecRandom.Views.SettingsPages;

[PageInfo("settings.history", "\ue98f")]
public partial class HistoryPage : UserControl
{
    public HistoryPage()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
