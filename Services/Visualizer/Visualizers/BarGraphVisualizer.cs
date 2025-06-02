using System;
using System.Collections.Generic;
using System.Numerics;
using AudioVisualizer.Extensions;
using AudioVisualizer.Utils;
using SkiaSharp;
using SoundFlow.Interfaces;

namespace AudioVisualizer.Services.Visualizer;

// Inspired by https://lsxprime.github.io/soundflow-docs/advanced-topics/#custom-visualizers
public class BarGraphVisualizer : VisualizerBase
{
    public override bool IsAudioDataRequired => true;

    private Vector2 _position;
    private float _intensity;
    private SKColor _color;
    private int _stepSizePx;
    private float _rotation;
    private bool _mirrored;
    private LineAnchor _anchor;

    private List<float> _levels { get; } = [];
    private List<float> _currentLevels { get; } = [];
    private int _sgn;

    public BarGraphVisualizer(Vector2 position, float intensity, Color color, int stepSizePx, float rotation, bool mirrored, LineAnchor anchor)
    {
        _position = position;
        _intensity = intensity;
        _color = color.ToSKColor();
        _stepSizePx = stepSizePx;
        _rotation = rotation;
        _mirrored = mirrored;
        _anchor = anchor;

        _sgn = mirrored ? -1 : 1;
    }

    public override void ProcessOnAudioData(Span<float> audioData)
    {
        if (!audioData.ContainsAnyExcept(0))
        {
            return;
        }

        lock (_levels)
        {
            _levels.Clear();
            foreach (var sample in audioData)
            {
                _levels.Add(Math.Abs(sample));
            }

            lock (_currentLevels)
            {
                // Ensure _currentLevels matches the size of _levels
                while (_currentLevels.Count < _levels.Count)
                {
                    _currentLevels.Add(0);
                }
                while (_currentLevels.Count > _levels.Count)
                {
                    _currentLevels.RemoveAt(_currentLevels.Count - 1);
                }
                for (int i = 0; i < _currentLevels.Count; i++)
                {
                    _currentLevels[i] = float.Lerp(_currentLevels[i], _levels[i], LerpFactor);
                }
            }
        }

        OnVisualizationUpdated();
    }

    public override void Render(SkiaVisualizationContext ctx)
    {
        if (_currentLevels.Count == 0)
        {
            return;
        }

        ctx.UseSkia((canvas) =>
        {
            canvas.Save();
            lock (_currentLevels)
            {
                Vector2 offset = _anchor switch
                {
                    LineAnchor.Left => new Vector2(_position.X, _position.Y),
                    LineAnchor.Right => new Vector2(_position.X - _currentLevels.Count * _stepSizePx, _position.Y),
                    LineAnchor.Center => new Vector2(_position.X - _currentLevels.Count * _stepSizePx / 2, _position.Y),
                    _ => throw new ArgumentOutOfRangeException(nameof(_anchor), _anchor, null)
                };
                canvas.Translate(offset.X, offset.Y);
                canvas.RotateDegrees(_rotation);
                for (int i = 0; i < _currentLevels.Count; i++)
                {
                    float x = i * _stepSizePx;
                    float height = _currentLevels[i] * _intensity;
                    canvas.DrawRect(x, 0, _sgn * _stepSizePx, _sgn * -height, new() { Color = _color });
                }
                canvas.Restore();
            }
        });
    }
}
