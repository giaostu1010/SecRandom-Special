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
        SettingsView.Current?.SelectNavigationItemById("settings.history.lotteryTable");
    }

    private void ClearLotteryHistory_OnClick(object? sender, RoutedEventArgs e)
    {
    }
}
