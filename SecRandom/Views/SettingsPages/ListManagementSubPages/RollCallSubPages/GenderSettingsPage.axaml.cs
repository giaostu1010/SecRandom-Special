using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using FluentAvalonia.UI.Controls;
using Microsoft.Extensions.Logging;
using SecRandom.Core.Attributes;
using SecRandom.Core.Enums;
using SecRandom.Core.Models;
using SecRandom.Core.Services;

namespace SecRandom.Views.SettingsPages.ListManagementSubPages.RollCallSubPages;

[PageInfo("settings.listManagement.rollCall.genderSettings", "\uE7C3", "settings.listManagement", PageLocation.Top, true)]
public partial class GenderSettingsPage : UserControl
{
    private readonly RollCallListService _service;
    private readonly ILogger<GenderSettingsPage>? _logger;
    private string? _currentClassName;
    private ObservableCollection<StudentItem> _students = [];
    private bool _saved = false;

    public GenderSettingsPage()
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
        if (string.IsNullOrEmpty(_currentClassName)) return;

        try
        {
            var students = _service.GetStudentList(_currentClassName);
            _students.Clear();
            foreach (var student in students)
            {
                _students.Add(student);
            }

            var dataGrid = this.FindControl<DataGrid>("StudentsDataGrid");
            if (dataGrid != null)
            {
                dataGrid.ItemsSource = _students;
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "加载学生列表失败");
        }
    }

    private async void SaveButton_OnClick(object? sender, RoutedEventArgs e)
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
                await ShowMessageAsync("提示", "没有学生数据需要保存");
                return;
            }

            // 保存学生数据
            if (_service.SaveStudents(_currentClassName, _students.ToList()))
            {
                _saved = true;
                await ShowMessageAsync("成功", $"成功保存 {_students.Count} 名学生的性别信息");
            }
            else
            {
                await ShowMessageAsync("错误", "保存失败");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "保存性别信息失败");
            await ShowMessageAsync("错误", $"保存失败：{ex.Message}");
        }
    }

    private async void CancelButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (!_saved)
        {
            var result = await ShowConfirmAsync("未保存的更改", "您有未保存的更改，是否放弃？");
            if (result != ContentDialogResult.Primary)
            {
                return;
            }
        }

        // 重新加载数据
        LoadStudents();
        _saved = false;
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

    private static async Task<ContentDialogResult> ShowConfirmAsync(string title, string message)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            PrimaryButtonText = "确定",
            SecondaryButtonText = "取消"
        };
        return await dialog.ShowAsync();
    }
}
