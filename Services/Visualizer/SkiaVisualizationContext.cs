namespace AudioVisualizer.Services.Visualizer;

using System;
using AudioVisualizer.Extensions;
using SkiaSharp;
using SoundFlow.Interfaces;

public class SkiaVisualizationContext : IVisualizationContext
{
    private readonly SKCanvas _canvas;

    public SkiaVisualizationContext(SKCanvas canvas)
    {
        _canvas = canvas;
    }

    public void Clear()
    {
        _canvas.Clear();
    }

    public void DrawLine(float x1, float y1, float x2, float y2, Color color, float thickness = 1f)
    {
        _canvas.DrawLine(x1, y1, x2, y2, new SKPaint() { Color = color.ToSKColor(), StrokeWidth = thickness });
    }

    public void DrawRectangle(float x, float y, float width, float height, Color color)
    {
        _canvas.DrawRect(x, y, width, height, new SKPaint() { Color = color.ToSKColor() });
    }

    public void UseSkia(Action<SKCanvas> action)
    {
        action(_canvas);
    }
}
