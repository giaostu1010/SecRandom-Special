using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SecRandom.Core.Models;

/// <summary>
/// 奖品项模型
/// </summary>
public partial class PrizeItem : ObservableObject
{
    /// <summary>
    /// 奖品ID
    /// </summary>
    [ObservableProperty]
    [JsonPropertyName("id")]
    private int _id;

    /// <summary>
    /// 奖品名称
    /// </summary>
    [ObservableProperty]
    [JsonPropertyName("name")]
    private string _name = string.Empty;

    /// <summary>
    /// 奖品权重
    /// </summary>
    [ObservableProperty]
    [JsonPropertyName("weight")]
    private double _weight = 1.0;

    /// <summary>
    /// 是否存在（是否参与抽奖）
    /// </summary>
    [ObservableProperty]
    [JsonPropertyName("exist")]
    private bool _exist = true;

    /// <summary>
    /// 奖品数量
    /// </summary>
    [ObservableProperty]
    [JsonPropertyName("count")]
    private int _count = 1;

    /// <summary>
    /// 标签列表
    /// </summary>
    [ObservableProperty]
    [JsonPropertyName("tags")]
    private List<string> _tags = [];
}
