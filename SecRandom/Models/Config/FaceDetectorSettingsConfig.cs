using CommunityToolkit.Mvvm.ComponentModel;

namespace SecRandom.Models.Config;

/// <summary>
/// 人脸抽取设置配置模型
/// </summary>
public partial class FaceDetectorSettingsConfig : ObservableObject
{
    [ObservableProperty] private int _cameraSourceIndex = 0;
    [ObservableProperty] private string _detectorType = "version-RFB-640.onnx";
    [ObservableProperty] private int _pickingDurationSeconds = 3;
}
