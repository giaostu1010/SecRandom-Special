using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SecRandom.Core.Attributes;

namespace SecRandom.Views.SettingsPages.HistoryManagementSubPages.TableSubPages;

[PageInfo("settings.history.rollCallTable", "\uE81C", "settings.history")]
public partial class RollCallHistoryTableSubPage : UserControl
{
    public RollCallHistoryTableSubPage()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
