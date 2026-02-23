using System.Diagnostics;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SecRandom.Core.Abstraction;
using SecRandom.Core.Attributes;
using SecRandom.Core;
using SecRandom.Services.Config;
using SecRandom.ViewModels;

namespace SecRandom.Views.SettingsPages;

[PageInfo("settings.draw.rollCall", "\ue07c")]
public partial class RollCallSettingsSubPage : UserControl
{
    private readonly RootConfigHandler _rootConfigHandler;
    private readonly ListNamesSource _globalListNamesSource;
    private readonly RollCallListSpecificSettingsViewModel _listSpecificViewModel;

    public RollCallSettingsSubPage()
    {
        _rootConfigHandler = IAppHost.GetService<RootConfigHandler>();
        DataContext = _rootConfigHandler.Data.DrawSettings.RollCallSettings;
        InitializeComponent();

        _globalListNamesSource = new ListNamesSource(Utils.GetFilePath("list", "roll_call_list"));
        _globalListNamesSource.PropertyChanged += GlobalListNamesSource_OnPropertyChanged;

        var defaultClassComboBox = this.FindControl<ComboBox>("DefaultClassComboBox");
        if (defaultClassComboBox is not null)
        {
            defaultClassComboBox.ItemsSource = _globalListNamesSource.Names;
        }

        _listSpecificViewModel = new RollCallListSpecificSettingsViewModel(_rootConfigHandler);
        _listSpecificViewModel.PropertyChanged += ListSpecificViewModel_OnPropertyChanged;

        var listSpecificPanel = this.FindControl<StackPanel>("ListSpecificSettingsPanel");
        if (listSpecificPanel is not null)
        {
            listSpecificPanel.DataContext = _listSpecificViewModel;
        }

        var listSpecificClassComboBox = this.FindControl<ComboBox>("ListSpecificClassComboBox");
        if (listSpecificClassComboBox is not null)
        {
            listSpecificClassComboBox.ItemsSource = _listSpecificViewModel.ListNames;
        }

        var listSpecificDefaultClassComboBox = this.FindControl<ComboBox>("ListSpecificDefaultClassComboBox");
        if (listSpecificDefaultClassComboBox is not null)
        {
            listSpecificDefaultClassComboBox.ItemsSource = _listSpecificViewModel.ListNames;
        }

        DetachedFromVisualTree += RollCallSettingsSubPage_OnDetachedFromVisualTree;
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

        var defaultClassComboBox = this.FindControl<ComboBox>("DefaultClassComboBox");
        if (defaultClassComboBox is not null)
        {
            defaultClassComboBox.ItemsSource = _globalListNamesSource.Names;
        }
    }

    private void ListSpecificViewModel_OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(RollCallListSpecificSettingsViewModel.ListNames))
        {
            return;
        }

        var listSpecificClassComboBox = this.FindControl<ComboBox>("ListSpecificClassComboBox");
        if (listSpecificClassComboBox is not null)
        {
            listSpecificClassComboBox.ItemsSource = _listSpecificViewModel.ListNames;
        }

        var listSpecificDefaultClassComboBox = this.FindControl<ComboBox>("ListSpecificDefaultClassComboBox");
        if (listSpecificDefaultClassComboBox is not null)
        {
            listSpecificDefaultClassComboBox.ItemsSource = _listSpecificViewModel.ListNames;
        }
    }

    private void SyncListSpecificSettings_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _listSpecificViewModel.SyncWithGlobal();
    }

    private void OpenStudentImageFolder_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var folderPath = Utils.GetFilePath("images", "student_images");
        Process.Start(new ProcessStartInfo
        {
            FileName = folderPath,
            UseShellExecute = true
        });
    }

    private void RollCallSettingsSubPage_OnDetachedFromVisualTree(object? sender, Avalonia.VisualTreeAttachmentEventArgs e)
    {
        DetachedFromVisualTree -= RollCallSettingsSubPage_OnDetachedFromVisualTree;

        _globalListNamesSource.PropertyChanged -= GlobalListNamesSource_OnPropertyChanged;
        _globalListNamesSource.Dispose();

        _listSpecificViewModel.PropertyChanged -= ListSpecificViewModel_OnPropertyChanged;
        _listSpecificViewModel.Dispose();
    }
}
