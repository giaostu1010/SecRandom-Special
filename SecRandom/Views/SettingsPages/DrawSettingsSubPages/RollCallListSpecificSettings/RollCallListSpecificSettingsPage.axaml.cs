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
using SecRandom.Core.Enums;
using SecRandom.Services.Config;
using SecRandom.ViewModels;

namespace SecRandom.Views.SettingsPages;

[PageInfo("settings.draw.rollCall.listSpecific", "\ue8a7", "settings.draw", PageLocation.Top, true)]
public partial class RollCallListSpecificSettingsPage : UserControl
{
    private readonly MainConfigHandler _mainConfigHandler;
    private readonly RollCallListSpecificSettingsViewModel _listSpecificViewModel;
    private ComboBox? _listSpecificCustomFontComboBox;
    private List<FontFamily> _fontFamilies = [];

    public RollCallListSpecificSettingsPage()
    {
        _mainConfigHandler = IAppHost.GetService<MainConfigHandler>();
        _listSpecificViewModel = new RollCallListSpecificSettingsViewModel(_mainConfigHandler);
        _listSpecificViewModel.PropertyChanged += ListSpecificViewModel_OnPropertyChanged;

        DataContext = _listSpecificViewModel;
        InitializeComponent();

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

        _listSpecificCustomFontComboBox = this.FindControl<ComboBox>("ListSpecificCustomFontComboBox");
        InitializeFontFamilies();

        DetachedFromVisualTree += RollCallListSpecificSettingsPage_OnDetachedFromVisualTree;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void ListSpecificViewModel_OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(RollCallListSpecificSettingsViewModel.Settings))
        {
            SyncListSpecificCustomFontSelection();
            return;
        }

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

    private void ListSpecificCustomFontComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is not ComboBox comboBox || comboBox.SelectedItem is not FontFamily selected)
        {
            return;
        }

        if (comboBox.DataContext is not RollCallSettingsOverrideProxy proxy)
        {
            return;
        }

        if (!string.Equals(proxy.CustomFont, selected.Name, StringComparison.Ordinal))
        {
            proxy.CustomFont = selected.Name;
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

    private void RollCallListSpecificSettingsPage_OnDetachedFromVisualTree(object? sender, Avalonia.VisualTreeAttachmentEventArgs e)
    {
        DetachedFromVisualTree -= RollCallListSpecificSettingsPage_OnDetachedFromVisualTree;

        _listSpecificViewModel.PropertyChanged -= ListSpecificViewModel_OnPropertyChanged;
        _listSpecificViewModel.Dispose();
    }
}
