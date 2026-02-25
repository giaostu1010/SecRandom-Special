using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SecRandom.Core.Attributes;
using SecRandom.Core.Enums;

namespace SecRandom.Views.SettingsPages.HistoryManagementSubPages.TableSubPages;

[PageInfo("settings.history.rollCallTable", "\uE81C", "settings.history", PageLocation.Top, true)]
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
