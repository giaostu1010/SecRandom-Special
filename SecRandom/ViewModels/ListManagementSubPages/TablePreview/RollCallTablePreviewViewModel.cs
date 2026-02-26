using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SecRandom.Services.Config;

namespace SecRandom.ViewModels.ListManagementSubPages.TablePreview;

public partial class RollCallTablePreviewViewModel : ObservableObject
{
    private readonly MainConfigHandler _configHandler;
    private readonly ILogger<RollCallTablePreviewViewModel>? _logger;

    [ObservableProperty]
    private string _selectedClass = string.Empty;

    [ObservableProperty]
    private int _totalStudents;

    public ObservableCollection<string> Classes { get; } = [];

    public ObservableCollection<StudentTableRow> Students { get; } = [];

    public RollCallTablePreviewViewModel(MainConfigHandler configHandler, ILogger<RollCallTablePreviewViewModel>? logger = null)
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

    partial void OnSelectedClassChanged(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            LoadStudents(value);
        }
    }

    private void LoadStudents(string className)
    {
        try
        {
            Students.Clear();
            // TODO: 从服务加载学生列表
            TotalStudents = Students.Count;
            _logger?.LogInformation("学生列表已加载: {Class}", className);
        }
        catch (System.Exception ex)
        {
            _logger?.LogError(ex, "加载学生列表失败");
        }
    }

    [RelayCommand]
    private void Refresh()
    {
        if (!string.IsNullOrEmpty(SelectedClass))
        {
            LoadStudents(SelectedClass);
        }
    }
}

public partial class StudentTableRow : ObservableObject
{
    [ObservableProperty]
    private string _studentId = string.Empty;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _gender = string.Empty;

    [ObservableProperty]
    private string _group = string.Empty;

    [ObservableProperty]
    private string _tags = string.Empty;

    [ObservableProperty]
    private bool _isEnabled = true;
}
