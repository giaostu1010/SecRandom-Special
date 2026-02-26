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

[PageInfo("settings.listManagement.lottery.nameSettings", "\uE8A1", "settings.listManagement", PageLocation.Top, true)]
public partial class NameSettingsPage : UserControl
{
    private readonly LotteryListService _service;
    private readonly ILogger<NameSettingsPage>? _logger;
    private string? _currentPoolName;
    private List<string> _initialNames = [];
    private bool _saved = false;

    public NameSettingsPage()
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

        // 加载现有名称
        LoadNames();
    }

    private void LoadNames()
    {
        if (string.IsNullOrEmpty(_currentPoolName)) return;

        try
        {
            var prizes = _service.GetPoolList(_currentPoolName);
            _initialNames = prizes.Select(p => p.Name).ToList();

            var textBox = this.FindControl<TextBox>("NamesTextBox");
            if (textBox != null && _initialNames.Count > 0)
            {
                textBox.Text = string.Join("\n", _initialNames);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "加载奖品名称失败");
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

            var textBox = this.FindControl<TextBox>("NamesTextBox");
            if (textBox == null) return;

            var text = textBox.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(text))
            {
                await ShowMessageAsync("错误", "请输入奖品名称");
                return;
            }

            var names = text.Split('\n')
                .Select(n => n.Trim())
                .Where(n => !string.IsNullOrEmpty(n))
                .ToList();

            // 验证名称
            var invalidNames = names.Where(n => !IsValidName(n)).ToList();
            if (invalidNames.Count > 0)
            {
                await ShowMessageAsync("错误", $"以下名称包含非法字符：\n{string.Join("\n", invalidNames)}");
                return;
            }

            // 检查重复名称
            var duplicates = names.GroupBy(n => n)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicates.Count > 0)
            {
                var result = await ShowConfirmAsync("重复名称", 
                    $"发现 {duplicates.Count} 个重复的名称：\n{string.Join("\n", duplicates)}\n\n是否自动重命名？");
                
                if (result == ContentDialogResult.Primary)
                {
                    names = MakeUniqueNames(names);
                    textBox.Text = string.Join("\n", names);
                    return;
                }
                else if (result == ContentDialogResult.Secondary)
                {
                    return;
                }
            }

            // 获取现有奖品数据
            var existingPrizes = _service.GetPoolList(_currentPoolName);
            var existingDict = existingPrizes.ToDictionary(p => p.Name, p => p);

            // 创建新的奖品列表
            var newPrizes = new List<PrizeItem>();
            var id = 1;
            foreach (var name in names)
            {
                if (existingDict.TryGetValue(name, out var existingPrize))
                {
                    // 保留现有奖品的信息
                    newPrizes.Add(existingPrize);
                }
                else
                {
                    // 新增奖品
                    newPrizes.Add(new PrizeItem
                    {
                        Id = id,
                        Name = name,
                        Count = 1,
                        Weight = 1.0,
                        Exist = true,
                        Tags = []
                    });
                }
                id++;
            }

            // 保存
            if (_service.SavePrizes(_currentPoolName, newPrizes))
            {
                _saved = true;
                _initialNames = names;
                await ShowMessageAsync("成功", $"成功保存 {names.Count} 个奖品");
            }
            else
            {
                await ShowMessageAsync("错误", "保存失败");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "保存奖品名称失败");
            await ShowMessageAsync("错误", $"保存失败：{ex.Message}");
        }
    }

    private async void CancelButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var textBox = this.FindControl<TextBox>("NamesTextBox");
        if (textBox == null) return;

        var currentText = textBox.Text?.Trim() ?? string.Empty;
        var currentNames = currentText.Split('\n')
            .Select(n => n.Trim())
            .Where(n => !string.IsNullOrEmpty(n))
            .ToList();

        // 检查是否有未保存的更改
        if (!_saved && !currentNames.SequenceEqual(_initialNames))
        {
            var result = await ShowConfirmAsync("未保存的更改", "您有未保存的更改，是否放弃？");
            if (result != ContentDialogResult.Primary)
            {
                return;
            }
        }

        // 恢复初始值
        if (_initialNames.Count > 0)
        {
            textBox.Text = string.Join("\n", _initialNames);
        }
        else
        {
            textBox.Text = string.Empty;
        }
        _saved = false;
    }

    private static bool IsValidName(string name)
    {
        var invalidChars = new[] { '/', ':', '*', '?', '"', '<', '>', '|' };
        return !name.Any(c => invalidChars.Contains(c)) && 
               !name.Equals("pool", StringComparison.OrdinalIgnoreCase);
    }

    private static List<string> MakeUniqueNames(List<string> names)
    {
        var result = new List<string>();
        var usedNames = new HashSet<string>();
        var counters = new Dictionary<string, int>();

        foreach (var name in names)
        {
            if (!usedNames.Contains(name))
            {
                usedNames.Add(name);
                result.Add(name);
                continue;
            }

            var index = counters.GetValueOrDefault(name, 0) + 1;
            var candidate = $"{name}_{index}";

            while (usedNames.Contains(candidate))
            {
                index++;
                candidate = $"{name}_{index}";
            }

            counters[name] = index;
            usedNames.Add(candidate);
            result.Add(candidate);
        }

        return result;
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
