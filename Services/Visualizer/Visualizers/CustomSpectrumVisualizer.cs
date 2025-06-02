namespace AudioVisualizer.Services.Visualizer;

using System;
using System.Linq;
using System.Numerics;
using SoundFlow.Interfaces;
using SoundFlow.Utils;

// Inspired by https://github.com/LSXPrime/SoundFlow/blob/ab5dfd17968acb6fd4667d93bcbc6b3452103774/Src/Visualization/SpectrumVisualizer.cs
public class CustomSpectrumVisualizer : VisualizerBase
{
    public override bool IsAudioDataRequired => true;

    private Vector2 _position;
    private Vector2 _size;
    private float _intensity;
    private Color _barColor;
    private int _fftSize;

    private readonly float[] _spectrumData;
    private readonly Complex[] _fftBuffer;
    private readonly float[] _window;
    private readonly float _barWidth;

    public CustomSpectrumVisualizer(Vector2 position, Vector2 size, float intensity, Color barColor, int fftSize)
    {
        _position = position;
        _size = size;
        _intensity = intensity;
        _barColor = barColor;
        _fftSize = fftSize;

        _spectrumData = new float[_fftSize / 2];
        _fftBuffer = new Complex[_fftSize];
        _window = MathHelper.HammingWindow(_fftSize);
        _barWidth = Math.Max(1, _size.X / (_fftSize / 2));
    }

    public override void ProcessOnAudioData(Span<float> audioData)
    {
        if (!audioData.ContainsAnyExcept(0))
        {
            return;
        }

        // Apply window function and copy to FFT buffer
        var numSamples = Math.Min(audioData.Length, _fftSize);
        for (var i = 0; i < numSamples; i++)
        {
            if (float.IsNaN(audioData[i]) || float.IsInfinity(audioData[i]))
            {
                _fftBuffer[i] = Complex.Zero;
            }
            _fftBuffer[i] = new Complex(audioData[i] * _window[i], 0);
        }

        for (var i = numSamples; i < _fftSize; i++)
        {
            _fftBuffer[i] = Complex.Zero;
        }

        // Perform FFT
        MathHelper.Fft(_fftBuffer);

        // Calculate magnitude spectrum
        float normalizationFactor = _fftSize * _window.Sum(); // Normalize by FFT size and window sum
        lock (_spectrumData)
        {
            for (var i = 0; i < _fftSize / 2; i++)
            {
                _spectrumData[i] = (float)_fftBuffer[i].Magnitude / normalizationFactor;
            }
        }

        OnVisualizationUpdated();
    }

    public override void Render(SkiaVisualizationContext ctx)
    {
        lock (_spectrumData)
        {
            for (var i = 0; i < _spectrumData.Length; i++)
            {
                var barHeight = _spectrumData[i] * _intensity * _size.Y / 2;
                barHeight = Math.Clamp(barHeight, 0f, _size.Y - 1f);

                var y = _size.Y - barHeight;
                ctx.DrawRectangle(_position.X + i * _barWidth, _position.Y + y, _barWidth, barHeight, _barColor);
            }
        }
    }
}
