using System;
using System.Text.Json.Serialization;
using Avalonia.Media;

namespace AudioVisualizer.Models.ThemeModules.Core;

public abstract class ThemeModuleWave : ThemeModuleCore
{
    private float _posX = 0f;
    private float _posY = 0f;
    private float _intensity = 10f;
    private Color _color = Colors.White;
    // private bool _useSmoothing = false;
    private int _stepSizePx = 10;

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

    // TODO
    // [JsonRequired]
    // public bool UseSmoothing
    // {
    //     get => _useSmoothing;
    //     set => SetProperty(ref _useSmoothing, value);
    // }

    [JsonRequired]
    public int StepSizePx
    {
        get => _stepSizePx;
        set => SetProperty(ref _stepSizePx, value);
    }

    public override bool Equals(object? obj)
    {
        return obj is ThemeModuleWave other &&
               PosX == other.PosX &&
               PosY == other.PosY &&
               Intensity == other.Intensity &&
               Color == other.Color;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(PosX, PosY, Intensity, Color);
    }
}
