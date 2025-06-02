using System;
using System.Text.Json.Serialization;
using AudioVisualizer.Extensions;
using AudioVisualizer.Services.Visualizer;
using Avalonia.Media;

namespace AudioVisualizer.Models.ThemeModules.Core;

public class ThemeModuleSpectrum : ThemeModuleCore
{
    private const int FftSize = 2048;

    public override string Identifier => "spectrum";
    public override string Name => "Spectrum";

    private float _posX = 0f;
    private float _posY = 0f;
    private float _intensity = 10f;
    private Color _barColor = Colors.White;
    private int _width = 1000;
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
    public Color BarColor
    {
        get => _barColor;
        set => SetProperty(ref _barColor, value);
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
        return new CustomSpectrumVisualizer(new(PosX, PosY), new(Width, Height),
            Intensity, BarColor.ToSFColor(), FftSize);
    }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj) && obj is ThemeModuleSpectrum other &&
            PosX.Equals(other.PosX) &&
            PosY.Equals(other.PosY) &&
            BarColor.Equals(other.BarColor) &&
            Width == other.Width &&
            Height == other.Height;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), PosX, PosY, BarColor, Width, Height);
    }
}
