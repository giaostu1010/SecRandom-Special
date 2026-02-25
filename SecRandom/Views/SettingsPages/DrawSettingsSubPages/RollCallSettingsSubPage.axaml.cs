using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

[PageInfo("settings.draw.rollCall", "\uECAA", "settings.draw")]
public partial class RollCallSettingsSubPage : UserControl
{
    private readonly MainConfigHandler _mainConfigHandler;
    private readonly RollCallSettingsConfig _globalSettings;
    private readonly ListNamesSource _globalListNamesSource;
    private ComboBox? _customFontComboBox;
    private List<FontFamily> _fontFamilies = [];

    public RollCallSettingsSubPage()
    {
        _mainConfigHandler = IAppHost.GetService<MainConfigHandler>();
        _globalSettings = _mainConfigHandler.Data.DrawSettings.RollCallSettings;
        DataContext = _globalSettings;
        InitializeComponent();

        _globalListNamesSource = new ListNamesSource(Utils.GetFilePath("list", "roll_call_list"));
        _globalListNamesSource.PropertyChanged += GlobalListNamesSource_OnPropertyChanged;

        var defaultClassComboBox = this.FindControl<ComboBox>("DefaultClassComboBox");
        if (defaultClassComboBox is not null)
        {
            defaultClassComboBox.ItemsSource = _globalListNamesSource.Names;
        }

        _customFontComboBox = this.FindControl<ComboBox>("CustomFontComboBox");
        InitializeFontFamilies();

        _globalSettings.PropertyChanged += GlobalSettings_OnPropertyChanged;

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
        if (e.PropertyName == nameof(RollCallSettingsConfig.CustomFont))
        {
            SyncGlobalCustomFontSelection();
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

    private void OpenStudentImageFolder_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var folderPath = Utils.GetFilePath("images", "student_images");
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
        SettingsView.Current?.SelectNavigationItemById("settings.draw.rollCall.listSpecific");
    }

    private void RollCallSettingsSubPage_OnDetachedFromVisualTree(object? sender, Avalonia.VisualTreeAttachmentEventArgs e)
    {
        DetachedFromVisualTree -= RollCallSettingsSubPage_OnDetachedFromVisualTree;

        _globalSettings.PropertyChanged -= GlobalSettings_OnPropertyChanged;

        _globalListNamesSource.PropertyChanged -= GlobalListNamesSource_OnPropertyChanged;
        _globalListNamesSource.Dispose();
    }
}
