using System;
using System.Numerics;
using AudioVisualizer.Extensions;
using AudioVisualizer.Utils;
using SkiaSharp;

namespace AudioVisualizer.Services.Visualizer;

public class BoxVisualizer : VisualizerBase
{
    public override bool IsAudioDataRequired => false;
    public override int Priority => 20;

    private Vector2 _position;
    private Vector2 _size;
    private Avalonia.Media.Color _bgColor;
    private float _rotation;
    private RectAnchor _anchor;

    public BoxVisualizer(Vector2 position, Vector2 size, Avalonia.Media.Color bgColor, float rotation, RectAnchor anchor)
    {
        _position = position;
        _size = size;
        _bgColor = bgColor;
        _rotation = rotation;
        _anchor = anchor;
    }

    public override void Render(SkiaVisualizationContext ctx)
    {
        ctx.UseSkia((canvas) =>
        {
            canvas.Save();
            var offset = _anchor switch
            {
                RectAnchor.TopLeft => new Vector2(0, 0),
                RectAnchor.TopRight => new Vector2(-_size.X, 0),
                RectAnchor.BottomLeft => new Vector2(0, -_size.Y),
                RectAnchor.BottomRight => new Vector2(-_size.X, -_size.Y),
                RectAnchor.Center => new Vector2(-_size.X / 2, -_size.Y / 2),
                RectAnchor.Top => new Vector2(-_size.X / 2, 0),
                RectAnchor.Bottom => new Vector2(-_size.X / 2, -_size.Y),
                RectAnchor.Left => new Vector2(0, -_size.Y / 2),
                RectAnchor.Right => new Vector2(-_size.X, -_size.Y / 2),
                _ => throw new ArgumentOutOfRangeException()
            };
            canvas.Translate(offset.X, offset.Y);
            canvas.RotateDegrees(_rotation, _position.X + _size.X / 2, _position.Y + _size.Y / 2);
            var rect = new SKRect(_position.X, _position.Y, _position.X + _size.X, _position.Y + _size.Y);
            canvas.DrawRect(rect, new SKPaint
            {
                Color = _bgColor.ToSKColor(),
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            });
            canvas.Restore();
        });
    }
}
