using System.Text.Json.Serialization;
using Avalonia.Media;

namespace AudioVisualizer.Models.ThemeModules.Core;

public class ThemeModuleWaveRadial : ThemeModuleWave
{
    public override string Identifier { get => "radialWave"; }
    [JsonIgnore]
    public override string Name { get => "Radial Wave"; }

    private float _innerRadius = 50f;
    [JsonRequired]
    public float InnerRadius
    {
        get => _innerRadius;
        set => SetProperty(ref _innerRadius, value);
    }

    private Color _innerColor = Colors.Transparent;
    [JsonRequired]
    public Color InnerColor
    {
        get => _innerColor;
        set => SetProperty(ref _innerColor, value);
    }
}
