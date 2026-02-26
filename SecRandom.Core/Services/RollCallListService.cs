using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SecRandom.Core.Models;

namespace SecRandom.Core.Services;

/// <summary>
/// 点名名单服务 - 管理班级和学生数据
/// </summary>
public class RollCallListService
{
    private readonly ILogger<RollCallListService>? _logger;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    /// <summary>
    /// 班级名称列表
    /// </summary>
    public ObservableCollection<string> ClassNames { get; } = [];

    /// <summary>
    /// 当前选中的班级名称
    /// </summary>
    public string? CurrentClassName { get; set; }

    /// <summary>
    /// 当前班级的学生列表
    /// </summary>
    public ObservableCollection<StudentItem> CurrentStudents { get; } = [];

    public RollCallListService(ILogger<RollCallListService>? logger = null)
    {
        _logger = logger;
    }

    /// <summary>
    /// 获取班级数据目录路径
    /// </summary>
    private static string GetRollCallListDirectory()
    {
        return Utils.GetFilePath("list", "roll_call_list");
    }

    /// <summary>
    /// 获取班级文件路径
    /// </summary>
    private static string GetClassFilePath(string className)
    {
        return Path.Combine(GetRollCallListDirectory(), $"{className}.json");
    }

    /// <summary>
    /// 刷新班级名称列表
    /// </summary>
    public void RefreshClassNames()
    {
        try
        {
            var directory = GetRollCallListDirectory();

            if (!Directory.Exists(directory))
            {
                _logger?.LogWarning("班级文件夹不存在: {Directory}", directory);
                Directory.CreateDirectory(directory);
                ClassNames.Clear();
                return;
            }

            var files = Directory.GetFiles(directory, "*.json");
            var classNames = files
                .Select(Path.GetFileNameWithoutExtension)
                .Where(name => !string.IsNullOrEmpty(name))
                .OrderBy(name => name)
                .ToList();

            ClassNames.Clear();
            foreach (var name in classNames)
            {
                ClassNames.Add(name!);
            }

            _logger?.LogDebug("找到 {Count} 个班级: {Classes}", classNames.Count, string.Join(", ", classNames));
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "获取班级列表失败");
        }
    }

    /// <summary>
    /// 获取指定班级的学生列表
    /// </summary>
    /// <param name="className">班级名称</param>
    /// <returns>学生列表</returns>
    public List<StudentItem> GetStudentList(string className)
    {
        try
        {
            var filePath = GetClassFilePath(className);

            if (!File.Exists(filePath))
            {
                _logger?.LogWarning("班级文件不存在: {FilePath}", filePath);
                return [];
            }

            var json = File.ReadAllText(filePath);
            var studentData = JsonSerializer.Deserialize<Dictionary<string, StudentItemData>>(json, _jsonOptions);

            if (studentData == null)
            {
                return [];
            }

            var students = new List<StudentItem>();
            var id = 1;
            foreach (var (name, info) in studentData)
            {
                var student = new StudentItem
                {
                    Id = info.Id > 0 ? info.Id : id,
                    Name = name,
                    Gender = info.Gender ?? string.Empty,
                    Group = info.Group ?? string.Empty,
                    Exist = info.Exist,
                    Tags = NormalizeTags(info.Tags)
                };
                students.Add(student);
                id++;
            }

            // 按ID排序
            students = students.OrderBy(s => s.Id).ToList();

            _logger?.LogDebug("班级 {ClassName} 共有 {Count} 名学生", className, students.Count);
            return students;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "获取学生列表失败: {ClassName}", className);
            return [];
        }
    }

    /// <summary>
    /// 刷新当前班级的学生列表
    /// </summary>
    public void RefreshCurrentStudents()
    {
        if (string.IsNullOrEmpty(CurrentClassName))
        {
            CurrentStudents.Clear();
            return;
        }

        var students = GetStudentList(CurrentClassName);
        CurrentStudents.Clear();
        foreach (var student in students)
        {
            CurrentStudents.Add(student);
        }
    }

    /// <summary>
    /// 创建新班级
    /// </summary>
    /// <param name="className">班级名称</param>
    /// <returns>是否成功</returns>
    public bool CreateClass(string className)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(className))
            {
                _logger?.LogWarning("班级名称不能为空");
                return false;
            }

            // 验证班级名称
            if (!IsValidName(className))
            {
                _logger?.LogWarning("班级名称包含非法字符: {ClassName}", className);
                return false;
            }

            var filePath = GetClassFilePath(className);

            if (File.Exists(filePath))
            {
                _logger?.LogWarning("班级已存在: {ClassName}", className);
                return false;
            }

            // 创建空班级文件
            var emptyData = new Dictionary<string, StudentItemData>();
            var json = JsonSerializer.Serialize(emptyData, _jsonOptions);
            File.WriteAllText(filePath, json);

            _logger?.LogInformation("创建班级成功: {ClassName}", className);
            RefreshClassNames();
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "创建班级失败: {ClassName}", className);
            return false;
        }
    }

    /// <summary>
    /// 删除班级
    /// </summary>
    /// <param name="className">班级名称</param>
    /// <returns>是否成功</returns>
    public bool DeleteClass(string className)
    {
        try
        {
            var filePath = GetClassFilePath(className);

            if (!File.Exists(filePath))
            {
                _logger?.LogWarning("班级不存在: {ClassName}", className);
                return false;
            }

            File.Delete(filePath);

            _logger?.LogInformation("删除班级成功: {ClassName}", className);
            RefreshClassNames();
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "删除班级失败: {ClassName}", className);
            return false;
        }
    }

    /// <summary>
    /// 重命名班级
    /// </summary>
    /// <param name="oldName">旧名称</param>
    /// <param name="newName">新名称</param>
    /// <returns>是否成功</returns>
    public bool RenameClass(string oldName, string newName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(newName))
            {
                _logger?.LogWarning("新班级名称不能为空");
                return false;
            }

            if (!IsValidName(newName))
            {
                _logger?.LogWarning("新班级名称包含非法字符: {NewName}", newName);
                return false;
            }

            var oldPath = GetClassFilePath(oldName);
            var newPath = GetClassFilePath(newName);

            if (!File.Exists(oldPath))
            {
                _logger?.LogWarning("班级不存在: {OldName}", oldName);
                return false;
            }

            if (File.Exists(newPath))
            {
                _logger?.LogWarning("目标班级已存在: {NewName}", newName);
                return false;
            }

            File.Move(oldPath, newPath);

            _logger?.LogInformation("重命名班级成功: {OldName} -> {NewName}", oldName, newName);
            RefreshClassNames();
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "重命名班级失败: {OldName} -> {NewName}", oldName, newName);
            return false;
        }
    }

    /// <summary>
    /// 保存学生数据
    /// </summary>
    /// <param name="className">班级名称</param>
    /// <param name="students">学生列表</param>
    /// <returns>是否成功</returns>
    public bool SaveStudents(string className, List<StudentItem> students)
    {
        try
        {
            var filePath = GetClassFilePath(className);

            // 转换为字典格式（名称 -> 数据）
            var studentData = new Dictionary<string, StudentItemData>();
            foreach (var student in students)
            {
                studentData[student.Name] = new StudentItemData
                {
                    Id = student.Id,
                    Gender = student.Gender,
                    Group = student.Group,
                    Exist = student.Exist,
                    Tags = student.Tags
                };
            }

            var json = JsonSerializer.Serialize(studentData, _jsonOptions);
            File.WriteAllText(filePath, json);

            _logger?.LogDebug("保存班级数据成功: {ClassName}", className);
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "保存班级数据失败: {ClassName}", className);
            return false;
        }
    }

    /// <summary>
    /// 更新单个学生
    /// </summary>
    /// <param name="className">班级名称</param>
    /// <param name="studentId">学生ID</param>
    /// <param name="updatedStudent">更新后的学生数据</param>
    /// <returns>是否成功</returns>
    public bool UpdateStudent(string className, int studentId, StudentItem updatedStudent)
    {
        try
        {
            var filePath = GetClassFilePath(className);

            if (!File.Exists(filePath))
            {
                _logger?.LogWarning("班级文件不存在: {FilePath}", filePath);
                return false;
            }

            var json = File.ReadAllText(filePath);
            var studentData = JsonSerializer.Deserialize<Dictionary<string, StudentItemData>>(json, _jsonOptions);

            if (studentData == null)
            {
                return false;
            }

            // 查找要更新的学生
            string? keyToUpdate = null;
            foreach (var (key, value) in studentData)
            {
                if (value.Id == studentId)
                {
                    keyToUpdate = key;
                    break;
                }
            }

            if (keyToUpdate == null)
            {
                _logger?.LogWarning("未找到学生ID: {StudentId}", studentId);
                return false;
            }

            // 如果名称改变，需要重新创建键
            if (keyToUpdate != updatedStudent.Name)
            {
                studentData.Remove(keyToUpdate);
                studentData[updatedStudent.Name] = new StudentItemData
                {
                    Id = updatedStudent.Id,
                    Gender = updatedStudent.Gender,
                    Group = updatedStudent.Group,
                    Exist = updatedStudent.Exist,
                    Tags = updatedStudent.Tags
                };
            }
            else
            {
                studentData[keyToUpdate] = new StudentItemData
                {
                    Id = updatedStudent.Id,
                    Gender = updatedStudent.Gender,
                    Group = updatedStudent.Group,
                    Exist = updatedStudent.Exist,
                    Tags = updatedStudent.Tags
                };
            }

            var newJson = JsonSerializer.Serialize(studentData, _jsonOptions);
            File.WriteAllText(filePath, newJson);

            _logger?.LogDebug("更新学生成功: {ClassName} - {StudentId}", className, studentId);
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "更新学生失败: {ClassName} - {StudentId}", className, studentId);
            return false;
        }
    }

    /// <summary>
    /// 添加学生
    /// </summary>
    /// <param name="className">班级名称</param>
    /// <param name="student">学生</param>
    /// <returns>是否成功</returns>
    public bool AddStudent(string className, StudentItem student)
    {
        try
        {
            var filePath = GetClassFilePath(className);

            if (!File.Exists(filePath))
            {
                _logger?.LogWarning("班级文件不存在: {FilePath}", filePath);
                return false;
            }

            var json = File.ReadAllText(filePath);
            var studentData = JsonSerializer.Deserialize<Dictionary<string, StudentItemData>>(json, _jsonOptions)
                            ?? new Dictionary<string, StudentItemData>();

            // 检查名称是否重复
            if (studentData.ContainsKey(student.Name))
            {
                _logger?.LogWarning("学生姓名已存在: {StudentName}", student.Name);
                return false;
            }

            // 计算新ID
            var maxId = studentData.Values.Max(s => s.Id);
            student.Id = maxId + 1;

            studentData[student.Name] = new StudentItemData
            {
                Id = student.Id,
                Gender = student.Gender,
                Group = student.Group,
                Exist = student.Exist,
                Tags = student.Tags
            };

            var newJson = JsonSerializer.Serialize(studentData, _jsonOptions);
            File.WriteAllText(filePath, newJson);

            _logger?.LogDebug("添加学生成功: {ClassName} - {StudentName}", className, student.Name);
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "添加学生失败: {ClassName} - {StudentName}", className, student.Name);
            return false;
        }
    }

    /// <summary>
    /// 删除学生
    /// </summary>
    /// <param name="className">班级名称</param>
    /// <param name="studentId">学生ID</param>
    /// <returns>是否成功</returns>
    public bool DeleteStudent(string className, int studentId)
    {
        try
        {
            var filePath = GetClassFilePath(className);

            if (!File.Exists(filePath))
            {
                _logger?.LogWarning("班级文件不存在: {FilePath}", filePath);
                return false;
            }

            var json = File.ReadAllText(filePath);
            var studentData = JsonSerializer.Deserialize<Dictionary<string, StudentItemData>>(json, _jsonOptions);

            if (studentData == null)
            {
                return false;
            }

            // 查找要删除的学生
            string? keyToDelete = null;
            foreach (var (key, value) in studentData)
            {
                if (value.Id == studentId)
                {
                    keyToDelete = key;
                    break;
                }
            }

            if (keyToDelete == null)
            {
                _logger?.LogWarning("未找到学生ID: {StudentId}", studentId);
                return false;
            }

            studentData.Remove(keyToDelete);

            var newJson = JsonSerializer.Serialize(studentData, _jsonOptions);
            File.WriteAllText(filePath, newJson);

            _logger?.LogDebug("删除学生成功: {ClassName} - {StudentId}", className, studentId);
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "删除学生失败: {ClassName} - {StudentId}", className, studentId);
            return false;
        }
    }

    /// <summary>
    /// 从文件导入学生
    /// </summary>
    /// <param name="className">班级名称</param>
    /// <param name="filePath">导入文件路径</param>
    /// <param name="clearExisting">是否清除现有数据</param>
    /// <returns>导入的学生数量</returns>
    public int ImportStudents(string className, string filePath, bool clearExisting = false)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                _logger?.LogWarning("导入文件不存在: {FilePath}", filePath);
                return 0;
            }

            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            List<StudentItem> importedStudents;

            switch (extension)
            {
                case ".json":
                    importedStudents = ImportFromJson(filePath);
                    break;
                case ".csv":
                    importedStudents = ImportFromCsv(filePath);
                    break;
                case ".txt":
                    importedStudents = ImportFromTxt(filePath);
                    break;
                case ".xlsx":
                case ".xls":
                    importedStudents = ImportFromExcel(filePath);
                    break;
                default:
                    _logger?.LogWarning("不支持的文件格式: {Extension}", extension);
                    return 0;
            }

            if (importedStudents.Count == 0)
            {
                return 0;
            }

            var classFilePath = GetClassFilePath(className);
            Dictionary<string, StudentItemData> studentData;

            if (clearExisting || !File.Exists(classFilePath))
            {
                studentData = new Dictionary<string, StudentItemData>();
            }
            else
            {
                var json = File.ReadAllText(classFilePath);
                studentData = JsonSerializer.Deserialize<Dictionary<string, StudentItemData>>(json, _jsonOptions)
                            ?? new Dictionary<string, StudentItemData>();
            }

            // 计算起始ID
            var maxId = studentData.Count > 0 ? studentData.Values.Max(s => s.Id) : 0;

            // 处理重复名称
            var uniqueStudents = MakeUniqueNames(importedStudents, studentData.Keys.ToHashSet());

            foreach (var student in uniqueStudents)
            {
                maxId++;
                student.Id = maxId;
                studentData[student.Name] = new StudentItemData
                {
                    Id = student.Id,
                    Gender = student.Gender,
                    Group = student.Group,
                    Exist = student.Exist,
                    Tags = student.Tags
                };
            }

            var newJson = JsonSerializer.Serialize(studentData, _jsonOptions);
            File.WriteAllText(classFilePath, newJson);

            _logger?.LogInformation("导入学生成功: {ClassName} - {Count} 名", className, uniqueStudents.Count);
            return uniqueStudents.Count;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "导入学生失败: {ClassName}", className);
            return 0;
        }
    }

    /// <summary>
    /// 导出学生到文件
    /// </summary>
    /// <param name="className">班级名称</param>
    /// <param name="filePath">导出文件路径</param>
    /// <returns>是否成功</returns>
    public bool ExportStudents(string className, string filePath)
    {
        try
        {
            var students = GetStudentList(className);
            if (students.Count == 0)
            {
                _logger?.LogWarning("班级没有学生数据: {ClassName}", className);
                return false;
            }

            var extension = Path.GetExtension(filePath).ToLowerInvariant();

            switch (extension)
            {
                case ".json":
                    ExportToJson(students, filePath);
                    break;
                case ".csv":
                    ExportToCsv(students, filePath);
                    break;
                case ".txt":
                    ExportToTxt(students, filePath);
                    break;
                case ".xlsx":
                    ExportToExcel(students, filePath);
                    break;
                default:
                    _logger?.LogWarning("不支持的导出格式: {Extension}", extension);
                    return false;
            }

            _logger?.LogInformation("导出学生成功: {FilePath}", filePath);
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "导出学生失败: {ClassName}", className);
            return false;
        }
    }

    /// <summary>
    /// 获取班级中的小组列表
    /// </summary>
    /// <param name="className">班级名称</param>
    /// <returns>小组列表</returns>
    public List<string> GetGroupList(string className)
    {
        var students = GetStudentList(className);
        return students
            .Select(s => s.Group)
            .Where(g => !string.IsNullOrEmpty(g))
            .Distinct()
            .OrderBy(g => g)
            .ToList();
    }

    /// <summary>
    /// 获取班级中的性别列表
    /// </summary>
    /// <param name="className">班级名称</param>
    /// <returns>性别列表</returns>
    public List<string> GetGenderList(string className)
    {
        var students = GetStudentList(className);
        return students
            .Select(s => s.Gender)
            .Where(g => !string.IsNullOrEmpty(g))
            .Distinct()
            .OrderBy(g => g)
            .ToList();
    }

    /// <summary>
    /// 获取指定小组的成员
    /// </summary>
    /// <param name="className">班级名称</param>
    /// <param name="groupName">小组名称</param>
    /// <returns>小组成员列表</returns>
    public List<StudentItem> GetGroupMembers(string className, string groupName)
    {
        var students = GetStudentList(className);
        return students.Where(s => s.Group == groupName).OrderBy(s => s.Id).ToList();
    }

    /// <summary>
    /// 批量创建班级
    /// </summary>
    /// <param name="classNames">班级名称列表</param>
    /// <returns>创建的班级数量</returns>
    public int CreateClasses(List<string> classNames)
    {
        var created = 0;
        foreach (var name in classNames)
        {
            if (CreateClass(name))
            {
                created++;
            }
        }
        return created;
    }

    /// <summary>
    /// 批量删除班级
    /// </summary>
    /// <param name="classNames">班级名称列表</param>
    /// <returns>删除的班级数量</returns>
    public int DeleteClasses(List<string> classNames)
    {
        var deleted = 0;
        foreach (var name in classNames)
        {
            if (DeleteClass(name))
            {
                deleted++;
            }
        }
        return deleted;
    }

    #region 私有辅助方法

    /// <summary>
    /// 验证名称是否有效
    /// </summary>
    private static bool IsValidName(string name)
    {
        var invalidChars = new[] { '/', ':', '*', '?', '"', '<', '>', '|' };
        return !name.Any(c => invalidChars.Contains(c)) && 
               !name.Equals("class", StringComparison.OrdinalIgnoreCase);
    }

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
    private static List<StudentItem> MakeUniqueNames(List<StudentItem> students, HashSet<string> existingNames)
    {
        var result = new List<StudentItem>();
        var usedNames = new HashSet<string>(existingNames);
        var counters = new Dictionary<string, int>();

        foreach (var student in students)
        {
            var name = student.Name;

            if (!usedNames.Contains(name))
            {
                usedNames.Add(name);
                result.Add(student);
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
            student.Name = candidate;
            result.Add(student);
        }

        return result;
    }

    private List<StudentItem> ImportFromJson(string filePath)
    {
        try
        {
            var json = File.ReadAllText(filePath);
            var data = JsonSerializer.Deserialize<Dictionary<string, StudentItemData>>(json, _jsonOptions);

            if (data == null)
            {
                return [];
            }

            var id = 1;
            return data.Select(kvp => new StudentItem
            {
                Id = kvp.Value.Id > 0 ? kvp.Value.Id : id++,
                Name = kvp.Key,
                Gender = kvp.Value.Gender ?? string.Empty,
                Group = kvp.Value.Group ?? string.Empty,
                Exist = kvp.Value.Exist,
                Tags = NormalizeTags(kvp.Value.Tags)
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "从JSON导入失败: {FilePath}", filePath);
            return [];
        }
    }

    private List<StudentItem> ImportFromCsv(string filePath)
    {
        try
        {
            var students = new List<StudentItem>();
            var lines = File.ReadAllLines(filePath, Encoding.UTF8);

            // 跳过标题行
            for (var i = 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                var parts = line.Split(',');
                if (parts.Length < 2 || string.IsNullOrWhiteSpace(parts[1]))
                {
                    continue;
                }

                var student = new StudentItem
                {
                    Id = int.TryParse(parts[0].Trim(), out var id) ? id : i,
                    Name = parts[1].Trim(),
                    Gender = parts.Length > 2 ? parts[2].Trim() : string.Empty,
                    Group = parts.Length > 3 ? parts[3].Trim() : string.Empty,
                    Tags = parts.Length > 4 ? NormalizeTags(parts[4]) : []
                };

                students.Add(student);
            }

            return students;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "从CSV导入失败: {FilePath}", filePath);
            return [];
        }
    }

    private List<StudentItem> ImportFromTxt(string filePath)
    {
        try
        {
            var students = new List<StudentItem>();
            var lines = File.ReadAllLines(filePath, Encoding.UTF8);
            var id = 1;

            foreach (var line in lines)
            {
                var name = line.Trim();
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                students.Add(new StudentItem
                {
                    Id = id++,
                    Name = name
                });
            }

            return students;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "从TXT导入失败: {FilePath}", filePath);
            return [];
        }
    }

    private List<StudentItem> ImportFromExcel(string filePath)
    {
        // Excel导入需要额外的库支持，这里返回空列表
        // 实际实现可以使用 EPPlus 或 NPOI 等库
        _logger?.LogWarning("Excel导入功能需要额外库支持: {FilePath}", filePath);
        return [];
    }

    private void ExportToJson(List<StudentItem> students, string filePath)
    {
        var data = students.ToDictionary(
            s => s.Name,
            s => new StudentItemData
            {
                Id = s.Id,
                Gender = s.Gender,
                Group = s.Group,
                Exist = s.Exist,
                Tags = s.Tags
            });

        var json = JsonSerializer.Serialize(data, _jsonOptions);
        File.WriteAllText(filePath, json);
    }

    private void ExportToCsv(List<StudentItem> students, string filePath)
    {
        using var writer = new StreamWriter(filePath, false, Encoding.UTF8);
        writer.WriteLine("学号,姓名,性别,小组,标签");

        foreach (var student in students)
        {
            var tags = string.Join(",", student.Tags);
            writer.WriteLine($"{student.Id},{student.Name},{student.Gender},{student.Group},{tags}");
        }
    }

    private void ExportToTxt(List<StudentItem> students, string filePath)
    {
        using var writer = new StreamWriter(filePath, false, Encoding.UTF8);

        foreach (var student in students)
        {
            writer.WriteLine(student.Name);
        }
    }

    private void ExportToExcel(List<StudentItem> students, string filePath)
    {
        // Excel导出需要额外的库支持
        // 实际实现可以使用 EPPlus 或 NPOI 等库
        _logger?.LogWarning("Excel导出功能需要额外库支持: {FilePath}", filePath);
    }

    #endregion

    /// <summary>
    /// 学生数据（用于JSON序列化）
    /// </summary>
    private class StudentItemData
    {
        public int Id { get; set; }
        public string? Gender { get; set; }
        public string? Group { get; set; }
        public bool Exist { get; set; } = true;
        public List<string> Tags { get; set; } = [];
    }
}
