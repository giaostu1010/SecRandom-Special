using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SecRandom.Services.Config;

namespace SecRandom.ViewModels.ListManagementSubPages.ListSettingsSubPage;

public partial class RollCallListSettingsViewModel : ObservableObject
{
    private readonly MainConfigHandler _configHandler;
    private readonly ILogger<RollCallListSettingsViewModel>? _logger;

    public RollCallListSettingsViewModel(MainConfigHandler configHandler, ILogger<RollCallListSettingsViewModel>? logger = null)
    {
        _configHandler = configHandler;
        _logger = logger;
    }

    [RelayCommand]
    private void OpenPreviewTable()
    {
        // TODO: 导航到预览表格页面
        _logger?.LogInformation("打开预览表格");
    }

    [RelayCommand]
    private void OpenSetClassName()
    {
        // TODO: 导航到设置班级名称页面
        _logger?.LogInformation("打开设置班级名称");
    }

    [RelayCommand]
    private void OpenImportStudent()
    {
        // TODO: 导航到导入学生页面
        _logger?.LogInformation("打开导入学生");
    }

    [RelayCommand]
    private void OpenNameSettings()
    {
        // TODO: 导航到姓名设置页面
        _logger?.LogInformation("打开姓名设置");
    }

    [RelayCommand]
    private void OpenGenderSettings()
    {
        // TODO: 导航到性别设置页面
        _logger?.LogInformation("打开性别设置");
    }

    [RelayCommand]
    private void OpenGroupSettings()
    {
        // TODO: 导航到小组设置页面
        _logger?.LogInformation("打开小组设置");
    }

    [RelayCommand]
    private void OpenTagSettings()
    {
        // TODO: 导航到标签设置页面
        _logger?.LogInformation("打开标签设置");
    }

    [RelayCommand]
    private void OpenExportStudent()
    {
        // TODO: 导航到导出学生页面
        _logger?.LogInformation("打开导出学生");
    }
}
