using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using MiniExcelLibs;
using SecRandom.Core.Attributes;
using SecRandom.Core.Enums;
using SecRandom.Core.Models;
using SecRandom.Core.Services;

namespace SecRandom.Views.SettingsPages.ListManagementSubPages.RollCallSubPages;

[PageInfo("settings.listManagement.rollCall.importStudent", "\uE8F4", "settings.listManagement", PageLocation.Top, true)]
public partial class ImportStudentPage : UserControl
{
    private readonly RollCallListService _service;
    private readonly ILogger<ImportStudentPage>? _logger;
    private string? _selectedFilePath;
    private List<string> _columns = [];
    private List<Dictionary<string, string>> _rawData = [];
    private string? _currentClassName;

    public ImportStudentPage()
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
    }

    private async void SelectFileButton_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            // 获取顶级窗口
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel == null) return;

            // 打开文件选择对话框
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "选择学生名单文件",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("所有支持格式") { Patterns = new[] { "*.xlsx", "*.xls", "*.csv", "*.txt", "*.json" } },
                    new FilePickerFileType("Excel 文件") { Patterns = new[] { "*.xlsx", "*.xls" } },
                    new FilePickerFileType("CSV 文件") { Patterns = new[] { "*.csv" } },
                    new FilePickerFileType("文本文件") { Patterns = new[] { "*.txt" } },
                    new FilePickerFileType("JSON 文件") { Patterns = new[] { "*.json" } }
                }
            });

            if (files.Count == 0) return;

            var file = files[0];
            _selectedFilePath = file.Path.LocalPath;

            // 更新文件路径显示
            var filePathTextBlock = this.FindControl<TextBlock>("FilePathTextBlock");
            if (filePathTextBlock != null)
            {
                filePathTextBlock.Text = file.Name;
            }

            // 加载文件数据
            await LoadFileData(_selectedFilePath);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "选择文件失败");
            await ShowMessageAsync("错误", $"选择文件失败：{ex.Message}");
        }
    }

    private async Task LoadFileData(string filePath)
    {
        try
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            
            switch (extension)
            {
                case ".csv":
                    LoadFromCsv(filePath);
                    break;
                case ".txt":
                    LoadFromTxt(filePath);
                    break;
                case ".json":
                    LoadFromJson(filePath);
                    break;
                case ".xlsx":
                case ".xls":
                    LoadFromExcel(filePath);
                    break;
                default:
                    await ShowMessageAsync("错误", "不支持的文件格式");
                    return;
            }

            if (_columns.Count == 0 || _rawData.Count == 0)
            {
                await ShowMessageAsync("提示", "文件中没有找到有效数据");
                return;
            }

            // 显示列映射区域
            var columnMappingBorder = this.FindControl<Border>("ColumnMappingBorder");
            if (columnMappingBorder != null)
            {
                columnMappingBorder.IsVisible = true;
            }

            // 填充列选择下拉框
            PopulateColumnComboBoxes();

            // 自动映射列
            AutoMapColumns();

            // 显示预览
            UpdatePreview();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "加载文件数据失败");
            await ShowMessageAsync("错误", $"加载文件失败：{ex.Message}");
        }
    }

    private void LoadFromCsv(string filePath)
    {
        // 尝试多种编码读取CSV文件
        var encodings = new[] { Encoding.UTF8, Encoding.GetEncoding("GB2312"), Encoding.GetEncoding("GBK"), Encoding.Default };
        string[]? lines = null;
        
        foreach (var encoding in encodings)
        {
            try
            {
                lines = File.ReadAllLines(filePath, encoding);
                // 检查是否有乱码（简单检查）
                if (lines.Length > 0 && !HasGarbledText(lines[0]))
                {
                    break;
                }
            }
            catch
            {
                continue;
            }
        }
        
        if (lines == null || lines.Length == 0) return;

        // 第一行作为列名
        _columns = lines[0].Split(',').Select(c => c.Trim()).ToList();

        // 读取数据
        _rawData = new List<Dictionary<string, string>>();
        for (var i = 1; i < lines.Length; i++)
        {
            var values = lines[i].Split(',');
            var row = new Dictionary<string, string>();
            for (var j = 0; j < Math.Min(_columns.Count, values.Length); j++)
            {
                row[_columns[j]] = values[j].Trim();
            }
            _rawData.Add(row);
        }
    }

    private void LoadFromTxt(string filePath)
    {
        // 尝试多种编码读取TXT文件
        var encodings = new[] { Encoding.UTF8, Encoding.GetEncoding("GB2312"), Encoding.GetEncoding("GBK"), Encoding.Default };
        string[]? lines = null;
        
        foreach (var encoding in encodings)
        {
            try
            {
                lines = File.ReadAllLines(filePath, encoding);
                // 检查是否有乱码（简单检查）
                if (lines.Length > 0 && !HasGarbledText(lines[0]))
                {
                    break;
                }
            }
            catch
            {
                continue;
            }
        }
        
        if (lines == null || lines.Length == 0) return;

        // TXT文件只有姓名列
        _columns = new List<string> { "姓名" };
        _rawData = new List<Dictionary<string, string>>();

        for (var i = 0; i < lines.Length; i++)
        {
            var name = lines[i].Trim();
            if (!string.IsNullOrEmpty(name))
            {
                _rawData.Add(new Dictionary<string, string> { { "姓名", name } });
            }
        }
    }
    
    private void LoadFromExcel(string filePath)
    {
        try
        {
            // 使用 MiniExcel 读取 Excel 文件
            var rows = filePath.Query().ToList();
            if (rows.Count == 0) return;

            // 第一行作为列名
            var firstRow = (IDictionary<string, object>)rows[0];
            _columns = firstRow.Keys.ToList();

            // 读取数据
            _rawData = new List<Dictionary<string, string>>();
            for (var i = 1; i < rows.Count; i++)
            {
                var row = (IDictionary<string, object>)rows[i];
                var rowData = new Dictionary<string, string>();
                foreach (var key in _columns)
                {
                    rowData[key] = row.TryGetValue(key, out var value) ? value?.ToString() ?? "" : "";
                }
                _rawData.Add(rowData);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "读取Excel文件失败");
        }
    }
    
    private static bool HasGarbledText(string text)
    {
        // 简单的乱码检测：检查是否包含常见的乱码字符模式
        var garbledPatterns = new[] { "�", "??", "锟斤拷", "烫烫烫" };
        return garbledPatterns.Any(pattern => text.Contains(pattern));
    }

    private void LoadFromJson(string filePath)
    {
        var json = File.ReadAllText(filePath);
        var data = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        
        if (data == null) return;

        // JSON格式：姓名 -> 学生信息
        _columns = new List<string> { "姓名", "学号", "性别", "小组", "标签" };
        _rawData = new List<Dictionary<string, string>>();

        foreach (var (name, value) in data)
        {
            var row = new Dictionary<string, string> { { "姓名", name } };
            
            if (value is System.Text.Json.JsonElement element)
            {
                if (element.TryGetProperty("id", out var idProp))
                    row["学号"] = idProp.ToString();
                if (element.TryGetProperty("gender", out var genderProp))
                    row["性别"] = genderProp.ToString();
                if (element.TryGetProperty("group", out var groupProp))
                    row["小组"] = groupProp.ToString();
                if (element.TryGetProperty("tags", out var tagsProp))
                    row["标签"] = tagsProp.ToString();
            }
            
            _rawData.Add(row);
        }
    }

    private void PopulateColumnComboBoxes()
    {
        var comboBoxes = new[] 
        { 
            "IdColumnComboBox", 
            "NameColumnComboBox", 
            "GenderColumnComboBox", 
            "GroupColumnComboBox", 
            "TagsColumnComboBox" 
        };

        foreach (var comboBoxName in comboBoxes)
        {
            var comboBox = this.FindControl<ComboBox>(comboBoxName);
            if (comboBox == null) continue;

            comboBox.Items.Clear();
            
            // 添加"无"选项（对于可选列）
            if (comboBoxName != "IdColumnComboBox" && comboBoxName != "NameColumnComboBox")
            {
                comboBox.Items.Add("(无)");
            }

            // 添加所有列
            foreach (var column in _columns)
            {
                comboBox.Items.Add(column);
            }

            comboBox.SelectedIndex = 0;
        }
    }

    private void AutoMapColumns()
    {
        // 学号列关键词
        var idKeywords = new[] { "学号", "id", "编号", "座号" };
        AutoMapColumn("IdColumnComboBox", idKeywords);

        // 姓名列关键词
        var nameKeywords = new[] { "姓名", "name", "名字", "学生姓名" };
        AutoMapColumn("NameColumnComboBox", nameKeywords);

        // 性别列关键词
        var genderKeywords = new[] { "性别", "gender", "男女" };
        AutoMapColumn("GenderColumnComboBox", genderKeywords);

        // 小组列关键词
        var groupKeywords = new[] { "小组", "group", "组别", "分组" };
        AutoMapColumn("GroupColumnComboBox", groupKeywords);

        // 标签列关键词
        var tagsKeywords = new[] { "标签", "tag", "tags", "分类" };
        AutoMapColumn("TagsColumnComboBox", tagsKeywords);
    }

    private void AutoMapColumn(string comboBoxName, string[] keywords)
    {
        var comboBox = this.FindControl<ComboBox>(comboBoxName);
        if (comboBox == null) return;

        foreach (var column in _columns)
        {
            var columnLower = column.ToLower();
            foreach (var keyword in keywords)
            {
                if (columnLower.Contains(keyword.ToLower()))
                {
                    var index = comboBox.Items.IndexOf(column);
                    if (index >= 0)
                    {
                        comboBox.SelectedIndex = index;
                        return;
                    }
                }
            }
        }
    }

    private void UpdatePreview()
    {
        var previewBorder = this.FindControl<Border>("PreviewBorder");
        var previewDataGrid = this.FindControl<DataGrid>("PreviewDataGrid");
        var importButton = this.FindControl<Button>("ImportButton");

        if (previewBorder == null || previewDataGrid == null) return;

        // 获取选择的列
        var idColumn = GetSelectedColumn("IdColumnComboBox");
        var nameColumn = GetSelectedColumn("NameColumnComboBox");
        var genderColumn = GetSelectedColumn("GenderColumnComboBox");
        var groupColumn = GetSelectedColumn("GroupColumnComboBox");
        var tagsColumn = GetSelectedColumn("TagsColumnComboBox");

        // 检查必填列
        if (string.IsNullOrEmpty(nameColumn))
        {
            previewBorder.IsVisible = false;
            if (importButton != null) importButton.IsEnabled = false;
            return;
        }

        previewBorder.IsVisible = true;
        if (importButton != null) importButton.IsEnabled = !string.IsNullOrEmpty(_currentClassName);

        // 生成预览数据
        var previewItems = new ObservableCollection<PreviewItem>();
        var maxRows = Math.Min(5, _rawData.Count);

        for (var i = 0; i < maxRows; i++)
        {
            var row = _rawData[i];
            var item = new PreviewItem
            {
                Id = idColumn != null && row.TryGetValue(idColumn, out var id) ? id : (i + 1).ToString(),
                Name = row.TryGetValue(nameColumn, out var name) ? name : "",
                Gender = genderColumn != null && row.TryGetValue(genderColumn, out var gender) ? gender : "",
                Group = groupColumn != null && row.TryGetValue(groupColumn, out var group) ? group : "",
                TagsDisplay = tagsColumn != null && row.TryGetValue(tagsColumn, out var tags) ? tags : ""
            };
            previewItems.Add(item);
        }

        previewDataGrid.ItemsSource = previewItems;
    }

    private string? GetSelectedColumn(string comboBoxName)
    {
        var comboBox = this.FindControl<ComboBox>(comboBoxName);
        if (comboBox == null || comboBox.SelectedItem == null) return null;

        var selected = comboBox.SelectedItem.ToString();
        return selected == "(无)" ? null : selected;
    }

    private async void ImportButton_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(_currentClassName))
            {
                await ShowMessageAsync("错误", "请先选择要导入的班级");
                return;
            }

            var idColumn = GetSelectedColumn("IdColumnComboBox");
            var nameColumn = GetSelectedColumn("NameColumnComboBox");
            var genderColumn = GetSelectedColumn("GenderColumnComboBox");
            var groupColumn = GetSelectedColumn("GroupColumnComboBox");
            var tagsColumn = GetSelectedColumn("TagsColumnComboBox");

            if (string.IsNullOrEmpty(nameColumn))
            {
                await ShowMessageAsync("错误", "请选择姓名列");
                return;
            }

            // 转换数据
            var students = new List<StudentItem>();
            var id = 1;
            foreach (var row in _rawData)
            {
                var name = row.TryGetValue(nameColumn, out var n) ? n.Trim() : "";
                if (string.IsNullOrEmpty(name)) continue;

                var student = new StudentItem
                {
                    Id = int.TryParse(idColumn != null && row.TryGetValue(idColumn, out var idVal) ? idVal : "", out var parsedId) 
                        ? parsedId : id,
                    Name = name,
                    Gender = genderColumn != null && row.TryGetValue(genderColumn, out var gender) ? gender.Trim() : "",
                    Group = groupColumn != null && row.TryGetValue(groupColumn, out var group) ? group.Trim() : "",
                    Tags = ParseTags(tagsColumn != null && row.TryGetValue(tagsColumn, out var tags) ? tags : "")
                };
                students.Add(student);
                id++;
            }

            if (students.Count == 0)
            {
                await ShowMessageAsync("提示", "没有找到有效的学生数据");
                return;
            }

            // 导入数据
            var count = _service.ImportStudents(_currentClassName, _selectedFilePath!, true);
            
            await ShowMessageAsync("成功", $"成功导入 {count} 名学生到班级 {_currentClassName}");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "导入学生失败");
            await ShowMessageAsync("错误", $"导入失败：{ex.Message}");
        }
    }

    private static List<string> ParseTags(string tagsText)
    {
        if (string.IsNullOrWhiteSpace(tagsText)) return new List<string>();
        
        var separators = new[] { "，", ",", "；", ";", "|", "/", "\\", "\n", "\t" };
        foreach (var sep in separators)
        {
            tagsText = tagsText.Replace(sep, " ");
        }

        return tagsText.Split(' ')
            .Select(t => t.Trim())
            .Where(t => !string.IsNullOrEmpty(t))
            .Distinct()
            .ToList();
    }

    private void CancelButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // 重置页面
        _selectedFilePath = null;
        _columns.Clear();
        _rawData.Clear();

        var filePathTextBlock = this.FindControl<TextBlock>("FilePathTextBlock");
        if (filePathTextBlock != null) filePathTextBlock.Text = "未选择文件";

        var columnMappingBorder = this.FindControl<Border>("ColumnMappingBorder");
        if (columnMappingBorder != null) columnMappingBorder.IsVisible = false;

        var previewBorder = this.FindControl<Border>("PreviewBorder");
        if (previewBorder != null) previewBorder.IsVisible = false;

        var importButton = this.FindControl<Button>("ImportButton");
        if (importButton != null) importButton.IsEnabled = false;
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
/// 预览数据项
/// </summary>
public class PreviewItem
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Group { get; set; } = string.Empty;
    public string TagsDisplay { get; set; } = string.Empty;
}
