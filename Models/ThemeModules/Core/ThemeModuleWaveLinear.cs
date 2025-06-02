using System;
using System.Text.Json.Serialization;
using AudioVisualizer.Extensions;
using AudioVisualizer.Services.Visualizer;
using AudioVisualizer.Utils;

namespace AudioVisualizer.Models.ThemeModules.Core;

public class ThemeModuleWaveLinear : ThemeModuleWave
{
    public override string Identifier { get => "linearWave"; }
    [JsonIgnore]
    public override string Name { get => "Linear Wave"; }

    private float _rotation = 0f;
    [JsonRequired]
    public float Rotation
    {
        get => _rotation;
        set => SetProperty(ref _rotation, value);
    }

    private bool _mirrored = false;
    [JsonRequired]
    public bool Mirrored
    {
        get => _mirrored;
        set => SetProperty(ref _mirrored, value);
    }

    private LineAnchor _anchor = LineAnchor.Left;
    [JsonRequired]
    public LineAnchor Anchor
    {
        get => _anchor;
        set => SetProperty(ref _anchor, value);
    }

    public override VisualizerBase CreateVisualizer()
    {
        return new BarGraphVisualizer(new(PosX, PosY), Intensity, Color.ToSFColor(), StepSizePx, Rotation, Mirrored, Anchor);
    }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj) && obj is ThemeModuleWaveLinear other &&
               Rotation.Equals(other.Rotation) &&
               Mirrored == other.Mirrored &&
               Anchor == other.Anchor;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Rotation, Mirrored, Anchor);
    }
}
