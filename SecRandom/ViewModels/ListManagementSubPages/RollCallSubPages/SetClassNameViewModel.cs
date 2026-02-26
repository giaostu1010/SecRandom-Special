using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SecRandom.Services.Config;

namespace SecRandom.ViewModels.ListManagementSubPages.RollCallSubPages;

public partial class SetClassNameViewModel : ObservableObject
{
    private readonly MainConfigHandler _configHandler;
    private readonly ILogger<SetClassNameViewModel>? _logger;
    private List<string> _initialClassNames = [];

    [ObservableProperty]
    private string _classNamesText = string.Empty;

    [ObservableProperty]
    private bool _hasUnsavedChanges;

    public SetClassNameViewModel(MainConfigHandler configHandler, ILogger<SetClassNameViewModel>? logger = null)
    {
        _configHandler = configHandler;
        _logger = logger;
        LoadClassNames();
    }

    private void LoadClassNames()
    {
        try
        {
            // TODO: 从服务加载班级名称
            // _initialClassNames = _rollCallListService.ClassNames.ToList();
            // ClassNamesText = string.Join("\n", _initialClassNames);
            _logger?.LogInformation("班级名称已加载");
        }
        catch (System.Exception ex)
        {
            _logger?.LogError(ex, "加载班级名称失败");
        }
    }

    [RelayCommand]
    private void Save()
    {
        try
        {
            var classNames = ClassNamesText.Split('\n')
                .Select(n => n.Trim())
                .Where(n => !string.IsNullOrEmpty(n))
                .ToList();

            // TODO: 保存班级名称到服务
            // _rollCallListService.SaveClassNames(classNames);

            _initialClassNames = classNames;
            HasUnsavedChanges = false;
            _logger?.LogInformation("班级名称已保存");
        }
        catch (System.Exception ex)
        {
            _logger?.LogError(ex, "保存班级名称失败");
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        ClassNamesText = string.Join("\n", _initialClassNames);
        HasUnsavedChanges = false;
    }

    partial void OnClassNamesTextChanged(string value)
    {
        HasUnsavedChanges = value != string.Join("\n", _initialClassNames);
    }
}
