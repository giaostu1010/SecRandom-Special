using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using SecRandom.Core;
using SecRandom.Core.Attributes;
using SecRandom.Core.Enums;
using SecRandom.ViewModels;

namespace SecRandom.Views.SettingsPages.ListManagementSubPages.TablePreview;

public class StudentDisplayItem
{
    public string ExistText { get; set; } = "";
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Gender { get; set; } = "";
    public string Group { get; set; } = "";
    public string Tags { get; set; } = "";
}

[PageInfo("settings.listManagement.rollCall.preview", "\uE8A1", "settings.listManagement", PageLocation.Top, true)]
public partial class RollCallTablePreviewPage : UserControl
{
    private readonly ListNamesSource _listNamesSource;
    private readonly ObservableCollection<StudentDisplayItem> _students = [];

    public RollCallTablePreviewPage()
    {
        _listNamesSource = new ListNamesSource(Utils.GetFilePath("list", "roll_call_list"));
        _listNamesSource.PropertyChanged += ListNamesSource_OnPropertyChanged;
        
        InitializeComponent();
        
        Loaded += RollCallTablePreviewPage_Loaded;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void RollCallTablePreviewPage_Loaded(object? sender, RoutedEventArgs e)
    {
        var classComboBox = this.FindControl<ComboBox>("ClassComboBox");
        if (classComboBox is not null)
        {
            classComboBox.ItemsSource = _listNamesSource.Names;
        }
    }

    private void ListNamesSource_OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(ListNamesSource.Names))
        {
            return;
        }

        var classComboBox = this.FindControl<ComboBox>("ClassComboBox");
        if (classComboBox is not null)
        {
            classComboBox.ItemsSource = _listNamesSource.Names;
        }
    }

    private void ClassComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is not ComboBox comboBox || comboBox.SelectedItem is not string className)
        {
            return;
        }

        LoadStudents(className);
    }

    private void LoadStudents(string className)
    {
        _students.Clear();
        
        if (string.IsNullOrWhiteSpace(className))
        {
            var previewBorder = this.FindControl<Border>("PreviewBorder");
            if (previewBorder is not null)
            {
                previewBorder.IsVisible = false;
            }
            return;
        }

        var filePath = Utils.GetFilePath("list", "roll_call_list", $"{className}.json");
        if (!File.Exists(filePath))
        {
            var previewBorder = this.FindControl<Border>("PreviewBorder");
            if (previewBorder is not null)
            {
                previewBorder.IsVisible = false;
            }
            return;
        }

        try
        {
            var jsonContent = File.ReadAllText(filePath);
            var studentData = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(jsonContent);
            
            if (studentData is not null)
            {
                foreach (var kvp in studentData)
                {
                    var student = kvp.Value;
                    _students.Add(new StudentDisplayItem
                    {
                        ExistText = student.TryGetValue("exist", out var exist) && exist is bool b && b ? "✓" : "",
                        Id = student.TryGetValue("id", out var id) ? id?.ToString() ?? "" : "",
                        Name = kvp.Key,
                        Gender = student.TryGetValue("gender", out var gender) ? gender?.ToString() ?? "" : "",
                        Group = student.TryGetValue("group", out var group) ? group?.ToString() ?? "" : "",
                        Tags = student.TryGetValue("tags", out var tags) && tags is JsonElement tagsElement 
                            ? string.Join(", ", tagsElement.EnumerateArray().Select(t => t.GetString())) 
                            : ""
                    });
                }
            }
            
            var previewBorder = this.FindControl<Border>("PreviewBorder");
            if (previewBorder is not null)
            {
                previewBorder.IsVisible = _students.Count > 0;
            }
            
            var studentsList = this.FindControl<ItemsControl>("StudentsList");
            if (studentsList is not null)
            {
                studentsList.ItemsSource = _students;
            }
        }
        catch
        {
            var previewBorder = this.FindControl<Border>("PreviewBorder");
            if (previewBorder is not null)
            {
                previewBorder.IsVisible = false;
            }
        }
    }
}
