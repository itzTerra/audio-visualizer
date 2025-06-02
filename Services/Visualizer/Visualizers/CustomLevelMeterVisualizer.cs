using System;
using System.Numerics;
using SoundFlow.Interfaces;

namespace AudioVisualizer.Services.Visualizer;

// Inspired by https://lsxprime.github.io/soundflow-docs/advanced-topics/#custom-visualizers
public class CustomLevelMeterVisualizer : VisualizerBase
{
    private const float PeakHoldDuration = 1000; // Milliseconds

    public override bool IsAudioDataRequired => true;

    private Vector2 _position;
    private Vector2 _size;
    private Color _barColor;
    private Color _peakHoldColor;

    private DateTime _lastPeakTime = DateTime.MinValue;
    private float _peakHoldLevel; // Normalized peak hold level (0-1)
    private float _rms; // Normalized RMS level (0-1)

    public CustomLevelMeterVisualizer(Vector2 position, Vector2 size, Color barColor, Color peakHoldColor)
    {
        _position = position;
        _size = size;
        _barColor = barColor;
        _peakHoldColor = peakHoldColor;
    }

    public override void ProcessOnAudioData(Span<float> audioData)
    {
        if (!audioData.ContainsAnyExcept(0))
        {
            return;
        }

        var peak = 0f;
        var sumSquares = 0f;

        if (!Vector.IsHardwareAccelerated || audioData.Length < Vector<float>.Count)
        {
            // Scalar processing
            foreach (var sample in audioData)
            {
                var absSample = Math.Abs(sample);
                if (absSample > peak)
                {
                    peak = absSample;
                }
                sumSquares += sample * sample;
            }
        }
        else
        {
            // SIMD processing
            var vectorSize = Vector<float>.Count;
            var i = 0;
            var sumSquaresVector = Vector<float>.Zero;

            for (; i <= audioData.Length - vectorSize; i += vectorSize)
            {
                Vector<float> vector = new(audioData.Slice(i, vectorSize));
                var absVector = Vector.Abs(vector);

                // Find the maximum element in absVector
                var maxInVector = absVector[0];
                for (var j = 1; j < vectorSize; j++)
                {
                    maxInVector = Math.Max(maxInVector, absVector[j]);
                }

                peak = Math.Max(peak, maxInVector);
                sumSquaresVector += vector * vector;
            }

            // Reduce the sum of squares vector
            for (var j = 0; j < vectorSize; j++)
            {
                sumSquares += sumSquaresVector[j];
            }

            // Handle remaining elements
            for (; i < audioData.Length; i++)
            {
                var sample = audioData[i];
                var absSample = Math.Abs(sample);
                if (absSample > peak)
                {
                    peak = absSample;
                }
                sumSquares += sample * sample;
            }
        }

        _rms = MathF.Sqrt(sumSquares / audioData.Length);

        // Update peak hold
        if (_rms > _peakHoldLevel)
        {
            _peakHoldLevel = _rms;
            _lastPeakTime = DateTime.Now;
        }
        else if ((DateTime.Now - _lastPeakTime).TotalMilliseconds > PeakHoldDuration)
        {
            _peakHoldLevel = _rms; // Decay peak hold
        }

        OnVisualizationUpdated();
    }

    public override void Render(SkiaVisualizationContext ctx)
    {
        // Draw level bar
        var levelHeight = _rms * _size.Y;
        ctx.DrawRectangle(_position.X, _position.Y + _size.Y - levelHeight, _size.X, levelHeight, _barColor);

        // Draw peak hold indicator
        var peakHoldY = _size.Y - _peakHoldLevel * _size.Y;
        ctx.DrawLine(_position.X, _position.Y + peakHoldY, _position.X + _size.X, _position.Y + peakHoldY, _peakHoldColor);
    }
}
