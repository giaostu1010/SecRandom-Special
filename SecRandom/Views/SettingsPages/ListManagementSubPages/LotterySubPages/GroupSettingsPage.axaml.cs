using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using FluentAvalonia.UI.Controls;
using Microsoft.Extensions.Logging;
using SecRandom.Core.Attributes;
using SecRandom.Core.Enums;
using SecRandom.Core.Models;
using SecRandom.Core.Services;

namespace SecRandom.Views.SettingsPages.ListManagementSubPages.LotterySubPages;

[PageInfo("settings.listManagement.lottery.groupSettings", "\uE902", "settings.listManagement", PageLocation.Top, true)]
public partial class GroupSettingsPage : UserControl
{
    private readonly LotteryListService _service;
    private readonly ILogger<GroupSettingsPage>? _logger;
    private string? _currentPoolName;
    private ObservableCollection<PrizeItem> _prizes = [];
    private bool _saved = false;

    public GroupSettingsPage()
    {
        InitializeComponent();
        
        // 获取服务
        _service = new LotteryListService();
        
        // 初始化
        InitializePage();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void InitializePage()
    {
        // 获取当前选中的奖池
        _service.RefreshPoolNames();
        _currentPoolName = _service.CurrentPoolName;
        
        var currentPoolTextBlock = this.FindControl<TextBlock>("CurrentPoolTextBlock");
        if (currentPoolTextBlock != null)
        {
            currentPoolTextBlock.Text = string.IsNullOrEmpty(_currentPoolName) 
                ? "当前奖池：未选择" 
                : $"当前奖池：{_currentPoolName}";
        }

        // 加载奖品列表
        LoadPrizes();
    }

    private void LoadPrizes()
    {
        if (string.IsNullOrEmpty(_currentPoolName)) return;

        try
        {
            var prizes = _service.GetPoolList(_currentPoolName);
            _prizes.Clear();
            foreach (var prize in prizes)
            {
                _prizes.Add(prize);
            }

            var dataGrid = this.FindControl<DataGrid>("PrizesDataGrid");
            if (dataGrid != null)
            {
                dataGrid.ItemsSource = _prizes;
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "加载奖品列表失败");
        }
    }

    private async void SaveButton_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(_currentPoolName))
            {
                await ShowMessageAsync("错误", "请先选择奖池");
                return;
            }

            if (_prizes.Count == 0)
            {
                await ShowMessageAsync("提示", "没有奖品数据需要保存");
                return;
            }

            // 保存奖品数据
            if (_service.SavePrizes(_currentPoolName, _prizes.ToList()))
            {
                _saved = true;
                await ShowMessageAsync("成功", $"成功保存 {_prizes.Count} 个奖品的分组信息");
            }
            else
            {
                await ShowMessageAsync("错误", "保存失败");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "保存分组信息失败");
            await ShowMessageAsync("错误", $"保存失败：{ex.Message}");
        }
    }

    private async void CancelButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (!_saved)
        {
            var result = await ShowConfirmAsync("未保存的更改", "您有未保存的更改，是否放弃？");
            if (result != ContentDialogResult.Primary)
            {
                return;
            }
        }

        // 重新加载数据
        LoadPrizes();
        _saved = false;
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

    private static async Task<ContentDialogResult> ShowConfirmAsync(string title, string message)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            PrimaryButtonText = "确定",
            SecondaryButtonText = "取消"
        };
        return await dialog.ShowAsync();
    }
}
