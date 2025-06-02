namespace AudioVisualizer.Services.Visualizer;

using AudioVisualizer.Extensions;

public class BackgroundVisualizer : VisualizerBase
{
    public override bool IsAudioDataRequired => false;
    public override int Priority => 100;

    private Avalonia.Media.Color _bgColor;

    public BackgroundVisualizer(Avalonia.Media.Color bgColor)
    {
        _bgColor = bgColor;
    }

    public override void Render(SkiaVisualizationContext ctx)
    {
        ctx.UseSkia((canvas) =>
        {
            canvas.Clear(_bgColor.ToSKColor());
        });
    }
}
