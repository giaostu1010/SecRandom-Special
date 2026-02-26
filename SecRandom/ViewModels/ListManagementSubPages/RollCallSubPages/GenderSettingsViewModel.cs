using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SecRandom.Services.Config;

namespace SecRandom.ViewModels.ListManagementSubPages.RollCallSubPages;

public partial class GenderSettingsViewModel : ObservableObject
{
    private readonly MainConfigHandler _configHandler;
    private readonly ILogger<GenderSettingsViewModel>? _logger;

    [ObservableProperty]
    private StudentItem? _selectedStudent;

    [ObservableProperty]
    private int _selectedGenderIndex;

    public ObservableCollection<StudentItem> Students { get; } = [];

    public GenderSettingsViewModel(MainConfigHandler configHandler, ILogger<GenderSettingsViewModel>? logger = null)
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
    private void UpdateGender()
    {
        if (SelectedStudent == null)
        {
            return;
        }

        try
        {
            var gender = SelectedGenderIndex switch
            {
                0 => "男",
                1 => "女",
                _ => "未知"
            };
            SelectedStudent.Gender = gender;
            // TODO: 保存到服务
            _logger?.LogInformation("学生性别已更新: {Gender}", gender);
        }
        catch (System.Exception ex)
        {
            _logger?.LogError(ex, "更新学生性别失败");
        }
    }

    [RelayCommand]
    private void Save()
    {
        try
        {
            // TODO: 保存所有更改
            _logger?.LogInformation("性别设置已保存");
        }
        catch (System.Exception ex)
        {
            _logger?.LogError(ex, "保存性别设置失败");
        }
    }
}
