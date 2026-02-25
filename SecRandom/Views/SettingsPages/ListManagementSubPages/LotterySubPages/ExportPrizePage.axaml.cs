using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using FluentAvalonia.UI.Controls;
using SecRandom.Core.Abstraction;
using SecRandom.Core.Attributes;
using SecRandom.Core.Enums;
using SecRandom.Core.Services;
using Res = SecRandom.Langs.SettingsPages.ListManagementPage.Resources;

namespace SecRandom.Views.SettingsPages.ListManagementSubPages.LotterySubPages;

[PageInfo("settings.listManagement.lottery.exportPrize", "\uEDE1", "settings.listManagement", PageLocation.Top, true)]
public partial class ExportPrizePage : UserControl
{
    private readonly LotteryListService _lotteryListService;
    private string? _saveFilePath;

    public ExportPrizePage()
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

    private async void BrowseButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;
        var storageProvider = topLevel.StorageProvider;
        
        var jsonRadio = this.FindControl<RadioButton>("JsonRadioButton");
        var csvRadio = this.FindControl<RadioButton>("CsvRadioButton");
        
        var defaultExt = "json";
        if (csvRadio?.IsChecked == true) defaultExt = "csv";
        else if (this.FindControl<RadioButton>("TxtRadioButton")?.IsChecked == true) defaultExt = "txt";
        
        var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = Res.SaveLocation,
            SuggestedFileName = "prizes",
            DefaultExtension = defaultExt,
            FileTypeChoices = new[]
            {
                new FilePickerFileType(Res.JsonFormat) { Patterns = new[] { "*.json" } },
                new FilePickerFileType(Res.CsvFormat) { Patterns = new[] { "*.csv" } },
                new FilePickerFileType(Res.TxtFormat) { Patterns = new[] { "*.txt" } }
            }
        });

        if (file != null)
        {
            _saveFilePath = file.Path.LocalPath;
            var filePathTextBox = this.FindControl<TextBox>("FilePathTextBox");
            if (filePathTextBox != null)
            {
                filePathTextBox.Text = _saveFilePath;
            }
            
            var exportButton = this.FindControl<Button>("ExportButton");
            if (exportButton != null)
            {
                exportButton.IsEnabled = true;
            }
        }
    }

    private async void ExportButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var comboBox = this.FindControl<ComboBox>("PoolNameComboBox");
        var poolName = comboBox?.SelectedItem as string;
        
        if (string.IsNullOrEmpty(poolName))
        {
            await ShowMessageAsync(Res.Error, Res.PleaseSelectPool);
            return;
        }
        
        if (string.IsNullOrEmpty(_saveFilePath))
        {
            await ShowMessageAsync(Res.Error, Res.PleaseSelectSaveLocation);
            return;
        }

        if (_lotteryListService.ExportPrizes(poolName, _saveFilePath))
        {
            await ShowMessageAsync(Res.Success, string.Format(Res.ExportSuccess, _saveFilePath));
            
            // 重置状态
            _saveFilePath = null;
            var filePathTextBox = this.FindControl<TextBox>("FilePathTextBox");
            if (filePathTextBox != null)
            {
                filePathTextBox.Text = string.Empty;
            }
            
            var exportButton = this.FindControl<Button>("ExportButton");
            if (exportButton != null)
            {
                exportButton.IsEnabled = false;
            }
        }
        else
        {
            await ShowMessageAsync(Res.Failed, Res.ExportFailed);
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
