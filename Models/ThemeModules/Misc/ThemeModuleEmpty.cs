using System.Text.Json.Serialization;

namespace AudioVisualizer.Models.ThemeModules.Misc;

public class ThemeModuleEmpty : ThemeModuleMisc
{
    public override string Identifier => "empty";
    [JsonIgnore]
    public override string Name => "Empty";
}
