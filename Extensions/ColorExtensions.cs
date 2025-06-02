using SkiaSharp;

namespace AudioVisualizer.Extensions;

public static class ColorExtensions
{
    public static SKColor ToSKColor(this SoundFlow.Interfaces.Color color)
    {
        return new SKColor((byte)(color.R * 255), (byte)(color.G * 255), (byte)(color.B * 255), (byte)(color.A * 255));
    }

    public static SKColor ToSKColor(this Avalonia.Media.Color color)
    {
        return new SKColor(color.R, color.G, color.B, color.A);
    }

    public static SoundFlow.Interfaces.Color ToSFColor(this Avalonia.Media.Color color)
    {
        return new SoundFlow.Interfaces.Color(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
    }
}
