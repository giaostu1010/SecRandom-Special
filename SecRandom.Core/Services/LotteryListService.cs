using System.Collections.ObjectModel;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SecRandom.Core.Models;

namespace SecRandom.Core.Services;

/// <summary>
/// 抽奖名单服务 - 管理奖池和奖品数据
/// </summary>
public class LotteryListService
{
    private readonly ILogger<LotteryListService>? _logger;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    /// <summary>
    /// 奖池名称列表
    /// </summary>
    public ObservableCollection<string> PoolNames { get; } = [];

    /// <summary>
    /// 当前选中的奖池名称
    /// </summary>
    public string? CurrentPoolName { get; set; }

    /// <summary>
    /// 当前奖池的奖品列表
    /// </summary>
    public ObservableCollection<PrizeItem> CurrentPrizes { get; } = [];

    public LotteryListService(ILogger<LotteryListService>? logger = null)
    {
        _logger = logger;
    }

    /// <summary>
    /// 获取奖池数据目录路径
    /// </summary>
    private static string GetLotteryListDirectory()
    {
        return Utils.GetFilePath("list", "lottery_list");
    }

    /// <summary>
    /// 获取奖池文件路径
    /// </summary>
    private static string GetPoolFilePath(string poolName)
    {
        return Path.Combine(GetLotteryListDirectory(), $"{poolName}.json");
    }

    /// <summary>
    /// 刷新奖池名称列表
    /// </summary>
    public void RefreshPoolNames()
    {
        try
        {
            var directory = GetLotteryListDirectory();
            
            if (!Directory.Exists(directory))
            {
                _logger?.LogWarning("奖池文件夹不存在: {Directory}", directory);
                Directory.CreateDirectory(directory);
                PoolNames.Clear();
                return;
            }

            var files = Directory.GetFiles(directory, "*.json");
            var poolNames = files
                .Select(Path.GetFileNameWithoutExtension)
                .Where(name => !string.IsNullOrEmpty(name))
                .OrderBy(name => name)
                .ToList();

            PoolNames.Clear();
            foreach (var name in poolNames)
            {
                PoolNames.Add(name!);
            }

            _logger?.LogDebug("找到 {Count} 个奖池: {Pools}", poolNames.Count, string.Join(", ", poolNames));
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "获取奖池列表失败");
        }
    }

    /// <summary>
    /// 获取指定奖池的奖品列表
    /// </summary>
    /// <param name="poolName">奖池名称</param>
    /// <returns>奖品列表</returns>
    public List<PrizeItem> GetPoolList(string poolName)
    {
        try
        {
            var filePath = GetPoolFilePath(poolName);

            if (!File.Exists(filePath))
            {
                _logger?.LogWarning("奖池文件不存在: {FilePath}", filePath);
                return [];
            }

            var json = File.ReadAllText(filePath);
            var poolData = JsonSerializer.Deserialize<Dictionary<string, PrizeItemData>>(json, _jsonOptions);

            if (poolData == null)
            {
                return [];
            }

            var prizes = new List<PrizeItem>();
            var id = 1;
            foreach (var (name, info) in poolData)
            {
                var prize = new PrizeItem
                {
                    Id = info.Id > 0 ? info.Id : id,
                    Name = name,
                    Weight = info.Weight > 0 ? info.Weight : 1.0,
                    Exist = info.Exist,
                    Count = Math.Max(0, info.Count),
                    Tags = NormalizeTags(info.Tags)
                };
                prizes.Add(prize);
                id++;
            }

            // 按ID排序
            prizes = prizes.OrderBy(p => p.Id).ToList();

            _logger?.LogDebug("奖池 {PoolName} 共有 {Count} 个奖品", poolName, prizes.Count);
            return prizes;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "获取奖池列表失败: {PoolName}", poolName);
            return [];
        }
    }

    /// <summary>
    /// 刷新当前奖池的奖品列表
    /// </summary>
    public void RefreshCurrentPrizes()
    {
        if (string.IsNullOrEmpty(CurrentPoolName))
        {
            CurrentPrizes.Clear();
            return;
        }

        var prizes = GetPoolList(CurrentPoolName);
        CurrentPrizes.Clear();
        foreach (var prize in prizes)
        {
            CurrentPrizes.Add(prize);
        }
    }

    /// <summary>
    /// 创建新奖池
    /// </summary>
    /// <param name="poolName">奖池名称</param>
    /// <returns>是否成功</returns>
    public bool CreatePool(string poolName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(poolName))
            {
                _logger?.LogWarning("奖池名称不能为空");
                return false;
            }

            var filePath = GetPoolFilePath(poolName);

            if (File.Exists(filePath))
            {
                _logger?.LogWarning("奖池已存在: {PoolName}", poolName);
                return false;
            }

            // 创建空奖池文件
            var emptyData = new Dictionary<string, PrizeItemData>();
            var json = JsonSerializer.Serialize(emptyData, _jsonOptions);
            File.WriteAllText(filePath, json);

            _logger?.LogInformation("创建奖池成功: {PoolName}", poolName);
            RefreshPoolNames();
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "创建奖池失败: {PoolName}", poolName);
            return false;
        }
    }

    /// <summary>
    /// 删除奖池
    /// </summary>
    /// <param name="poolName">奖池名称</param>
    /// <returns>是否成功</returns>
    public bool DeletePool(string poolName)
    {
        try
        {
            var filePath = GetPoolFilePath(poolName);

            if (!File.Exists(filePath))
            {
                _logger?.LogWarning("奖池不存在: {PoolName}", poolName);
                return false;
            }

            File.Delete(filePath);

            _logger?.LogInformation("删除奖池成功: {PoolName}", poolName);
            RefreshPoolNames();
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "删除奖池失败: {PoolName}", poolName);
            return false;
        }
    }

    /// <summary>
    /// 重命名奖池
    /// </summary>
    /// <param name="oldName">旧名称</param>
    /// <param name="newName">新名称</param>
    /// <returns>是否成功</returns>
    public bool RenamePool(string oldName, string newName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(newName))
            {
                _logger?.LogWarning("新奖池名称不能为空");
                return false;
            }

            var oldPath = GetPoolFilePath(oldName);
            var newPath = GetPoolFilePath(newName);

            if (!File.Exists(oldPath))
            {
                _logger?.LogWarning("奖池不存在: {OldName}", oldName);
                return false;
            }

            if (File.Exists(newPath))
            {
                _logger?.LogWarning("目标奖池已存在: {NewName}", newName);
                return false;
            }

            File.Move(oldPath, newPath);

            _logger?.LogInformation("重命名奖池成功: {OldName} -> {NewName}", oldName, newName);
            RefreshPoolNames();
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "重命名奖池失败: {OldName} -> {NewName}", oldName, newName);
            return false;
        }
    }

    /// <summary>
    /// 保存奖品数据
    /// </summary>
    /// <param name="poolName">奖池名称</param>
    /// <param name="prizes">奖品列表</param>
    /// <returns>是否成功</returns>
    public bool SavePrizes(string poolName, List<PrizeItem> prizes)
    {
        try
        {
            var filePath = GetPoolFilePath(poolName);

            // 转换为字典格式（名称 -> 数据）
            var poolData = new Dictionary<string, PrizeItemData>();
            foreach (var prize in prizes)
            {
                poolData[prize.Name] = new PrizeItemData
                {
                    Id = prize.Id,
                    Weight = prize.Weight,
                    Exist = prize.Exist,
                    Count = prize.Count,
                    Tags = prize.Tags
                };
            }

            var json = JsonSerializer.Serialize(poolData, _jsonOptions);
            File.WriteAllText(filePath, json);

            _logger?.LogDebug("保存奖池数据成功: {PoolName}", poolName);
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "保存奖池数据失败: {PoolName}", poolName);
            return false;
        }
    }

    /// <summary>
    /// 更新单个奖品
    /// </summary>
    /// <param name="poolName">奖池名称</param>
    /// <param name="prizeId">奖品ID</param>
    /// <param name="updatedPrize">更新后的奖品数据</param>
    /// <returns>是否成功</returns>
    public bool UpdatePrize(string poolName, int prizeId, PrizeItem updatedPrize)
    {
        try
        {
            var filePath = GetPoolFilePath(poolName);

            if (!File.Exists(filePath))
            {
                _logger?.LogWarning("奖池文件不存在: {FilePath}", filePath);
                return false;
            }

            var json = File.ReadAllText(filePath);
            var poolData = JsonSerializer.Deserialize<Dictionary<string, PrizeItemData>>(json, _jsonOptions);

            if (poolData == null)
            {
                return false;
            }

            // 查找要更新的奖品
            string? keyToUpdate = null;
            foreach (var (key, value) in poolData)
            {
                if (value.Id == prizeId)
                {
                    keyToUpdate = key;
                    break;
                }
            }

            if (keyToUpdate == null)
            {
                _logger?.LogWarning("未找到奖品ID: {PrizeId}", prizeId);
                return false;
            }

            // 如果名称改变，需要重新创建键
            if (keyToUpdate != updatedPrize.Name)
            {
                poolData.Remove(keyToUpdate);
                poolData[updatedPrize.Name] = new PrizeItemData
                {
                    Id = updatedPrize.Id,
                    Weight = updatedPrize.Weight,
                    Exist = updatedPrize.Exist,
                    Count = updatedPrize.Count,
                    Tags = updatedPrize.Tags
                };
            }
            else
            {
                poolData[keyToUpdate] = new PrizeItemData
                {
                    Id = updatedPrize.Id,
                    Weight = updatedPrize.Weight,
                    Exist = updatedPrize.Exist,
                    Count = updatedPrize.Count,
                    Tags = updatedPrize.Tags
                };
            }

            var newJson = JsonSerializer.Serialize(poolData, _jsonOptions);
            File.WriteAllText(filePath, newJson);

            _logger?.LogDebug("更新奖品成功: {PoolName} - {PrizeId}", poolName, prizeId);
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "更新奖品失败: {PoolName} - {PrizeId}", poolName, prizeId);
            return false;
        }
    }

    /// <summary>
    /// 添加奖品
    /// </summary>
    /// <param name="poolName">奖池名称</param>
    /// <param name="prize">奖品</param>
    /// <returns>是否成功</returns>
    public bool AddPrize(string poolName, PrizeItem prize)
    {
        try
        {
            var filePath = GetPoolFilePath(poolName);

            if (!File.Exists(filePath))
            {
                _logger?.LogWarning("奖池文件不存在: {FilePath}", filePath);
                return false;
            }

            var json = File.ReadAllText(filePath);
            var poolData = JsonSerializer.Deserialize<Dictionary<string, PrizeItemData>>(json, _jsonOptions)
                          ?? new Dictionary<string, PrizeItemData>();

            // 检查名称是否重复
            if (poolData.ContainsKey(prize.Name))
            {
                _logger?.LogWarning("奖品名称已存在: {PrizeName}", prize.Name);
                return false;
            }

            // 计算新ID
            var maxId = poolData.Values.Max(p => p.Id);
            prize.Id = maxId + 1;

            poolData[prize.Name] = new PrizeItemData
            {
                Id = prize.Id,
                Weight = prize.Weight,
                Exist = prize.Exist,
                Count = prize.Count,
                Tags = prize.Tags
            };

            var newJson = JsonSerializer.Serialize(poolData, _jsonOptions);
            File.WriteAllText(filePath, newJson);

            _logger?.LogDebug("添加奖品成功: {PoolName} - {PrizeName}", poolName, prize.Name);
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "添加奖品失败: {PoolName} - {PrizeName}", poolName, prize.Name);
            return false;
        }
    }

    /// <summary>
    /// 删除奖品
    /// </summary>
    /// <param name="poolName">奖池名称</param>
    /// <param name="prizeId">奖品ID</param>
    /// <returns>是否成功</returns>
    public bool DeletePrize(string poolName, int prizeId)
    {
        try
        {
            var filePath = GetPoolFilePath(poolName);

            if (!File.Exists(filePath))
            {
                _logger?.LogWarning("奖池文件不存在: {FilePath}", filePath);
                return false;
            }

            var json = File.ReadAllText(filePath);
            var poolData = JsonSerializer.Deserialize<Dictionary<string, PrizeItemData>>(json, _jsonOptions);

            if (poolData == null)
            {
                return false;
            }

            // 查找要删除的奖品
            string? keyToDelete = null;
            foreach (var (key, value) in poolData)
            {
                if (value.Id == prizeId)
                {
                    keyToDelete = key;
                    break;
                }
            }

            if (keyToDelete == null)
            {
                _logger?.LogWarning("未找到奖品ID: {PrizeId}", prizeId);
                return false;
            }

            poolData.Remove(keyToDelete);

            var newJson = JsonSerializer.Serialize(poolData, _jsonOptions);
            File.WriteAllText(filePath, newJson);

            _logger?.LogDebug("删除奖品成功: {PoolName} - {PrizeId}", poolName, prizeId);
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "删除奖品失败: {PoolName} - {PrizeId}", poolName, prizeId);
            return false;
        }
    }

    /// <summary>
    /// 从文件导入奖品
    /// </summary>
    /// <param name="poolName">奖池名称</param>
    /// <param name="filePath">导入文件路径</param>
    /// <param name="clearExisting">是否清除现有数据</param>
    /// <returns>导入的奖品数量</returns>
    public int ImportPrizes(string poolName, string filePath, bool clearExisting = false)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                _logger?.LogWarning("导入文件不存在: {FilePath}", filePath);
                return 0;
            }

            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            List<PrizeItem> importedPrizes;

            switch (extension)
            {
                case ".json":
                    importedPrizes = ImportFromJson(filePath);
                    break;
                case ".csv":
                    importedPrizes = ImportFromCsv(filePath);
                    break;
                case ".txt":
                    importedPrizes = ImportFromTxt(filePath);
                    break;
                default:
                    _logger?.LogWarning("不支持的文件格式: {Extension}", extension);
                    return 0;
            }

            if (importedPrizes.Count == 0)
            {
                return 0;
            }

            var poolFilePath = GetPoolFilePath(poolName);
            Dictionary<string, PrizeItemData> poolData;

            if (clearExisting || !File.Exists(poolFilePath))
            {
                poolData = new Dictionary<string, PrizeItemData>();
            }
            else
            {
                var json = File.ReadAllText(poolFilePath);
                poolData = JsonSerializer.Deserialize<Dictionary<string, PrizeItemData>>(json, _jsonOptions)
                          ?? new Dictionary<string, PrizeItemData>();
            }

            // 计算起始ID
            var maxId = poolData.Count > 0 ? poolData.Values.Max(p => p.Id) : 0;

            // 处理重复名称
            var uniquePrizes = MakeUniqueNames(importedPrizes, poolData.Keys.ToHashSet());

            foreach (var prize in uniquePrizes)
            {
                maxId++;
                prize.Id = maxId;
                poolData[prize.Name] = new PrizeItemData
                {
                    Id = prize.Id,
                    Weight = prize.Weight,
                    Exist = prize.Exist,
                    Count = prize.Count,
                    Tags = prize.Tags
                };
            }

            var newJson = JsonSerializer.Serialize(poolData, _jsonOptions);
            File.WriteAllText(poolFilePath, newJson);

            _logger?.LogInformation("导入奖品成功: {PoolName} - {Count} 个", poolName, uniquePrizes.Count);
            return uniquePrizes.Count;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "导入奖品失败: {PoolName}", poolName);
            return 0;
        }
    }

    /// <summary>
    /// 导出奖品到文件
    /// </summary>
    /// <param name="poolName">奖池名称</param>
    /// <param name="filePath">导出文件路径</param>
    /// <returns>是否成功</returns>
    public bool ExportPrizes(string poolName, string filePath)
    {
        try
        {
            var prizes = GetPoolList(poolName);
            if (prizes.Count == 0)
            {
                _logger?.LogWarning("奖池没有奖品数据: {PoolName}", poolName);
                return false;
            }

            var extension = Path.GetExtension(filePath).ToLowerInvariant();

            switch (extension)
            {
                case ".json":
                    ExportToJson(prizes, filePath);
                    break;
                case ".csv":
                    ExportToCsv(prizes, filePath);
                    break;
                case ".txt":
                    ExportToTxt(prizes, filePath);
                    break;
                default:
                    _logger?.LogWarning("不支持的导出格式: {Extension}", extension);
                    return false;
            }

            _logger?.LogInformation("导出奖品成功: {FilePath}", filePath);
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "导出奖品失败: {PoolName}", poolName);
            return false;
        }
    }

    #region 私有辅助方法

    /// <summary>
    /// 规范化标签
    /// </summary>
    private static List<string> NormalizeTags(object? value)
    {
        if (value == null)
        {
            return [];
        }

        if (value is List<string> tags)
        {
            return tags.Where(t => !string.IsNullOrWhiteSpace(t)).Distinct().ToList();
        }

        if (value is string str)
        {
            if (string.IsNullOrWhiteSpace(str) || str.ToLower() == "nan")
            {
                return [];
            }

            // 替换各种分隔符为空格
            foreach (var sep in new[] { "，", ",", "；", ";", "|", "/", "\\", "\n", "\t" })
            {
                str = str.Replace(sep, " ");
            }

            return str.Split(' ')
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .Distinct()
                .ToList();
        }

        return [];
    }

    /// <summary>
    /// 使名称唯一
    /// </summary>
    private static List<PrizeItem> MakeUniqueNames(List<PrizeItem> prizes, HashSet<string> existingNames)
    {
        var result = new List<PrizeItem>();
        var usedNames = new HashSet<string>(existingNames);
        var counters = new Dictionary<string, int>();

        foreach (var prize in prizes)
        {
            var name = prize.Name;

            if (!usedNames.Contains(name))
            {
                usedNames.Add(name);
                result.Add(prize);
                continue;
            }

            // 名称重复，添加后缀
            var index = counters.GetValueOrDefault(name, 0) + 1;
            var candidate = $"{name}_{index}";

            while (usedNames.Contains(candidate))
            {
                index++;
                candidate = $"{name}_{index}";
            }

            counters[name] = index;
            usedNames.Add(candidate);
            prize.Name = candidate;
            result.Add(prize);
        }

        return result;
    }

    private List<PrizeItem> ImportFromJson(string filePath)
    {
        try
        {
            var json = File.ReadAllText(filePath);
            var data = JsonSerializer.Deserialize<Dictionary<string, PrizeItemData>>(json, _jsonOptions);

            if (data == null)
            {
                return [];
            }

            var id = 1;
            return data.Select(kvp => new PrizeItem
            {
                Id = kvp.Value.Id > 0 ? kvp.Value.Id : id++,
                Name = kvp.Key,
                Weight = kvp.Value.Weight > 0 ? kvp.Value.Weight : 1.0,
                Exist = kvp.Value.Exist,
                Count = Math.Max(0, kvp.Value.Count),
                Tags = NormalizeTags(kvp.Value.Tags)
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "从JSON导入失败: {FilePath}", filePath);
            return [];
        }
    }

    private List<PrizeItem> ImportFromCsv(string filePath)
    {
        try
        {
            var prizes = new List<PrizeItem>();
            var lines = File.ReadAllLines(filePath);

            // 跳过标题行
            for (var i = 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                var parts = line.Split(',');
                if (parts.Length == 0 || string.IsNullOrWhiteSpace(parts[0]))
                {
                    continue;
                }

                var prize = new PrizeItem
                {
                    Id = i,
                    Name = parts[0].Trim(),
                    Weight = parts.Length > 1 && double.TryParse(parts[1], out var weight) ? weight : 1.0,
                    Count = parts.Length > 2 && int.TryParse(parts[2], out var count) ? count : 1,
                    Tags = parts.Length > 3 ? NormalizeTags(parts[3]) : []
                };

                prizes.Add(prize);
            }

            return prizes;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "从CSV导入失败: {FilePath}", filePath);
            return [];
        }
    }

    private List<PrizeItem> ImportFromTxt(string filePath)
    {
        try
        {
            var prizes = new List<PrizeItem>();
            var lines = File.ReadAllLines(filePath);
            var id = 1;

            foreach (var line in lines)
            {
                var name = line.Trim();
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                prizes.Add(new PrizeItem
                {
                    Id = id++,
                    Name = name,
                    Weight = 1.0,
                    Count = 1
                });
            }

            return prizes;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "从TXT导入失败: {FilePath}", filePath);
            return [];
        }
    }

    private void ExportToJson(List<PrizeItem> prizes, string filePath)
    {
        var data = prizes.ToDictionary(
            p => p.Name,
            p => new PrizeItemData
            {
                Id = p.Id,
                Weight = p.Weight,
                Exist = p.Exist,
                Count = p.Count,
                Tags = p.Tags
            });

        var json = JsonSerializer.Serialize(data, _jsonOptions);
        File.WriteAllText(filePath, json);
    }

    private void ExportToCsv(List<PrizeItem> prizes, string filePath)
    {
        using var writer = new StreamWriter(filePath);
        writer.WriteLine("名称,权重,数量,标签");

        foreach (var prize in prizes)
        {
            var tags = string.Join(",", prize.Tags);
            writer.WriteLine($"{prize.Name},{prize.Weight},{prize.Count},{tags}");
        }
    }

    private void ExportToTxt(List<PrizeItem> prizes, string filePath)
    {
        using var writer = new StreamWriter(filePath);

        foreach (var prize in prizes)
        {
            writer.WriteLine(prize.Name);
        }
    }

    #endregion

    /// <summary>
    /// 奖品数据（用于JSON序列化）
    /// </summary>
    private class PrizeItemData
    {
        public int Id { get; set; }
        public double Weight { get; set; } = 1.0;
        public bool Exist { get; set; } = true;
        public int Count { get; set; } = 1;
        public List<string> Tags { get; set; } = [];
    }
}
