using System;
using System.Collections.Generic;
using System.Numerics;
using SoundFlow.Interfaces;

namespace AudioVisualizer.Services.Visualizer;

// Inspired by https://github.com/LSXPrime/SoundFlow/blob/master/Src/Visualization/WaveformVisualizer.cs
public class CustomWaveformVisualizer : VisualizerBase
{
    public override bool IsAudioDataRequired => true;

    private List<float> _waveform { get; } = [];
    private List<float> _smoothedWaveform { get; } = [];

    private Vector2 _position;
    private float _intensity;
    private Color _waveformColor;
    private Vector2 _size;

    public CustomWaveformVisualizer(Vector2 position, float intensity, Vector2 size, Color waveformColor)
    {
        _position = position;
        _intensity = intensity;
        _size = size;
        _waveformColor = waveformColor;
    }

    public override void ProcessOnAudioData(Span<float> audioData)
    {
        if (!audioData.ContainsAnyExcept(0))
        {
            return;
        }

        lock (_waveform)
        {
            _waveform.Clear();
            _waveform.AddRange(audioData.ToArray());

            lock (_smoothedWaveform)
            {
                // Ensure _smoothedWaveform has the same size as _waveform
                if (_smoothedWaveform.Count != _waveform.Count)
                {
                    _smoothedWaveform.Clear();
                    _smoothedWaveform.AddRange(_waveform);
                }

                for (int i = 0; i < _waveform.Count; i++)
                {
                    _smoothedWaveform[i] = float.Lerp(_smoothedWaveform[i], _waveform[i], LerpFactor);
                }
            }
        }

        OnVisualizationUpdated();
    }

    public override void Render(SkiaVisualizationContext ctx)
    {
        if (_smoothedWaveform.Count < 2)
        {
            return;
        }

        lock (_smoothedWaveform)
        {
            var midY = _size.Y / 2;
            var xStep = _size.X / (_smoothedWaveform.Count - 1);
            for (var i = 0; i < _smoothedWaveform.Count - 1; i++)
            {
                var x1 = i * xStep;
                var y1 = midY + _smoothedWaveform[i] * _intensity;
                var x2 = (i + 1) * xStep;
                var y2 = midY + _smoothedWaveform[i + 1] * _intensity;

                ctx.DrawLine(_position.X + x1, _position.Y + y1, _position.X + x2, _position.Y + y2, _waveformColor);
            }
        }
    }
}
