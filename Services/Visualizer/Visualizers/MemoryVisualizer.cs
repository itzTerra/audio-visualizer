using System;
using System.Collections.Generic;

namespace AudioVisualizer.Services.Visualizer;

public class MemoryVisualizer : VisualizerBase
{
    public override bool IsAudioDataRequired => true;

    public List<float> AudioData { get; } = [];

    public override void ProcessOnAudioData(Span<float> audioData)
    {
        if (!audioData.ContainsAnyExcept(0))
        {
            return;
        }
        AudioData.Clear();
        AudioData.AddRange(audioData.ToArray());
    }
}
