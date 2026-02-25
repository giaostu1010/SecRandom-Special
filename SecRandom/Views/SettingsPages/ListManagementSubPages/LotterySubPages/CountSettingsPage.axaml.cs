using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using FluentAvalonia.UI.Controls;
using SecRandom.Core.Abstraction;
using SecRandom.Core.Models;
using SecRandom.Core.Services;
using Res = SecRandom.Langs.SettingsPages.ListManagementPage.Resources;

namespace SecRandom.Views.SettingsPages.ListManagementSubPages.LotterySubPages;

public partial class CountSettingsPage : UserControl
{
    private readonly LotteryListService _lotteryListService;
    public ObservableCollection<PrizeItem> Prizes { get; } = [];

    public CountSettingsPage()
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
        
        var dataGrid = this.FindControl<DataGrid>("CountDataGrid");
        if (dataGrid != null)
        {
            dataGrid.ItemsSource = Prizes;
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
            await ShowMessageAsync(Res.Success, Res.CountSettingsSaved);
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
