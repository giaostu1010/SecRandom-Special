using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using FluentAvalonia.UI.Controls;
using SecRandom.Core.Abstraction;
using SecRandom.Core.Attributes;
using SecRandom.Core.Services;

namespace SecRandom.Views.SettingsPages.ListManagementSubPages;

[PageInfo("settings.listManagement.lottery", "\uE8F1", "settings.listManagement")]
public partial class LotteryListSettingsSubPage : UserControl
{
    private readonly LotteryListService _lotteryListService;

    public LotteryListSettingsSubPage()
    {
        _lotteryListService = IAppHost.GetService<LotteryListService>();
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OpenPreviewTable_OnClick(object? sender, RoutedEventArgs e)
    {
        SettingsView.Current?.SelectNavigationItemById("settings.listManagement.lottery.preview");
    }

    private void OpenSetPoolNameWindow_OnClick(object? sender, RoutedEventArgs e)
    {
        SettingsView.Current?.SelectNavigationItemById("settings.listManagement.lottery.setPoolName");
    }

    private async void OpenImportPrizeWindow_OnClick(object? sender, RoutedEventArgs e)
    {
        _lotteryListService.RefreshPoolNames();
        if (_lotteryListService.PoolNames.Count == 0)
        {
            await ShowMessageAsync("提示", "请先创建奖池");
            return;
        }
        SettingsView.Current?.SelectNavigationItemById("settings.listManagement.lottery.importPrize");
    }

    private async void OpenPrizeSettingsWindow_OnClick(object? sender, RoutedEventArgs e)
    {
        _lotteryListService.RefreshPoolNames();
        if (_lotteryListService.PoolNames.Count == 0)
        {
            await ShowMessageAsync("提示", "请先创建奖池");
            return;
        }
        SettingsView.Current?.SelectNavigationItemById("settings.listManagement.lottery.prizeSettings");
    }

    private async void OpenWeightSettingsWindow_OnClick(object? sender, RoutedEventArgs e)
    {
        _lotteryListService.RefreshPoolNames();
        if (_lotteryListService.PoolNames.Count == 0)
        {
            await ShowMessageAsync("提示", "请先创建奖池");
            return;
        }
        SettingsView.Current?.SelectNavigationItemById("settings.listManagement.lottery.weightSettings");
    }

    private async void OpenCountSettingsWindow_OnClick(object? sender, RoutedEventArgs e)
    {
        _lotteryListService.RefreshPoolNames();
        if (_lotteryListService.PoolNames.Count == 0)
        {
            await ShowMessageAsync("提示", "请先创建奖池");
            return;
        }
        SettingsView.Current?.SelectNavigationItemById("settings.listManagement.lottery.countSettings");
    }

    private async void OpenExportPrizeWindow_OnClick(object? sender, RoutedEventArgs e)
    {
        _lotteryListService.RefreshPoolNames();
        if (_lotteryListService.PoolNames.Count == 0)
        {
            await ShowMessageAsync("提示", "请先创建奖池");
            return;
        }
        SettingsView.Current?.SelectNavigationItemById("settings.listManagement.lottery.exportPrize");
    }

    private static async Task ShowMessageAsync(string title, string message)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            CloseButtonText = "确定"
        };
        await dialog.ShowAsync();
    }
}
