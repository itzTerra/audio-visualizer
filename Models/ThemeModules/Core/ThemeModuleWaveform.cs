using System;
using System.Text.Json.Serialization;
using AudioVisualizer.Extensions;
using AudioVisualizer.Services.Visualizer;
using Avalonia.Media;

namespace AudioVisualizer.Models.ThemeModules.Core;

public class ThemeModuleWaveform : ThemeModuleCore
{
    public override string Identifier => "waveform";
    public override string Name => "Waveform";

    private float _posX = 0f;
    private float _posY = 0f;
    private float _intensity = 30f;
    private Color _color = Colors.White;
    private int _width = 800;
    private int _height = 200;

    [JsonRequired]
    public float PosX
    {
        get => _posX;
        set => SetProperty(ref _posX, value);
    }

    [JsonRequired]
    public float PosY
    {
        get => _posY;
        set => SetProperty(ref _posY, value);
    }

    [JsonRequired]
    public float Intensity
    {
        get => _intensity;
        set => SetProperty(ref _intensity, value);
    }

    [JsonRequired]
    public Color Color
    {
        get => _color;
        set => SetProperty(ref _color, value);
    }

    [JsonRequired]
    public int Width
    {
        get => _width;
        set => SetProperty(ref _width, value);
    }

    [JsonRequired]
    public int Height
    {
        get => _height;
        set => SetProperty(ref _height, value);
    }

    public override VisualizerBase CreateVisualizer()
    {
        return new CustomWaveformVisualizer(new(PosX, PosY), Intensity, new(Width, Height), Color.ToSFColor());
    }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj) && obj is ThemeModuleWaveform other &&
               PosX.Equals(other.PosX) &&
               PosY.Equals(other.PosY) &&
               Intensity.Equals(other.Intensity) &&
               Color.Equals(other.Color) &&
               Width == other.Width &&
               Height == other.Height;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), PosX, PosY, Intensity, Color, Width, Height);
    }
}
