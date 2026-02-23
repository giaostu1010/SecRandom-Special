using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SecRandom.Core.Abstraction;
using SecRandom.Core.Attributes;
using SecRandom.Core;
using SecRandom.Services.Config;
using SecRandom.ViewModels;

namespace SecRandom.Views.SettingsPages;

[PageInfo("settings.draw.lottery", "\ue07c")]
public partial class LotterySettingsSubPage : UserControl
{
    private readonly RootConfigHandler _rootConfigHandler;
    private readonly ListNamesSource _globalListNamesSource;
    private readonly LotteryListSpecificSettingsViewModel _listSpecificViewModel;

    public LotterySettingsSubPage()
    {
        _rootConfigHandler = IAppHost.GetService<RootConfigHandler>();
        DataContext = _rootConfigHandler.Data.DrawSettings.LotterySettings;
        InitializeComponent();

        _globalListNamesSource = new ListNamesSource(Utils.GetFilePath("list", "lottery_list"));
        _globalListNamesSource.PropertyChanged += GlobalListNamesSource_OnPropertyChanged;

        var defaultPoolComboBox = this.FindControl<ComboBox>("DefaultPoolComboBox");
        if (defaultPoolComboBox is not null)
        {
            defaultPoolComboBox.ItemsSource = _globalListNamesSource.Names;
        }

        _listSpecificViewModel = new LotteryListSpecificSettingsViewModel(_rootConfigHandler);
        _listSpecificViewModel.PropertyChanged += ListSpecificViewModel_OnPropertyChanged;

        var listSpecificPanel = this.FindControl<StackPanel>("ListSpecificSettingsPanel");
        if (listSpecificPanel is not null)
        {
            listSpecificPanel.DataContext = _listSpecificViewModel;
        }

        var listSpecificPoolComboBox = this.FindControl<ComboBox>("ListSpecificPoolComboBox");
        if (listSpecificPoolComboBox is not null)
        {
            listSpecificPoolComboBox.ItemsSource = _listSpecificViewModel.ListNames;
        }

        var listSpecificDefaultPoolComboBox = this.FindControl<ComboBox>("ListSpecificDefaultPoolComboBox");
        if (listSpecificDefaultPoolComboBox is not null)
        {
            listSpecificDefaultPoolComboBox.ItemsSource = _listSpecificViewModel.ListNames;
        }

        DetachedFromVisualTree += LotterySettingsSubPage_OnDetachedFromVisualTree;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void GlobalListNamesSource_OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(ListNamesSource.Names))
        {
            return;
        }

        var defaultPoolComboBox = this.FindControl<ComboBox>("DefaultPoolComboBox");
        if (defaultPoolComboBox is not null)
        {
            defaultPoolComboBox.ItemsSource = _globalListNamesSource.Names;
        }
    }

    private void ListSpecificViewModel_OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(LotteryListSpecificSettingsViewModel.ListNames))
        {
            return;
        }

        var listSpecificPoolComboBox = this.FindControl<ComboBox>("ListSpecificPoolComboBox");
        if (listSpecificPoolComboBox is not null)
        {
            listSpecificPoolComboBox.ItemsSource = _listSpecificViewModel.ListNames;
        }

        var listSpecificDefaultPoolComboBox = this.FindControl<ComboBox>("ListSpecificDefaultPoolComboBox");
        if (listSpecificDefaultPoolComboBox is not null)
        {
            listSpecificDefaultPoolComboBox.ItemsSource = _listSpecificViewModel.ListNames;
        }
    }

    private void SyncListSpecificSettings_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _listSpecificViewModel.SyncWithGlobal();
    }

    private void LotterySettingsSubPage_OnDetachedFromVisualTree(object? sender, Avalonia.VisualTreeAttachmentEventArgs e)
    {
        DetachedFromVisualTree -= LotterySettingsSubPage_OnDetachedFromVisualTree;

        _globalListNamesSource.PropertyChanged -= GlobalListNamesSource_OnPropertyChanged;
        _globalListNamesSource.Dispose();

        _listSpecificViewModel.PropertyChanged -= ListSpecificViewModel_OnPropertyChanged;
        _listSpecificViewModel.Dispose();
    }
}
