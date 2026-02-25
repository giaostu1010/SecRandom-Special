using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using SecRandom.Core.Abstraction;
using SecRandom.Core.Attributes;
using SecRandom.Core.Services;

namespace SecRandom.Views.SettingsPages.HistoryManagementSubPages;

[PageInfo("settings.history.rollCall", "\uE77B", "settings.history")]
public partial class RollCallHistorySettingsSubPage : UserControl
{
    public RollCallHistorySettingsSubPage()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void ViewRollCallHistory_OnClick(object? sender, RoutedEventArgs e)
    {
        var settingsView = this.GetVisualAncestors().OfType<SettingsView>().FirstOrDefault();
        if (settingsView is not null)
        {
            var pageInfo = new PageInfo("settings.history.rollCallTable", "\uE81C", "settings.history")
            {
                Name = "点名历史记录表格"
            };
            settingsView.NavigateToPage(pageInfo, false);
        }
    }

    private void ClearRollCallHistory_OnClick(object? sender, RoutedEventArgs e)
    {
    }
}
