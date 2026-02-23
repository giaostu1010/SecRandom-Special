using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SecRandom.Core.Abstraction;
using SecRandom.Core.Attributes;
using SecRandom.Services.Config;

namespace SecRandom.Views.SettingsPages;

[PageInfo("settings.draw.faceDetector", "\ue07c")]
public partial class FaceDetectorSettingsSubPage : UserControl
{
    public FaceDetectorSettingsSubPage()
    {
        DataContext = IAppHost.GetService<MainConfigHandler>().Data.DrawSettings.FaceDetectorSettings;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
