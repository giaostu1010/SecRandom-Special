using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SecRandom.Services.Config;

namespace SecRandom.ViewModels.ListManagementSubPages.RollCallSubPages;

public partial class NameSettingsViewModel : ObservableObject
{
    private readonly MainConfigHandler _configHandler;
    private readonly ILogger<NameSettingsViewModel>? _logger;

    [ObservableProperty]
    private StudentItem? _selectedStudent;

    [ObservableProperty]
    private string _newName = string.Empty;

    public ObservableCollection<StudentItem> Students { get; } = new();

    public NameSettingsViewModel(MainConfigHandler configHandler, ILogger<NameSettingsViewModel>? logger = null)
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
    private void UpdateName()
    {
        if (SelectedStudent == null || string.IsNullOrEmpty(NewName))
        {
            return;
        }

        try
        {
            SelectedStudent.Name = NewName;
            // TODO: 保存到服务
            _logger?.LogInformation("学生姓名已更新: {Name}", NewName);
        }
        catch (System.Exception ex)
        {
            _logger?.LogError(ex, "更新学生姓名失败");
        }
    }

    [RelayCommand]
    private void Save()
    {
        try
        {
            // TODO: 保存所有更改
            _logger?.LogInformation("姓名设置已保存");
        }
        catch (System.Exception ex)
        {
            _logger?.LogError(ex, "保存姓名设置失败");
        }
    }
}

public partial class StudentItem : ObservableObject
{
    [ObservableProperty]
    private string _id = string.Empty;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _gender = string.Empty;

    [ObservableProperty]
    private string _group = string.Empty;

    [ObservableProperty]
    private string _tags = string.Empty;
}
