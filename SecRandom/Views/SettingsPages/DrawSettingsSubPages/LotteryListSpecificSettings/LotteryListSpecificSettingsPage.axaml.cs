using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using SecRandom.Core.Abstraction;
using SecRandom.Core.Attributes;
using SecRandom.Core;
using SecRandom.Services.Config;
using SecRandom.ViewModels;

namespace SecRandom.Views.SettingsPages;

[PageInfo("settings.draw.lottery.listSpecific", "\ue8a7")]
public partial class LotteryListSpecificSettingsPage : UserControl
{
    private readonly MainConfigHandler _mainConfigHandler;
    private readonly LotteryListSpecificSettingsViewModel _listSpecificViewModel;
    private ComboBox? _listSpecificClearRecordComboBox;
    private NumericUpDown? _listSpecificHalfRepeatUpDown;
    private ComboBox? _listSpecificCustomFontComboBox;
    private List<FontFamily> _fontFamilies = [];

    public LotteryListSpecificSettingsPage()
    {
        _mainConfigHandler = IAppHost.GetService<MainConfigHandler>();
        _listSpecificViewModel = new LotteryListSpecificSettingsViewModel(_mainConfigHandler);
        _listSpecificViewModel.PropertyChanged += ListSpecificViewModel_OnPropertyChanged;

        DataContext = _listSpecificViewModel;
        InitializeComponent();

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

        _listSpecificClearRecordComboBox = this.FindControl<ComboBox>("ListSpecificClearRecordComboBox");
        _listSpecificHalfRepeatUpDown = this.FindControl<NumericUpDown>("ListSpecificHalfRepeatUpDown");
        UpdateDependentControls(_listSpecificViewModel.Settings.DrawMode);

        _listSpecificCustomFontComboBox = this.FindControl<ComboBox>("ListSpecificCustomFontComboBox");
        InitializeFontFamilies();

        DetachedFromVisualTree += LotteryListSpecificSettingsPage_OnDetachedFromVisualTree;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void ListSpecificViewModel_OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(LotteryListSpecificSettingsViewModel.Settings))
        {
            SyncListSpecificCustomFontSelection();
            UpdateDependentControls(_listSpecificViewModel.Settings.DrawMode);
            return;
        }

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

    private void InitializeFontFamilies()
    {
        var fontCollection = FontManager.Current.SystemFonts;
        _fontFamilies = new List<FontFamily>(fontCollection).OrderBy(x => x.Name).ToList();

        if (_listSpecificCustomFontComboBox is not null)
        {
            _listSpecificCustomFontComboBox.ItemsSource = _fontFamilies;
            SyncListSpecificCustomFontSelection();
        }
    }

    private void SyncListSpecificCustomFontSelection()
    {
        if (_listSpecificCustomFontComboBox is null)
        {
            return;
        }

        var desiredName = _listSpecificViewModel.Settings.CustomFont;
        SyncSelectedFontFamily(_listSpecificCustomFontComboBox, desiredName);
    }

    private void SyncSelectedFontFamily(ComboBox comboBox, string desiredName)
    {
        if (_fontFamilies.Count == 0)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(desiredName))
        {
            if (comboBox.SelectedIndex != -1)
            {
                comboBox.SelectedIndex = -1;
            }
            return;
        }

        var selected = _fontFamilies.FirstOrDefault(x =>
            string.Equals(x.Name, desiredName, StringComparison.OrdinalIgnoreCase));

        if (selected is null)
        {
            return;
        }

        if (!Equals(comboBox.SelectedItem, selected))
        {
            comboBox.SelectedItem = selected;
        }
    }

    private void SyncListSpecificSettings_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _listSpecificViewModel.SyncWithGlobal();
    }

    private void ListSpecificDrawModeComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        UpdateDependentControls(_listSpecificViewModel.Settings.DrawMode);
    }

    private void UpdateDependentControls(int drawMode)
    {
        if (_listSpecificClearRecordComboBox is not null)
        {
            _listSpecificClearRecordComboBox.IsEnabled = drawMode != 0;
        }

        if (_listSpecificHalfRepeatUpDown is not null)
        {
            _listSpecificHalfRepeatUpDown.IsEnabled = drawMode == 2;
        }
    }

    private void ListSpecificCustomFontComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is not ComboBox comboBox || comboBox.SelectedItem is not FontFamily selected)
        {
            return;
        }

        if (comboBox.DataContext is not LotterySettingsOverrideProxy proxy)
        {
            return;
        }

        if (!string.Equals(proxy.CustomFont, selected.Name, StringComparison.Ordinal))
        {
            proxy.CustomFont = selected.Name;
        }
    }

    private void OpenLotteryImageFolder_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var folderPath = Utils.GetFilePath("images", "prize_images");
        Directory.CreateDirectory(folderPath);
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = folderPath,
                UseShellExecute = true
            });
        }
        catch
        {
        }
    }

    private void LotteryListSpecificSettingsPage_OnDetachedFromVisualTree(object? sender, Avalonia.VisualTreeAttachmentEventArgs e)
    {
        DetachedFromVisualTree -= LotteryListSpecificSettingsPage_OnDetachedFromVisualTree;

        _listSpecificViewModel.PropertyChanged -= ListSpecificViewModel_OnPropertyChanged;
        _listSpecificViewModel.Dispose();
    }
}
