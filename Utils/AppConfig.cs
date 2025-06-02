using AudioVisualizer.Models;
using AudioVisualizer.Models.ThemeModules.Background;
using AudioVisualizer.Models.ThemeModules.Core;
using AudioVisualizer.Models.ThemeModules.Misc;

namespace AudioVisualizer.Utils;

public static class AppConfig
{
    public static readonly bool IsDebug =
#if DEBUG
        true;
#else
        false;
#endif

    public static readonly string AppName = "AudioVisualizer";

    public static readonly string DefaultThemeNodeId = "Default-v1";
    public static readonly ThemeNodeModel[] DefaultThemes = new ThemeNodeModel[] {
            new ThemeNodeModel("Default", true, false) {
                Id = DefaultThemeNodeId,
                Nodes = [
                    new ThemeNodeModel("Harmony", false, true) {
                        Id = "Harmony-v1",
                        Modules = [
                            new ThemeModuleWaveLinear() {
                                Anchor = LineAnchor.Left,
                                PosX = 0f,
                                PosY = 540f,
                                Intensity = 200f,
                                Color = new(255, 6, 12, 12),
                                StepSizePx = 6,
                            },
                            new ThemeModuleWaveLinear() {
                                Mirrored = true,
                                Anchor = LineAnchor.Left,
                                PosX = 0f,
                                PosY = 540f,
                                Intensity = 400f,
                                Color = Avalonia.Media.Colors.White,
                                StepSizePx = 6,
                            },
                            new ThemeModuleBox() {
                                PosX = 0f,
                                PosY = 0f,
                                Width = 1920,
                                Height = 540,
                                Color = Avalonia.Media.Colors.White,
                                Anchor = RectAnchor.TopLeft
                            },
                            new ThemeModuleBox() {
                                PosX = 0f,
                                PosY = 540f,
                                Width = 1920,
                                Height = 540,
                                Color = Avalonia.Media.Colors.Black,
                                Anchor = RectAnchor.TopLeft
                            }
                        ]
                    },
                    new ThemeNodeModel("Dark Wave", false, true) {
                        Id = "DarkWave-v1",
                        Modules = [
                            new ThemeModuleBackgroundSolid(),
                            new ThemeModuleWaveform() {
                                Intensity = 150f,
                                Color = new(255, 142, 0, 255),
                                Width = 1920,
                                Height = 800
                            },
                            new ThemeModuleLevelMeter() {
                                PosX = 700f,
                                PosY = 600f,
                                BarColor = new(255, 142, 0, 255),
                                Width = 20,
                                Height = 100
                            },
                            new ThemeModuleText() {
                                Text = "Sample Text",
                                PosX = 820f,
                                PosY = 700f,
                                TextColor = Avalonia.Media.Colors.White,
                                FontSize = 40,
                            },
                            new ThemeModuleLevelMeter() {
                                PosX = 1300f,
                                PosY = 600f,
                                BarColor = new(255, 142, 0, 255),
                                Width = 20,
                                Height = 100
                            },
                        ]
                    },
                ]
            }
        };

    public static readonly int AudioSampleRate = 44100;
    public static readonly int StepButtonTimeSeconds = 5;
}
