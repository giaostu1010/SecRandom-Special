using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using SecRandom.Core.Abstraction;
using SecRandom.Core.Attributes;
using SecRandom.Core.Services;

namespace SecRandom.Views.SettingsPages.HistoryManagementSubPages;

[PageInfo("settings.history.lottery", "\uE8F1", "settings.history")]
public partial class LotteryHistorySettingsSubPage : UserControl
{
    public LotteryHistorySettingsSubPage()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void ViewLotteryHistory_OnClick(object? sender, RoutedEventArgs e)
    {
        var settingsView = this.GetVisualAncestors().OfType<SettingsView>().FirstOrDefault();
        if (settingsView is not null)
        {
            var pageInfo = new PageInfo("settings.history.lotteryTable", "\uE8F1", "settings.history")
            {
                Name = "抽奖历史记录表格"
            };
            settingsView.NavigateToPage(pageInfo, false);
        }
    }

    private void ClearLotteryHistory_OnClick(object? sender, RoutedEventArgs e)
    {
    }
}
