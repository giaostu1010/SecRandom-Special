using CommunityToolkit.Mvvm.ComponentModel;

namespace SecRandom.Models.Config;

/// <summary>
/// 人脸抽取设置配置模型
/// </summary>
public partial class FaceDetectorSettingsConfig : ObservableObject
{
    [ObservableProperty] private int _cameraSourceIndex = 0;
    [ObservableProperty] private string _cameraDisplayResolution = string.Empty;
    [ObservableProperty] private int _cameraPreviewMode = 0;
    [ObservableProperty] private string _detectorType = "version-RFB-640.onnx";
    [ObservableProperty] private int _modelInputWidth = 0;
    [ObservableProperty] private int _modelInputHeight = 0;
    [ObservableProperty] private string _pickerFrameColor = "#FFFFFF";
    [ObservableProperty] private int _pickingDurationSeconds = 3;
    [ObservableProperty] private bool _playProcessAudio = true;
    [ObservableProperty] private bool _playResultAudio = true;
}
