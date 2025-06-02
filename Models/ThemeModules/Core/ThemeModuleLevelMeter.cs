using System;
using System.Text.Json.Serialization;
using AudioVisualizer.Extensions;
using AudioVisualizer.Services.Visualizer;
using Avalonia.Media;

namespace AudioVisualizer.Models.ThemeModules.Core;

public class ThemeModuleLevelMeter : ThemeModuleCore
{
    public override string Identifier => "levelMeter";
    public override string Name => "Level Meter";

    private float _posX = 0f;
    private float _posY = 0f;
    private Color _barColor = Colors.White;
    private Color _peakHoldColor = Colors.Red;
    private int _width = 20;
    private int _height = 100;

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
    public Color BarColor
    {
        get => _barColor;
        set => SetProperty(ref _barColor, value);
    }

    [JsonRequired]
    public Color PeakHoldColor
    {
        get => _peakHoldColor;
        set => SetProperty(ref _peakHoldColor, value);
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
        return new CustomLevelMeterVisualizer(new(PosX, PosY), new(Width, Height),
            BarColor.ToSFColor(), PeakHoldColor.ToSFColor());
    }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj) && obj is ThemeModuleLevelMeter other &&
            PosX.Equals(other.PosX) &&
            PosY.Equals(other.PosY) &&
            BarColor.Equals(other.BarColor) &&
            PeakHoldColor.Equals(other.PeakHoldColor) &&
            Width == other.Width &&
            Height == other.Height;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), PosX, PosY, BarColor, PeakHoldColor, Width, Height);
    }
}
