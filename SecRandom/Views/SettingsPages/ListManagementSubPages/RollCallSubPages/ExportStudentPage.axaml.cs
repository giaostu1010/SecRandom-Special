using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using FluentAvalonia.UI.Controls;
using Microsoft.Extensions.Logging;
using SecRandom.Core.Attributes;
using SecRandom.Core.Enums;
using SecRandom.Core.Models;
using SecRandom.Core.Services;

namespace SecRandom.Views.SettingsPages.ListManagementSubPages.RollCallSubPages;

[PageInfo("settings.listManagement.rollCall.exportStudent", "\uEDE1", "settings.listManagement", PageLocation.Top, true)]
public partial class ExportStudentPage : UserControl
{
    private readonly RollCallListService _service;
    private readonly ILogger<ExportStudentPage>? _logger;
    private string? _currentClassName;
    private List<StudentItem> _students = new();

    public ExportStudentPage()
    {
        InitializeComponent();
        
        // 获取服务
        _service = new RollCallListService();
        
        // 初始化
        InitializePage();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void InitializePage()
    {
        // 获取当前选中的班级
        _service.RefreshClassNames();
        _currentClassName = _service.CurrentClassName;
        
        var currentClassTextBlock = this.FindControl<TextBlock>("CurrentClassTextBlock");
        if (currentClassTextBlock != null)
        {
            currentClassTextBlock.Text = string.IsNullOrEmpty(_currentClassName) 
                ? "当前班级：未选择" 
                : $"当前班级：{_currentClassName}";
        }

        // 加载学生列表
        LoadStudents();
    }

    private void LoadStudents()
    {
        if (string.IsNullOrEmpty(_currentClassName))
        {
            UpdateStudentCount(0);
            return;
        }

        try
        {
            _students = _service.GetStudentList(_currentClassName);
            
            // 更新学生数量
            UpdateStudentCount(_students.Count);

            // 更新预览
            var previewDataGrid = this.FindControl<DataGrid>("PreviewDataGrid");
            if (previewDataGrid != null)
            {
                var previewItems = _students.Select(s => new ExportPreviewItem
                {
                    Id = s.Id,
                    Name = s.Name,
                    Gender = s.Gender,
                    Group = s.Group,
                    TagsDisplay = string.Join(", ", s.Tags)
                }).ToList();
                
                previewDataGrid.ItemsSource = previewItems;
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "加载学生列表失败");
            UpdateStudentCount(0);
        }
    }

    private void UpdateStudentCount(int count)
    {
        var studentCountTextBlock = this.FindControl<TextBlock>("StudentCountTextBlock");
        if (studentCountTextBlock != null)
        {
            studentCountTextBlock.Text = count.ToString();
        }
    }

    private void RefreshButton_OnClick(object? sender, RoutedEventArgs e)
    {
        LoadStudents();
    }

    private async void ExportButton_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(_currentClassName))
            {
                await ShowMessageAsync("错误", "请先选择班级");
                return;
            }

            if (_students.Count == 0)
            {
                await ShowMessageAsync("提示", "当前班级没有学生数据");
                return;
            }

            // 获取选择的格式
            var formatComboBox = this.FindControl<ComboBox>("FormatComboBox");
            if (formatComboBox == null) return;

            var selectedIndex = formatComboBox.SelectedIndex;
            var (extension, filterName) = selectedIndex switch
            {
                0 => (".json", "JSON 文件"),
                1 => (".csv", "CSV 文件"),
                2 => (".txt", "TXT 文件"),
                _ => (".json", "JSON 文件")
            };

            // 打开保存文件对话框
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel == null) return;

            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "导出学生名单",
                SuggestedFileName = $"{_currentClassName}_学生名单-SecRandom{extension}",
                FileTypeChoices = new[]
                {
                    new FilePickerFileType(filterName) { Patterns = new[] { $"*{extension}" } }
                }
            });

            if (file == null) return;

            var filePath = file.Path.LocalPath;

            // 导出数据
            var success = selectedIndex switch
            {
                0 => ExportToJson(filePath),
                1 => ExportToCsv(filePath),
                2 => ExportToTxt(filePath),
                _ => ExportToJson(filePath)
            };

            if (success)
            {
                await ShowMessageAsync("成功", $"成功导出 {_students.Count} 名学生到：\n{filePath}");
            }
            else
            {
                await ShowMessageAsync("错误", "导出失败");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "导出学生名单失败");
            await ShowMessageAsync("错误", $"导出失败：{ex.Message}");
        }
    }

    private bool ExportToJson(string filePath)
    {
        try
        {
            var data = _students.ToDictionary(
                s => s.Name,
                s => new
                {
                    id = s.Id,
                    gender = s.Gender,
                    group = s.Group,
                    exist = s.Exist,
                    tags = s.Tags
                });

            var json = System.Text.Json.JsonSerializer.Serialize(data, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
            
            File.WriteAllText(filePath, json);
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "导出JSON失败");
            return false;
        }
    }

    private bool ExportToCsv(string filePath)
    {
        try
        {
            using var writer = new StreamWriter(filePath, false, Encoding.UTF8);
            writer.WriteLine("学号,姓名,性别,小组,标签");

            foreach (var student in _students)
            {
                var tags = string.Join(",", student.Tags);
                writer.WriteLine($"{student.Id},{student.Name},{student.Gender},{student.Group},{tags}");
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "导出CSV失败");
            return false;
        }
    }

    private bool ExportToTxt(string filePath)
    {
        try
        {
            using var writer = new StreamWriter(filePath, false, Encoding.UTF8);

            foreach (var student in _students)
            {
                writer.WriteLine(student.Name);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "导出TXT失败");
            return false;
        }
    }

    private static async Task ShowMessageAsync(string title, string message)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            CloseButtonText = "确定"
        };
        await dialog.ShowAsync();
    }
}

/// <summary>
/// 导出预览项
/// </summary>
public class ExportPreviewItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Group { get; set; } = string.Empty;
    public string TagsDisplay { get; set; } = string.Empty;
}
