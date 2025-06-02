using System.Text.Json.Serialization;
using AudioVisualizer.Utils;

namespace AudioVisualizer.Models;

public class SettingsModel
{
    [JsonRequired]
    public string DefaultExportDirectory { get; set; } = "";

    [JsonRequired]
    public int ResolutionWidth { get; set; } = 1920;

    [JsonRequired]
    public int ResolutionHeight { get; set; } = 1080;

    [JsonRequired]
    public AppThemeVariant Theme { get; set; } = AppThemeVariant.System;

    [JsonRequired]
    public Language Language { get; set; } = Language.English;

    [JsonRequired]
    public double MediaPlayerColWidth { get; set; } = 990;

    [JsonRequired]
    public double ThemeExplorerRowHeight { get; set; } = 260;

    public SettingsModel() { }
}
