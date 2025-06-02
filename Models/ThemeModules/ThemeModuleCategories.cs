using System.Collections.Generic;

namespace AudioVisualizer.Models.ThemeModules;

public record ThemeModuleCategoryInfo(
    ThemeModuleCategory Category,
    string DisplayName,
    string Icon
);

public static class ThemeModuleCategories
{
    public static readonly List<ThemeModuleCategoryInfo> All = new()
    {
        new(ThemeModuleCategory.Core, "Core", "⚙️"),
        new(ThemeModuleCategory.Particle, "Particles", "✨"),
        new(ThemeModuleCategory.Background, "Backgrounds", "🌌"),
        new(ThemeModuleCategory.Misc, "Misc.", "🎨")
    };
}
