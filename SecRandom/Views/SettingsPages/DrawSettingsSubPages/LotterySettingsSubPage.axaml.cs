using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.VisualTree;
using SecRandom.Core.Abstraction;
using SecRandom.Core.Attributes;
using SecRandom.Core.Services;
using SecRandom.Core;
using SecRandom.Models.Config;
using SecRandom.Services.Config;
using SecRandom.ViewModels;

namespace SecRandom.Views.SettingsPages;

[PageInfo("settings.draw.lottery", "\uE8EC", "settings.draw")]
public partial class LotterySettingsSubPage : UserControl
{
    private readonly MainConfigHandler _mainConfigHandler;
    private readonly LotterySettingsConfig _globalSettings;
    private readonly ListNamesSource _globalListNamesSource;
    private ComboBox? _clearRecordComboBox;
    private NumericUpDown? _halfRepeatUpDown;
    private ComboBox? _customFontComboBox;
    private List<FontFamily> _fontFamilies = [];

    public LotterySettingsSubPage()
    {
        _mainConfigHandler = IAppHost.GetService<MainConfigHandler>();
        _globalSettings = _mainConfigHandler.Data.DrawSettings.LotterySettings;
        DataContext = _globalSettings;
        InitializeComponent();

        _globalListNamesSource = new ListNamesSource(Utils.GetFilePath("list", "lottery_list"));
        _globalListNamesSource.PropertyChanged += GlobalListNamesSource_OnPropertyChanged;

        var defaultPoolComboBox = this.FindControl<ComboBox>("DefaultPoolComboBox");
        if (defaultPoolComboBox is not null)
        {
            defaultPoolComboBox.ItemsSource = _globalListNamesSource.Names;
        }

        _clearRecordComboBox = this.FindControl<ComboBox>("ClearRecordComboBox");
        _halfRepeatUpDown = this.FindControl<NumericUpDown>("HalfRepeatUpDown");
        UpdateDependentControls(_globalSettings.DrawMode);

        _customFontComboBox = this.FindControl<ComboBox>("CustomFontComboBox");
        InitializeFontFamilies();

        _globalSettings.PropertyChanged += GlobalSettings_OnPropertyChanged;

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

    private void InitializeFontFamilies()
    {
        var fontCollection = FontManager.Current.SystemFonts;
        _fontFamilies = new List<FontFamily>(fontCollection).OrderBy(x => x.Name).ToList();

        if (_customFontComboBox is not null)
        {
            _customFontComboBox.ItemsSource = _fontFamilies;
            SyncGlobalCustomFontSelection();
        }
    }

    private void GlobalSettings_OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(LotterySettingsConfig.CustomFont))
        {
            SyncGlobalCustomFontSelection();
        }
    }

    private void DrawModeComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        UpdateDependentControls(_globalSettings.DrawMode);
    }

    private void UpdateDependentControls(int drawMode)
    {
        if (_clearRecordComboBox is not null)
        {
            _clearRecordComboBox.IsEnabled = drawMode != 0;
        }

        if (_halfRepeatUpDown is not null)
        {
            _halfRepeatUpDown.IsEnabled = drawMode == 2;
        }
    }

    private void SyncGlobalCustomFontSelection()
    {
        if (_customFontComboBox is null)
        {
            return;
        }

        SyncSelectedFontFamily(_customFontComboBox, _globalSettings.CustomFont);
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

    private void CustomFontComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is not ComboBox comboBox || comboBox.SelectedItem is not FontFamily selected)
        {
            return;
        }

        if (!string.Equals(_globalSettings.CustomFont, selected.Name, StringComparison.Ordinal))
        {
            _globalSettings.CustomFont = selected.Name;
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
    
    private void OpenListSpecificSettings_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        SettingsView.Current?.SelectNavigationItemById("settings.draw.lottery.listSpecific");
    }

    private void LotterySettingsSubPage_OnDetachedFromVisualTree(object? sender, Avalonia.VisualTreeAttachmentEventArgs e)
    {
        DetachedFromVisualTree -= LotterySettingsSubPage_OnDetachedFromVisualTree;

        _globalSettings.PropertyChanged -= GlobalSettings_OnPropertyChanged;

        _globalListNamesSource.PropertyChanged -= GlobalListNamesSource_OnPropertyChanged;
        _globalListNamesSource.Dispose();
    }
}
