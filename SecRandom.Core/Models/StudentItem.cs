using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SecRandom.Core.Models;

/// <summary>
/// 学生项模型
/// </summary>
public partial class StudentItem : ObservableObject
{
    /// <summary>
    /// 学生ID/学号
    /// </summary>
    [ObservableProperty]
    [JsonPropertyName("id")]
    private int _id;

    /// <summary>
    /// 学生姓名
    /// </summary>
    [ObservableProperty]
    [JsonPropertyName("name")]
    private string _name = string.Empty;

    /// <summary>
    /// 性别
    /// </summary>
    [ObservableProperty]
    [JsonPropertyName("gender")]
    private string _gender = string.Empty;

    /// <summary>
    /// 小组
    /// </summary>
    [ObservableProperty]
    [JsonPropertyName("group")]
    private string _group = string.Empty;

    /// <summary>
    /// 是否存在（是否参与点名）
    /// </summary>
    [ObservableProperty]
    [JsonPropertyName("exist")]
    private bool _exist = true;

    /// <summary>
    /// 标签列表
    /// </summary>
    [ObservableProperty]
    [JsonPropertyName("tags")]
    private List<string> _tags = [];
}
