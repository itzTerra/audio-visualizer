using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using SoundFlow.Components;
using SoundFlow.Interfaces;

namespace AudioVisualizer.Services.Visualizer;

/// <summary>
/// Wrapper for control of multiple visualizers
/// </summary>
public class VisualizerService : IDisposable
{
    private SoundPlayer? _soundPlayer;
    private readonly List<VisualizerBase> _visualizers = [];
    private readonly List<VisualizerBase> _visualizersToConnect = [];
    private readonly Timer _timer = new(1000 / 60); // 60 FPS
    // private ThrottleDispatcher _throttleDispatcher = new ThrottleDispatcher(TimeSpan.FromMilliseconds(1000 / 60));
    private readonly MemoryVisualizer _memoryVisualizer = new();

    public event EventHandler? VisualizationUpdated;

    public bool IsRunning { get; private set; } = false;

    public VisualizerService()
    {
        _timer.Elapsed += OnVisualizationUpdated;
        _timer.AutoReset = true;
    }

    public void ConnectTo(SoundPlayer soundPlayer)
    {
        _soundPlayer = soundPlayer;
        _soundPlayer.AddAnalyzer(_memoryVisualizer);
        if (_visualizersToConnect.Count > 0)
        {
            RegisterVisualizers(_visualizersToConnect);
            _visualizersToConnect.Clear();
        }
    }

    public void RegisterVisualizer(VisualizerBase visualizer)
    {
        if (visualizer.IsAudioDataRequired)
        {
            if (_soundPlayer is null)
            {
                _visualizersToConnect.Add(visualizer);
                return;
            }
            _visualizers.Add(visualizer);
            visualizer.SoundPlayer = _soundPlayer!;
            _soundPlayer!.AddAnalyzer(visualizer);
            if (!IsRunning && _memoryVisualizer.AudioData.Count > 0)
            {
                visualizer.ProcessOnAudioData(_memoryVisualizer.AudioData.ToArray());
                OnVisualizationUpdated(this, EventArgs.Empty);
            }
        }
        else
        {
            _visualizers.Add(visualizer);
            if (!IsRunning)
            {
                OnVisualizationUpdated(this, EventArgs.Empty);
            }
        }
    }

    public void RegisterVisualizers(IEnumerable<VisualizerBase> visualizers)
    {
        foreach (var visualizer in visualizers)
        {
            RegisterVisualizer(visualizer);
        }
    }

    protected virtual void OnVisualizationUpdated(object? sender, EventArgs e)
    {
        // _throttleDispatcher.Throttle(() => VisualizationUpdated?.Invoke(sender, e));
        VisualizationUpdated?.Invoke(sender, e);
    }

    public void Render(IVisualizationContext context)
    {
        foreach (var visualizer in _visualizers.OrderByDescending(v => v.Priority))
        {
            visualizer.Render(context);
        }
    }

    public void Start()
    {
        _timer.Start();
        IsRunning = true;
    }

    public void Stop()
    {
        _timer.Stop();
        IsRunning = false;
    }

    public void Dispose()
    {
        Stop();
        foreach (var visualizer in _visualizers)
        {
            _soundPlayer?.RemoveAnalyzer(visualizer);
            visualizer.Dispose();
        }
        _visualizers.Clear();
    }
}
