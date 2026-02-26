using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SecRandom.Services.Config;

namespace SecRandom.ViewModels.ListManagementSubPages.LotterySubPages;

public partial class SetPoolNameViewModel : ObservableObject
{
    private readonly MainConfigHandler _configHandler;
    private readonly ILogger<SetPoolNameViewModel>? _logger;
    private List<string> _initialPoolNames = [];

    [ObservableProperty]
    private string _poolNamesText = string.Empty;

    [ObservableProperty]
    private bool _hasUnsavedChanges;

    public SetPoolNameViewModel(MainConfigHandler configHandler, ILogger<SetPoolNameViewModel>? logger = null)
    {
        _configHandler = configHandler;
        _logger = logger;
        LoadPoolNames();
    }

    private void LoadPoolNames()
    {
        try
        {
            // TODO: 从服务加载奖池名称
            // _initialPoolNames = _lotteryListService.PoolNames.ToList();
            // PoolNamesText = string.Join("\n", _initialPoolNames);
            _logger?.LogInformation("奖池名称已加载");
        }
        catch (System.Exception ex)
        {
            _logger?.LogError(ex, "加载奖池名称失败");
        }
    }

    [RelayCommand]
    private void Save()
    {
        try
        {
            var poolNames = PoolNamesText.Split('\n')
                .Select(n => n.Trim())
                .Where(n => !string.IsNullOrEmpty(n))
                .ToList();

            // TODO: 保存奖池名称到服务
            // _lotteryListService.SavePoolNames(poolNames);

            _initialPoolNames = poolNames;
            HasUnsavedChanges = false;
            _logger?.LogInformation("奖池名称已保存");
        }
        catch (System.Exception ex)
        {
            _logger?.LogError(ex, "保存奖池名称失败");
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        PoolNamesText = string.Join("\n", _initialPoolNames);
        HasUnsavedChanges = false;
    }

    partial void OnPoolNamesTextChanged(string value)
    {
        HasUnsavedChanges = value != string.Join("\n", _initialPoolNames);
    }
}
