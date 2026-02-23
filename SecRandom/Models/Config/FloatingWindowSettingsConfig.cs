using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace SecRandom.Models.Config;

/// <summary>
/// 悬浮窗设置配置模型
/// </summary>
public partial class FloatingWindowSettingsConfig : ObservableObject
{
    [ObservableProperty] private bool _isShowFloatingWindowOnStartupEnabled = true;
    [ObservableProperty] private double _floatingWindowOpacity = 0.8;
    [ObservableProperty] private int _floatingWindowTopmostMode = 1;
    [ObservableProperty] private bool _isExtendQuickDrawComponentEnabled = false;

    [ObservableProperty] private List<string> _floatingWindowButtonControl = ["roll_call", "quick_draw"];

    [ObservableProperty] private int _floatingWindowPlacement = 1;
    [ObservableProperty] private int _floatingWindowDisplayStyle = 0;
    [ObservableProperty] private int _floatingWindowSize = 3;
    [ObservableProperty] private int _floatingWindowThemeMode = 0;

    [ObservableProperty] private bool _isFloatingWindowStickToEdgeEnabled = true;
    [ObservableProperty] private int _floatingWindowStickToEdgeRecoverSeconds = 3;
    [ObservableProperty] private int _floatingWindowStickToEdgeDisplayStyle = 1;

    [ObservableProperty] private bool _isFloatingWindowDraggableEnabled = true;
    [ObservableProperty] private int _floatingWindowLongPressDuration = 500;

    [ObservableProperty] private bool _isDoNotStealFocusEnabled = true;

    [ObservableProperty] private bool _isHideFloatingWindowOnForegroundEnabled = false;
    [ObservableProperty] private string _hideFloatingWindowOnForegroundWindowTitles = string.Empty;
    [ObservableProperty] private string _hideFloatingWindowOnForegroundProcessNames = string.Empty;

    [ObservableProperty] private int _floatingWindowPositionX = 100;
    [ObservableProperty] private int _floatingWindowPositionY = 100;

    [JsonIgnore]
    public int FloatingWindowOpacityPercent
    {
        get => (int)Math.Round(FloatingWindowOpacity * 100, MidpointRounding.AwayFromZero);
        set => FloatingWindowOpacity = Math.Clamp(value, 0, 100) / 100.0;
    }

    [JsonIgnore]
    public bool IsRollCallButtonEnabled
    {
        get => HasButton("roll_call");
        set => SetButtonEnabled("roll_call", value);
    }

    [JsonIgnore]
    public bool IsQuickDrawButtonEnabled
    {
        get => HasButton("quick_draw");
        set => SetButtonEnabled("quick_draw", value);
    }

    [JsonIgnore]
    public bool IsLotteryButtonEnabled
    {
        get => HasButton("lottery");
        set => SetButtonEnabled("lottery", value);
    }

    [JsonIgnore]
    public bool IsFaceDrawButtonEnabled
    {
        get => HasButton("face_draw");
        set => SetButtonEnabled("face_draw", value);
    }

    [JsonIgnore]
    public bool IsTimerButtonEnabled
    {
        get => HasButton("timer");
        set => SetButtonEnabled("timer", value);
    }

    partial void OnFloatingWindowOpacityChanged(double value)
    {
        OnPropertyChanged(nameof(FloatingWindowOpacityPercent));
    }

    partial void OnFloatingWindowButtonControlChanged(List<string> value)
    {
        var normalized = NormalizeButtonKeys(value);
        if (!ReferenceEquals(normalized, value))
        {
            FloatingWindowButtonControl = normalized;
            return;
        }

        OnPropertyChanged(nameof(IsRollCallButtonEnabled));
        OnPropertyChanged(nameof(IsQuickDrawButtonEnabled));
        OnPropertyChanged(nameof(IsLotteryButtonEnabled));
        OnPropertyChanged(nameof(IsFaceDrawButtonEnabled));
        OnPropertyChanged(nameof(IsTimerButtonEnabled));
    }

    private bool HasButton(string key)
    {
        return FloatingWindowButtonControl.Any(x => string.Equals(x, key, StringComparison.Ordinal));
    }

    private void SetButtonEnabled(string key, bool enabled)
    {
        var normalized = NormalizeButtonKeys(FloatingWindowButtonControl);
        var has = normalized.Any(x => string.Equals(x, key, StringComparison.Ordinal));

        if (enabled == has)
        {
            return;
        }

        if (!enabled && normalized.Count == 1 && has)
        {
            OnPropertyChanged(GetButtonPropertyName(key));
            return;
        }

        var next = normalized.ToList();
        if (enabled)
        {
            next.Add(key);
        }
        else
        {
            next.RemoveAll(x => string.Equals(x, key, StringComparison.Ordinal));
        }

        FloatingWindowButtonControl = next;
    }

    private static List<string> NormalizeButtonKeys(List<string>? raw)
    {
        var allowed = new HashSet<string>(StringComparer.Ordinal)
        {
            "roll_call",
            "quick_draw",
            "lottery",
            "face_draw",
            "timer"
        };

        var result = new List<string>();
        if (raw is null)
        {
            return ["roll_call"];
        }

        foreach (var v in raw)
        {
            if (string.IsNullOrWhiteSpace(v))
            {
                continue;
            }

            var key = v.Trim();
            if (!allowed.Contains(key))
            {
                continue;
            }

            if (result.Contains(key, StringComparer.Ordinal))
            {
                continue;
            }

            result.Add(key);
        }

        if (result.Count == 0)
        {
            result.Add("roll_call");
        }

        if (result.Count == raw.Count)
        {
            var same = true;
            for (var i = 0; i < result.Count; i++)
            {
                if (!string.Equals(raw[i], result[i], StringComparison.Ordinal))
                {
                    same = false;
                    break;
                }
            }

            if (same)
            {
                return raw;
            }
        }

        return result;
    }

    private static string GetButtonPropertyName(string key)
    {
        return key switch
        {
            "roll_call" => nameof(IsRollCallButtonEnabled),
            "quick_draw" => nameof(IsQuickDrawButtonEnabled),
            "lottery" => nameof(IsLotteryButtonEnabled),
            "face_draw" => nameof(IsFaceDrawButtonEnabled),
            "timer" => nameof(IsTimerButtonEnabled),
            _ => nameof(FloatingWindowButtonControl)
        };
    }
}
