using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using FluentAvalonia.UI.Controls;
using Microsoft.Extensions.Logging;
using SecRandom.Core.Attributes;
using SecRandom.Core.Enums;
using SecRandom.Core.Models;
using SecRandom.Core.Services;

namespace SecRandom.Views.SettingsPages.ListManagementSubPages.LotterySubPages;

[PageInfo("settings.listManagement.lottery.exportPrize", "\uEDE1", "settings.listManagement", PageLocation.Top, true)]
public partial class ExportPrizePage : UserControl
{
    private readonly LotteryListService _service;
    private readonly ILogger<ExportPrizePage>? _logger;
    private string? _currentPoolName;
    private List<PrizeItem> _prizes = new();

    public ExportPrizePage()
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
        if (string.IsNullOrEmpty(_currentPoolName))
        {
            UpdatePrizeCount(0);
            return;
        }

        try
        {
            _prizes = _service.GetPrizeList(_currentPoolName);
            
            // 更新奖品数量
            UpdatePrizeCount(_prizes.Count);

            // 更新预览
            var previewDataGrid = this.FindControl<DataGrid>("PreviewDataGrid");
            if (previewDataGrid != null)
            {
                var previewItems = _prizes.Select(p => new ExportPreviewItem
                {
                    Id = p.Id,
                    Name = p.Name,
                    Count = p.Count,
                    Weight = p.Weight,
                    TagsDisplay = string.Join(", ", p.Tags)
                }).ToList();
                
                previewDataGrid.ItemsSource = previewItems;
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "加载奖品列表失败");
            UpdatePrizeCount(0);
        }
    }

    private void UpdatePrizeCount(int count)
    {
        var prizeCountTextBlock = this.FindControl<TextBlock>("PrizeCountTextBlock");
        if (prizeCountTextBlock != null)
        {
            prizeCountTextBlock.Text = count.ToString();
        }
    }

    private void RefreshButton_OnClick(object? sender, RoutedEventArgs e)
    {
        LoadPrizes();
    }

    private async void ExportButton_OnClick(object? sender, RoutedEventArgs e)
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
                await ShowMessageAsync("提示", "当前奖池没有奖品数据");
                return;
            }

            // 获取选择的格式
            var formatComboBox = this.FindControl<ComboBox>("FormatComboBox");
            if (formatComboBox == null) return;

            var selectedIndex = formatComboBox.SelectedIndex;
            var (extension, filterName) = selectedIndex switch
            {
                0 => (".json", "JSON 文件"),
                1 => (".csv", "CSV 文件"),
                2 => (".txt", "TXT 文件"),
                _ => (".json", "JSON 文件")
            };

            // 打开保存文件对话框
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel == null) return;

            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "导出奖品名单",
                SuggestedFileName = $"{_currentPoolName}_奖品名单-SecRandom{extension}",
                FileTypeChoices = new[]
                {
                    new FilePickerFileType(filterName) { Patterns = new[] { $"*{extension}" } }
                }
            });

            if (file == null) return;

            var filePath = file.Path.LocalPath;

            // 导出数据
            var success = selectedIndex switch
            {
                0 => ExportToJson(filePath),
                1 => ExportToCsv(filePath),
                2 => ExportToTxt(filePath),
                _ => ExportToJson(filePath)
            };

            if (success)
            {
                await ShowMessageAsync("成功", $"成功导出 {_prizes.Count} 个奖品到：\n{filePath}");
            }
            else
            {
                await ShowMessageAsync("错误", "导出失败");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "导出奖品名单失败");
            await ShowMessageAsync("错误", $"导出失败：{ex.Message}");
        }
    }

    private bool ExportToJson(string filePath)
    {
        try
        {
            var data = _prizes.ToDictionary(
                p => p.Name,
                p => new
                {
                    id = p.Id,
                    count = p.Count,
                    weight = p.Weight,
                    exist = p.Exist,
                    tags = p.Tags
                });

            var json = System.Text.Json.JsonSerializer.Serialize(data, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
            
            File.WriteAllText(filePath, json);
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "导出JSON失败");
            return false;
        }
    }

    private bool ExportToCsv(string filePath)
    {
        try
        {
            using var writer = new StreamWriter(filePath, false, Encoding.UTF8);
            writer.WriteLine("编号,名称,数量,权重,标签");

            foreach (var prize in _prizes)
            {
                var tags = string.Join(",", prize.Tags);
                writer.WriteLine($"{prize.Id},{prize.Name},{prize.Count},{prize.Weight},{tags}");
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "导出CSV失败");
            return false;
        }
    }

    private bool ExportToTxt(string filePath)
    {
        try
        {
            using var writer = new StreamWriter(filePath, false, Encoding.UTF8);

            foreach (var prize in _prizes)
            {
                writer.WriteLine(prize.Name);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "导出TXT失败");
            return false;
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

/// <summary>
/// 导出预览项
/// </summary>
public class ExportPreviewItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Weight { get; set; }
    public string TagsDisplay { get; set; } = string.Empty;
}
