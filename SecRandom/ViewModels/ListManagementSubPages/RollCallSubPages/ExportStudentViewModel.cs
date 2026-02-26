using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SecRandom.Services.Config;

namespace SecRandom.ViewModels.ListManagementSubPages.RollCallSubPages;

public partial class ExportStudentViewModel : ObservableObject
{
    private readonly MainConfigHandler _configHandler;
    private readonly ILogger<ExportStudentViewModel>? _logger;

    [ObservableProperty]
    private string _savePath = string.Empty;

    [ObservableProperty]
    private int _selectedFormatIndex;

    [ObservableProperty]
    private string _selectedClass = string.Empty;

    public ObservableCollection<string> Classes { get; } = [];

    public ExportStudentViewModel(MainConfigHandler configHandler, ILogger<ExportStudentViewModel>? logger = null)
    {
        _configHandler = configHandler;
        _logger = logger;
        LoadClasses();
    }

    private void LoadClasses()
    {
        try
        {
            // TODO: 从服务加载班级列表
            _logger?.LogInformation("班级列表已加载");
        }
        catch (System.Exception ex)
        {
            _logger?.LogError(ex, "加载班级列表失败");
        }
    }

    [RelayCommand]
    private void Browse()
    {
        // TODO: 打开保存文件对话框
        _logger?.LogInformation("浏览保存位置");
    }

    [RelayCommand]
    private void Export()
    {
        if (string.IsNullOrEmpty(SelectedClass))
        {
            _logger?.LogWarning("请先选择班级");
            return;
        }

        if (string.IsNullOrEmpty(SavePath))
        {
            _logger?.LogWarning("请选择保存位置");
            return;
        }

        try
        {
            // TODO: 执行导出操作
            _logger?.LogInformation("开始导出学生名单");
        }
        catch (System.Exception ex)
        {
            _logger?.LogError(ex, "导出学生名单失败");
        }
    }
}
