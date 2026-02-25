using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using SecRandom.Core;
using SecRandom.Core.Attributes;
using SecRandom.Core.Enums;
using SecRandom.ViewModels;

namespace SecRandom.Views.SettingsPages.ListManagementSubPages.TablePreview;

public class PrizeDisplayItem
{
    public string ExistText { get; set; } = "";
    public string Serial { get; set; } = "";
    public string Prize { get; set; } = "";
    public string Weight { get; set; } = "";
    public string Count { get; set; } = "";
}

[PageInfo("settings.listManagement.lottery.preview", "\uE8A1", "settings.listManagement", PageLocation.Top, true)]
public partial class LotteryTablePreviewPage : UserControl
{
    private readonly ListNamesSource _listNamesSource;
    private readonly ObservableCollection<PrizeDisplayItem> _prizes = [];

    public LotteryTablePreviewPage()
    {
        _listNamesSource = new ListNamesSource(Utils.GetFilePath("list", "lottery_list"));
        _listNamesSource.PropertyChanged += ListNamesSource_OnPropertyChanged;
        
        InitializeComponent();
        
        Loaded += LotteryTablePreviewPage_Loaded;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void LotteryTablePreviewPage_Loaded(object? sender, RoutedEventArgs e)
    {
        var poolComboBox = this.FindControl<ComboBox>("PoolComboBox");
        if (poolComboBox is not null)
        {
            poolComboBox.ItemsSource = _listNamesSource.Names;
        }
    }

    private void ListNamesSource_OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(ListNamesSource.Names))
        {
            return;
        }

        var poolComboBox = this.FindControl<ComboBox>("PoolComboBox");
        if (poolComboBox is not null)
        {
            poolComboBox.ItemsSource = _listNamesSource.Names;
        }
    }

    private void PoolComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is not ComboBox comboBox || comboBox.SelectedItem is not string poolName)
        {
            return;
        }

        LoadPrizes(poolName);
    }

    private void LoadPrizes(string poolName)
    {
        _prizes.Clear();
        
        if (string.IsNullOrWhiteSpace(poolName))
        {
            var previewBorder = this.FindControl<Border>("PreviewBorder");
            if (previewBorder is not null)
            {
                previewBorder.IsVisible = false;
            }
            return;
        }

        var filePath = Utils.GetFilePath("list", "lottery_list", $"{poolName}.json");
        if (!File.Exists(filePath))
        {
            var previewBorder = this.FindControl<Border>("PreviewBorder");
            if (previewBorder is not null)
            {
                previewBorder.IsVisible = false;
            }
            return;
        }

        try
        {
            var jsonContent = File.ReadAllText(filePath);
            var prizeData = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(jsonContent);
            
            if (prizeData is not null)
            {
                int serial = 1;
                foreach (var kvp in prizeData)
                {
                    var prize = kvp.Value;
                    _prizes.Add(new PrizeDisplayItem
                    {
                        ExistText = prize.TryGetValue("exist", out var exist) && exist is bool b && b ? "✓" : "",
                        Serial = (serial++).ToString(),
                        Prize = kvp.Key,
                        Weight = prize.TryGetValue("weight", out var weight) ? weight?.ToString() ?? "1" : "1",
                        Count = prize.TryGetValue("count", out var count) ? count?.ToString() ?? "1" : "1"
                    });
                }
            }
            
            var previewBorder = this.FindControl<Border>("PreviewBorder");
            if (previewBorder is not null)
            {
                previewBorder.IsVisible = _prizes.Count > 0;
            }
            
            var prizesList = this.FindControl<ItemsControl>("PrizesList");
            if (prizesList is not null)
            {
                prizesList.ItemsSource = _prizes;
            }
        }
        catch
        {
            var previewBorder = this.FindControl<Border>("PreviewBorder");
            if (previewBorder is not null)
            {
                previewBorder.IsVisible = false;
            }
        }
    }
}
