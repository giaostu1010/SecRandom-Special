using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using FluentAvalonia.UI.Controls;
using SecRandom.Core.Abstraction;
using SecRandom.Core.Services;
using Res = SecRandom.Langs.SettingsPages.ListManagementPage.Resources;

namespace SecRandom.Views.SettingsPages.ListManagementSubPages.LotterySubPages;

public partial class ImportPrizePage : UserControl
{
    private readonly LotteryListService _lotteryListService;
    private string? _selectedFilePath;

    public ImportPrizePage()
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
        
        var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = Res.SelectFile,
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType(Res.SupportedFormats)
                {
                    Patterns = new[] { "*.json", "*.csv", "*.txt" }
                },
                new FilePickerFileType("JSON") { Patterns = new[] { "*.json" } },
                new FilePickerFileType("CSV") { Patterns = new[] { "*.csv" } },
                new FilePickerFileType("TXT") { Patterns = new[] { "*.txt" } }
            }
        });

        if (files.Count > 0)
        {
            _selectedFilePath = files[0].Path.LocalPath;
            var filePathTextBox = this.FindControl<TextBox>("FilePathTextBox");
            if (filePathTextBox != null)
            {
                filePathTextBox.Text = _selectedFilePath;
            }
            
            var importButton = this.FindControl<Button>("ImportButton");
            if (importButton != null)
            {
                importButton.IsEnabled = true;
            }
        }
    }

    private async void ImportButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var comboBox = this.FindControl<ComboBox>("PoolNameComboBox");
        var poolName = comboBox?.SelectedItem as string;
        
        if (string.IsNullOrEmpty(poolName))
        {
            await ShowMessageAsync(Res.Error, Res.PleaseSelectPool);
            return;
        }
        
        if (string.IsNullOrEmpty(_selectedFilePath) || !File.Exists(_selectedFilePath))
        {
            await ShowMessageAsync(Res.Error, Res.PleaseSelectValidFile);
            return;
        }

        var clearExistingCheckBox = this.FindControl<CheckBox>("ClearExistingCheckBox");
        var clearExisting = clearExistingCheckBox?.IsChecked == true;

        var importedCount = _lotteryListService.ImportPrizes(poolName, _selectedFilePath, clearExisting);
        
        if (importedCount > 0)
        {
            await ShowMessageAsync(Res.Success, string.Format(Res.ImportSuccess, importedCount));
            
            // 重置状态
            _selectedFilePath = null;
            var filePathTextBox = this.FindControl<TextBox>("FilePathTextBox");
            if (filePathTextBox != null)
            {
                filePathTextBox.Text = string.Empty;
            }
            
            var importButton = this.FindControl<Button>("ImportButton");
            if (importButton != null)
            {
                importButton.IsEnabled = false;
            }
        }
        else
        {
            await ShowMessageAsync(Res.Failed, Res.ImportFailed);
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
