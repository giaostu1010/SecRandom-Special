using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using FluentAvalonia.UI.Controls;
using SecRandom.Core.Abstraction;
using SecRandom.Core.Attributes;
using SecRandom.Core.Services;
using SecRandom.Views;

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
        NavigateToPage("settings.listManagement.lottery.preview");
    }

    private void OpenSetPoolNameWindow_OnClick(object? sender, RoutedEventArgs e)
    {
        NavigateToPage("settings.listManagement.lottery.setPoolName");
    }

    private async void OpenImportPrizeWindow_OnClick(object? sender, RoutedEventArgs e)
    {
        _lotteryListService.RefreshPoolNames();
        if (_lotteryListService.PoolNames.Count == 0)
        {
            await ShowMessageAsync("提示", "请先创建奖池");
            return;
        }
        NavigateToPage("settings.listManagement.lottery.importPrize");
    }

    private async void OpenPrizeSettingsWindow_OnClick(object? sender, RoutedEventArgs e)
    {
        _lotteryListService.RefreshPoolNames();
        if (_lotteryListService.PoolNames.Count == 0)
        {
            await ShowMessageAsync("提示", "请先创建奖池");
            return;
        }
        NavigateToPage("settings.listManagement.lottery.prizeSettings");
    }

    private async void OpenWeightSettingsWindow_OnClick(object? sender, RoutedEventArgs e)
    {
        _lotteryListService.RefreshPoolNames();
        if (_lotteryListService.PoolNames.Count == 0)
        {
            await ShowMessageAsync("提示", "请先创建奖池");
            return;
        }
        NavigateToPage("settings.listManagement.lottery.weightSettings");
    }

    private async void OpenCountSettingsWindow_OnClick(object? sender, RoutedEventArgs e)
    {
        _lotteryListService.RefreshPoolNames();
        if (_lotteryListService.PoolNames.Count == 0)
        {
            await ShowMessageAsync("提示", "请先创建奖池");
            return;
        }
        NavigateToPage("settings.listManagement.lottery.countSettings");
    }

    private async void OpenExportPrizeWindow_OnClick(object? sender, RoutedEventArgs e)
    {
        _lotteryListService.RefreshPoolNames();
        if (_lotteryListService.PoolNames.Count == 0)
        {
            await ShowMessageAsync("提示", "请先创建奖池");
            return;
        }
        NavigateToPage("settings.listManagement.lottery.exportPrize");
    }

    private void NavigateToPage(string pageId)
    {
        var settingsView = this.GetVisualAncestors().OfType<SettingsView>().FirstOrDefault();
        var pageInfo = PagesRegistryService.SettingsItems.FirstOrDefault(x => x.Id == pageId);
        
        if (settingsView is not null && pageInfo is not null)
        {
            settingsView.NavigateToPage(pageInfo, false);
        }
        else if (settingsView is not null)
        {
            // 如果页面未注册，创建一个临时的 PageInfo
            var icon = pageId switch
            {
                "settings.listManagement.lottery.setPoolName" => "\uE8EC",
                "settings.listManagement.lottery.importPrize" => "\uE8E5",
                "settings.listManagement.lottery.prizeSettings" => "\uE8A1",
                "settings.listManagement.lottery.weightSettings" => "\uE8A1",
                "settings.listManagement.lottery.countSettings" => "\uE8A1",
                "settings.listManagement.lottery.exportPrize" => "\uEDE1",
                _ => "\uE8A1"
            };
            settingsView.NavigateToPage(
                new PageInfo(pageId, icon, "settings.listManagement"),
                false);
        }
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
