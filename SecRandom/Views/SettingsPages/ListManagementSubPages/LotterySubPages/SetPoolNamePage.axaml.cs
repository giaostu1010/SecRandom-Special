using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using FluentAvalonia.UI.Controls;
using SecRandom.Core.Abstraction;
using SecRandom.Core.Services;
using Res = SecRandom.Langs.SettingsPages.ListManagementPage.Resources;

namespace SecRandom.Views.SettingsPages.ListManagementSubPages.LotterySubPages;

public partial class SetPoolNamePage : UserControl
{
    private readonly LotteryListService _lotteryListService;
    public ObservableCollection<string> PoolNames { get; } = [];

    public SetPoolNamePage()
    {
        _lotteryListService = IAppHost.GetService<LotteryListService>();
        InitializeComponent();
        RefreshPoolList();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void RefreshPoolList()
    {
        _lotteryListService.RefreshPoolNames();
        PoolNames.Clear();
        foreach (var name in _lotteryListService.PoolNames)
        {
            PoolNames.Add(name);
        }
        
        var itemsControl = this.FindControl<ItemsControl>("PoolListItemsControl");
        if (itemsControl != null)
        {
            itemsControl.ItemsSource = PoolNames;
        }
    }

    private async void CreatePoolButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var textBox = this.FindControl<TextBox>("NewPoolNameTextBox");
        if (textBox == null || string.IsNullOrWhiteSpace(textBox.Text))
        {
            await ShowMessageAsync(Res.Tip, Res.PleaseEnterPoolName);
            return;
        }

        var poolName = textBox.Text.Trim();
        if (_lotteryListService.CreatePool(poolName))
        {
            textBox.Text = string.Empty;
            RefreshPoolList();
            await ShowMessageAsync(Res.Success, string.Format(Res.PoolCreatedSuccess, poolName));
        }
        else
        {
            await ShowMessageAsync(Res.Failed, Res.PoolCreateFailed);
        }
    }

    private async void RenamePoolButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is string oldName)
        {
            var dialog = new ContentDialog
            {
                Title = Res.RenamePool,
                PrimaryButtonText = Res.Confirm,
                CloseButtonText = Res.Cancel
            };

            var textBox = new TextBox
            {
                Text = oldName,
                Watermark = Res.EnterNewName
            };
            dialog.Content = textBox;

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary && !string.IsNullOrWhiteSpace(textBox.Text))
            {
                var newName = textBox.Text.Trim();
                if (_lotteryListService.RenamePool(oldName, newName))
                {
                    RefreshPoolList();
                    await ShowMessageAsync(Res.Success, string.Format(Res.PoolRenamedSuccess, newName));
                }
                else
                {
                    await ShowMessageAsync(Res.Failed, Res.RenameFailed);
                }
            }
        }
    }

    private async void DeletePoolButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is string poolName)
        {
            var dialog = new ContentDialog
            {
                Title = Res.ConfirmDelete,
                Content = string.Format(Res.ConfirmDeletePool, poolName),
                PrimaryButtonText = Res.Delete,
                CloseButtonText = Res.Cancel
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                if (_lotteryListService.DeletePool(poolName))
                {
                    RefreshPoolList();
                    await ShowMessageAsync(Res.Success, string.Format(Res.PoolDeletedSuccess, poolName));
                }
                else
                {
                    await ShowMessageAsync(Res.Failed, Res.DeletePoolFailed);
                }
            }
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
