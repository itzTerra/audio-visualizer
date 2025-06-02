using System;
using System.Text.Json.Serialization;
using AudioVisualizer.Services.Visualizer;
using AudioVisualizer.Utils;
using Avalonia.Media;

namespace AudioVisualizer.Models.ThemeModules.Misc;

public class ThemeModuleBox : ThemeModuleMisc
{
    public override string Identifier { get => "box"; }
    [JsonIgnore]
    public override string Name { get => "Box"; }

    private float _posX = 0f;
    [JsonRequired]
    public float PosX
    {
        get => _posX;
        set => SetProperty(ref _posX, value);
    }

    private float _posY = 0f;
    [JsonRequired]
    public float PosY
    {
        get => _posY;
        set => SetProperty(ref _posY, value);
    }

    private int _width = 500;
    [JsonRequired]
    public int Width
    {
        get => _width;
        set => SetProperty(ref _width, value);
    }

    private int _height = 500;
    [JsonRequired]
    public int Height
    {
        get => _height;
        set => SetProperty(ref _height, value);
    }

    private Color _color = Colors.White;
    [JsonRequired]
    public Color Color
    {
        get => _color;
        set => SetProperty(ref _color, value);
    }

    private float _rotation = 0f;
    [JsonRequired]
    public float Rotation
    {
        get => _rotation;
        set => SetProperty(ref _rotation, value);
    }

    private RectAnchor _anchor = RectAnchor.TopLeft;
    [JsonRequired]
    public RectAnchor Anchor
    {
        get => _anchor;
        set => SetProperty(ref _anchor, value);
    }

    public override VisualizerBase CreateVisualizer()
    {
        return new BoxVisualizer(new(PosX, PosY), new(Width, Height), Color, Rotation, Anchor);
    }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj) && obj is ThemeModuleBox other &&
            PosX.Equals(other.PosX) &&
            PosY.Equals(other.PosY) &&
            Width.Equals(other.Width) &&
            Height.Equals(other.Height) &&
            Color.Equals(other.Color) &&
            Rotation.Equals(other.Rotation) &&
            Anchor == other.Anchor;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), PosX, PosY, Width, Height, Color, Rotation, Anchor);
    }
}
