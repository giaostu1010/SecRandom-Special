using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SecRandom.Services.Config;

namespace SecRandom.ViewModels.ListManagementSubPages.RollCallSubPages;

public partial class GroupSettingsViewModel : ObservableObject
{
    private readonly MainConfigHandler _configHandler;
    private readonly ILogger<GroupSettingsViewModel>? _logger;

    [ObservableProperty]
    private StudentItem? _selectedStudent;

    [ObservableProperty]
    private string _newGroup = string.Empty;

    public ObservableCollection<StudentItem> Students { get; } = new();

    public GroupSettingsViewModel(MainConfigHandler configHandler, ILogger<GroupSettingsViewModel>? logger = null)
    {
        _configHandler = configHandler;
        _logger = logger;
        LoadStudents();
    }

    private void LoadStudents()
    {
        try
        {
            // TODO: 从服务加载学生列表
            _logger?.LogInformation("学生列表已加载");
        }
        catch (System.Exception ex)
        {
            _logger?.LogError(ex, "加载学生列表失败");
        }
    }

    [RelayCommand]
    private void UpdateGroup()
    {
        if (SelectedStudent == null || string.IsNullOrEmpty(NewGroup))
        {
            return;
        }

        try
        {
            SelectedStudent.Group = NewGroup;
            // TODO: 保存到服务
            _logger?.LogInformation("学生小组已更新: {Group}", NewGroup);
        }
        catch (System.Exception ex)
        {
            _logger?.LogError(ex, "更新学生小组失败");
        }
    }

    [RelayCommand]
    private void Save()
    {
        try
        {
            // TODO: 保存所有更改
            _logger?.LogInformation("小组设置已保存");
        }
        catch (System.Exception ex)
        {
            _logger?.LogError(ex, "保存小组设置失败");
        }
    }
}
