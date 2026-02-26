using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using FluentAvalonia.UI.Controls;
using Microsoft.Extensions.Logging;
using SecRandom.Core.Attributes;
using SecRandom.Core.Enums;
using SecRandom.Core.Services;

namespace SecRandom.Views.SettingsPages.ListManagementSubPages.LotterySubPages;

[PageInfo("settings.listManagement.lottery.setPoolName", "\uE8EC", "settings.listManagement", PageLocation.Top, true)]
public partial class SetPoolNamePage : UserControl
{
    private readonly LotteryListService _service;
    private readonly ILogger<SetPoolNamePage>? _logger;
    private List<string> _initialPoolNames = [];

    public SetPoolNamePage()
    {
        InitializeComponent();
        
        // 获取服务（实际项目中应该通过依赖注入）
        _service = new LotteryListService();
        
        // 加载现有奖池名称
        LoadPoolNames();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void LoadPoolNames()
    {
        try
        {
            _service.RefreshPoolNames();
            _initialPoolNames = _service.PoolNames.ToList();
            
            var textBox = this.FindControl<TextBox>("PoolNamesTextBox");
            if (textBox != null && _initialPoolNames.Count > 0)
            {
                textBox.Text = string.Join("\n", _initialPoolNames);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "加载奖池名称失败");
        }
    }

    private async void SaveButton_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            var textBox = this.FindControl<TextBox>("PoolNamesTextBox");
            if (textBox == null) return;

            var text = textBox.Text?.Trim() ?? string.Empty;
            var poolNames = text.Split('\n')
                .Select(n => n.Trim())
                .Where(n => !string.IsNullOrEmpty(n))
                .ToList();

            // 验证奖池名称
            var invalidNames = poolNames.Where(n => !IsValidPoolName(n)).ToList();
            if (invalidNames.Count > 0)
            {
                await ShowMessageAsync("错误", $"以下奖池名称包含非法字符：\n{string.Join("\n", invalidNames)}");
                return;
            }

            // 检查重复名称
            var duplicates = poolNames.GroupBy(n => n)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicates.Count > 0)
            {
                var result = await ShowConfirmAsync("重复名称", 
                    $"发现 {duplicates.Count} 个重复的奖池名称：\n{string.Join("\n", duplicates)}\n\n是否自动重命名？");
                
                if (result == ContentDialogResult.Primary)
                {
                    poolNames = MakeUniqueNames(poolNames);
                    textBox.Text = string.Join("\n", poolNames);
                    return;
                }
                else if (result == ContentDialogResult.Secondary)
                {
                    return;
                }
            }

            // 检查要删除的奖池
            var deletedPools = _initialPoolNames.Except(poolNames).ToList();
            if (deletedPools.Count > 0)
            {
                var result = await ShowConfirmAsync("确认删除", 
                    $"以下 {deletedPools.Count} 个奖池将被删除：\n{string.Join("\n", deletedPools)}\n\n此操作不可撤销，是否继续？");
                
                if (result != ContentDialogResult.Primary)
                {
                    return;
                }

                // 删除奖池
                foreach (var poolName in deletedPools)
                {
                    _service.DeletePool(poolName);
                }
            }

            // 创建新奖池
            var newPools = poolNames.Except(_initialPoolNames).ToList();
            var createdCount = 0;
            foreach (var poolName in newPools)
            {
                if (_service.CreatePool(poolName))
                {
                    createdCount++;
                }
            }

            // 显示结果
            if (createdCount > 0 || deletedPools.Count > 0)
            {
                var message = createdCount > 0 
                    ? $"成功创建 {createdCount} 个奖池" 
                    : "保存成功";
                if (deletedPools.Count > 0)
                {
                    message += $"\n删除了 {deletedPools.Count} 个奖池";
                }
                await ShowMessageAsync("成功", message);
            }
            else
            {
                await ShowMessageAsync("提示", "没有需要保存的更改");
            }

            // 更新初始列表
            _initialPoolNames = poolNames;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "保存奖池名称失败");
            await ShowMessageAsync("错误", $"保存失败：{ex.Message}");
        }
    }

    private async void CancelButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var textBox = this.FindControl<TextBox>("PoolNamesTextBox");
        if (textBox == null) return;

        var currentText = textBox.Text?.Trim() ?? string.Empty;
        var currentNames = currentText.Split('\n')
            .Select(n => n.Trim())
            .Where(n => !string.IsNullOrEmpty(n))
            .ToList();

        // 检查是否有未保存的更改
        if (!currentNames.SequenceEqual(_initialPoolNames))
        {
            var result = await ShowConfirmAsync("未保存的更改", "您有未保存的更改，是否放弃？");
            if (result != ContentDialogResult.Primary)
            {
                return;
            }
        }

        // 恢复初始值
        if (_initialPoolNames.Count > 0)
        {
            textBox.Text = string.Join("\n", _initialPoolNames);
        }
        else
        {
            textBox.Text = string.Empty;
        }
    }

    private static bool IsValidPoolName(string name)
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
