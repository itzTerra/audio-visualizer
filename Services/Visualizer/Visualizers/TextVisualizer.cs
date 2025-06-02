using System.Numerics;
using AudioVisualizer.Extensions;
using Avalonia.Media;
using SkiaSharp;

namespace AudioVisualizer.Services.Visualizer;

public static class TypeFaces
{
    public readonly static SKTypeface Regular = SKTypeface.FromFamilyName("Roboto");
    public readonly static SKTypeface Bold = SKTypeface.FromFamilyName("Roboto", SKFontStyleWeight.Bold, SKFontStyleWidth.SemiExpanded, SKFontStyleSlant.Upright);
    public readonly static SKTypeface Italic = SKTypeface.FromFamilyName("Roboto", SKFontStyleWeight.Normal, SKFontStyleWidth.SemiExpanded, SKFontStyleSlant.Italic);
    public readonly static SKTypeface BoldItalic = SKTypeface.FromFamilyName("Roboto", SKFontStyleWeight.Bold, SKFontStyleWidth.SemiExpanded, SKFontStyleSlant.Italic);
}

public class TextVisualizer : VisualizerBase
{
    public override bool IsAudioDataRequired => false;

    private string _text = "Text";
    private Vector2 _position = Vector2.Zero;
    private int _fontSize = 12;
    private bool _bold = false;
    private bool _italic = false;

    private SKPaint _paint = new SKPaint
    {
        IsAntialias = true,
        Color = Colors.White.ToSKColor(),
    };

    public TextVisualizer(
        string text,
        Vector2 position,
        Color textColor,
        int fontSize,
        bool bold = false,
        bool italic = false
    )
    {
        _text = text;
        _position = position;
        _fontSize = fontSize;
        _bold = bold;
        _italic = italic;
        _paint.Color = textColor.ToSKColor();
    }

    public override void Render(SkiaVisualizationContext ctx)
    {
        ctx.UseSkia((canvas) =>
        {
            var typeface = _bold && _italic ? TypeFaces.BoldItalic :
                _bold ? TypeFaces.Bold :
                _italic ? TypeFaces.Italic : TypeFaces.Regular;
            canvas.DrawText(_text, _position.X, _position.Y, typeface.ToFont(_fontSize), _paint);
        });
    }
}
