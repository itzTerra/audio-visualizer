using System;
using System.Text.Json.Serialization;
using AudioVisualizer.Services.Visualizer;
using Avalonia.Media;

namespace AudioVisualizer.Models.ThemeModules.Background;

public class ThemeModuleBackgroundSolid : ThemeModuleBackground
{
    public override string Identifier => "backgroundSolid";
    [JsonIgnore]
    public override string Name => "Solid Background";

    private Color _color = Colors.Transparent;
    [JsonRequired]
    public Color Color
    {
        get => _color;
        set => SetProperty(ref _color, value);
    }

    public override VisualizerBase CreateVisualizer()
    {
        return new BackgroundVisualizer(Color);
    }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj) && obj is ThemeModuleBackgroundSolid other &&
               Color.Equals(other.Color);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Color);
    }
}
