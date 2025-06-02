using System;
using SoundFlow.Abstracts;
using SoundFlow.Components;
using SoundFlow.Interfaces;

namespace AudioVisualizer.Services.Visualizer;

public abstract class VisualizerBase : AudioAnalyzer, IVisualizer
{
    protected const float LerpFactor = 0.25f;

    public virtual bool IsAudioDataRequired { get; } = false;
    public SoundPlayer SoundPlayer { get; set; } = null!;
    public virtual int Priority { get; } = 0;

    public event EventHandler? VisualizationUpdated;

    protected virtual void OnVisualizationUpdated()
    {
        VisualizationUpdated?.Invoke(this, EventArgs.Empty);
    }

    protected override void Analyze(Span<float> buffer)
    {
        ProcessOnAudioData(buffer);
    }

    public virtual void ProcessOnAudioData(Span<float> audioData)
    {
    }

    public void Render(IVisualizationContext context)
    {
        if (context is SkiaVisualizationContext skiaContext)
        {
            Render(skiaContext);
        }
        else
        {
            throw new NotSupportedException("Unsupported visualization context type.");
        }
    }

    public virtual void Render(SkiaVisualizationContext ctx) { }

    public virtual void Dispose()
    {
    }
}
