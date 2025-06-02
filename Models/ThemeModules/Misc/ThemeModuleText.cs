using System;
using System.Numerics;
using System.Text.Json.Serialization;
using AudioVisualizer.Services.Visualizer;
using Avalonia.Media;

namespace AudioVisualizer.Models.ThemeModules.Misc;

public class ThemeModuleText : ThemeModuleMisc
{
    public override string Identifier => "text";
    [JsonIgnore]
    public override string Name => "Text";

    private string _text = "Text";
    [JsonRequired]
    public string Text
    {
        get => _text;
        set => SetProperty(ref _text, value);
    }

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

    private Color _textColor = Colors.White;
    [JsonRequired]
    public Color TextColor
    {
        get => _textColor;
        set => SetProperty(ref _textColor, value);
    }

    private int _fontSize = 12;
    [JsonRequired]
    public int FontSize
    {
        get => _fontSize;
        set => SetProperty(ref _fontSize, value);
    }

    // TODO fix serialization of FontFamily
    // private FontFamily _fontFamily = FontFamily.Default;
    // [JsonRequired]
    // public FontFamily FontFamily
    // {
    //     get => _fontFamily;
    //     set => SetProperty(ref _fontFamily, value);
    // }

    private bool _bold = false;
    [JsonRequired]
    public bool Bold
    {
        get => _bold;
        set => SetProperty(ref _bold, value);
    }

    private bool _italic = false;
    [JsonRequired]
    public bool Italic
    {
        get => _italic;
        set => SetProperty(ref _italic, value);
    }

    public override VisualizerBase CreateVisualizer()
    {
        return new TextVisualizer(
            Text,
            new Vector2(PosX, PosY),
            TextColor,
            FontSize,
            Bold,
            Italic
        );
    }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj) && obj is ThemeModuleText other &&
               Text == other.Text &&
               PosX.Equals(other.PosX) &&
               PosY.Equals(other.PosY) &&
               TextColor.Equals(other.TextColor) &&
               FontSize == other.FontSize &&
               Bold == other.Bold &&
               Italic == other.Italic;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Text, PosX, PosY, TextColor, FontSize, Bold, Italic);
    }
}
