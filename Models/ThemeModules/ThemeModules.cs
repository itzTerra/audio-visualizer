using System.Collections.Generic;
using System.Collections.ObjectModel;
using AudioVisualizer.Models.ThemeModules.Background;
using AudioVisualizer.Models.ThemeModules.Core;
using AudioVisualizer.Models.ThemeModules.Misc;

namespace AudioVisualizer.Models.ThemeModules;

public static class ThemeModules
{
    public static ThemeModuleBase Empty => new ThemeModuleEmpty();

    public static List<ThemeModuleBase> GetThemeModules()
    {
        return new List<ThemeModuleBase>
        {
            Empty,
            new ThemeModuleText(),
            new ThemeModuleWaveform(),
            new ThemeModuleLevelMeter(),
            // TODO add spectrum after fix
            // new ThemeModuleSpectrum(),
            new ThemeModuleWaveLinear(),
            // TODO
            // new ThemeModuleWaveRadial(),
            new ThemeModuleBackgroundSolid(),
            new ThemeModuleBox(),
        };
    }

    public static readonly ReadOnlyCollection<ThemeModuleBase> Modules = new ReadOnlyCollection<ThemeModuleBase>(GetThemeModules());
}
