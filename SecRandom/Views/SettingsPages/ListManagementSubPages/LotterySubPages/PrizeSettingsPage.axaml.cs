using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using FluentAvalonia.UI.Controls;
using SecRandom.Core.Abstraction;
using SecRandom.Core.Attributes;
using SecRandom.Core.Enums;
using SecRandom.Core.Models;
using SecRandom.Core.Services;
using Res = SecRandom.Langs.SettingsPages.ListManagementPage.Resources;

namespace SecRandom.Views.SettingsPages.ListManagementSubPages.LotterySubPages;

[PageInfo("settings.listManagement.lottery.prizeSettings", "\uE8A1", "settings.listManagement", PageLocation.Top, true)]
public partial class PrizeSettingsPage : UserControl
{
    private readonly LotteryListService _lotteryListService;
    public ObservableCollection<PrizeItem> Prizes { get; } = [];

    public PrizeSettingsPage()
    {
        _lotteryListService = IAppHost.GetService<LotteryListService>();
        InitializeComponent();
        LoadPoolNames();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void LoadPoolNames()
    {
        _lotteryListService.RefreshPoolNames();
        var comboBox = this.FindControl<ComboBox>("PoolNameComboBox");
        if (comboBox != null)
        {
            comboBox.ItemsSource = _lotteryListService.PoolNames;
            if (_lotteryListService.PoolNames.Count > 0)
            {
                comboBox.SelectedIndex = 0;
            }
        }
    }

    private void PoolNameComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var comboBox = this.FindControl<ComboBox>("PoolNameComboBox");
        if (comboBox?.SelectedItem is string poolName)
        {
            _lotteryListService.CurrentPoolName = poolName;
            LoadPrizes(poolName);
        }
    }

    private void LoadPrizes(string poolName)
    {
        var prizes = _lotteryListService.GetPoolList(poolName);
        Prizes.Clear();
        foreach (var prize in prizes)
        {
            Prizes.Add(prize);
        }
        
        var dataGrid = this.FindControl<DataGrid>("PrizeDataGrid");
        if (dataGrid != null)
        {
            dataGrid.ItemsSource = Prizes;
        }
    }

    private async void AddPrizeButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var dialog = new ContentDialog
        {
            Title = Res.AddPrize,
            PrimaryButtonText = Res.Confirm,
            CloseButtonText = Res.Cancel
        };

        var stackPanel = new StackPanel { Spacing = 8 };
        var nameTextBox = new TextBox { Watermark = Res.EnterPrizeName };
        var weightTextBox = new TextBox { Watermark = Res.EnterWeight, Text = "1" };
        var countTextBox = new TextBox { Watermark = Res.EnterCount, Text = "1" };
        
        stackPanel.Children.Add(new TextBlock { Text = Res.NameLabel });
        stackPanel.Children.Add(nameTextBox);
        stackPanel.Children.Add(new TextBlock { Text = Res.WeightLabel });
        stackPanel.Children.Add(weightTextBox);
        stackPanel.Children.Add(new TextBlock { Text = Res.CountLabel });
        stackPanel.Children.Add(countTextBox);
        
        dialog.Content = stackPanel;

        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary && !string.IsNullOrWhiteSpace(nameTextBox.Text))
        {
            var prize = new PrizeItem
            {
                Name = nameTextBox.Text.Trim(),
                Weight = double.TryParse(weightTextBox.Text, out var w) ? w : 1.0,
                Count = int.TryParse(countTextBox.Text, out var c) ? c : 1,
                Exist = true
            };
            
            // 计算新ID
            prize.Id = Prizes.Count > 0 ? Prizes.Max(p => p.Id) + 1 : 1;
            Prizes.Add(prize);
        }
    }

    private async void DeletePrizeButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var dataGrid = this.FindControl<DataGrid>("PrizeDataGrid");
        if (dataGrid?.SelectedItem is PrizeItem selectedPrize)
        {
            var dialog = new ContentDialog
            {
                Title = Res.ConfirmDelete,
                Content = string.Format(Res.ConfirmDeletePrize, selectedPrize.Name),
                PrimaryButtonText = Res.Delete,
                CloseButtonText = Res.Cancel
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                Prizes.Remove(selectedPrize);
            }
        }
        else
        {
            await ShowMessageAsync(Res.Tip, Res.PleaseSelectPrizeToDelete);
        }
    }

    private async void SaveButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var comboBox = this.FindControl<ComboBox>("PoolNameComboBox");
        var poolName = comboBox?.SelectedItem as string;
        
        if (string.IsNullOrEmpty(poolName))
        {
            await ShowMessageAsync(Res.Error, Res.PleaseSelectPool);
            return;
        }
        
        if (_lotteryListService.SavePrizes(poolName, Prizes.ToList()))
        {
            await ShowMessageAsync(Res.Success, Res.PrizeSettingsSaved);
        }
        else
        {
            await ShowMessageAsync(Res.Failed, Res.SaveFailed);
        }
    }

    private static async Task ShowMessageAsync(string title, string message)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            CloseButtonText = Res.Confirm
        };
        await dialog.ShowAsync();
    }
}
