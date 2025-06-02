using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AudioVisualizer.Models.ThemeModules;

namespace AudioVisualizer.Models;

public class ThemeNodeModel
{
    [JsonRequired]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [JsonRequired]
    public string Name { get; set; }
    [JsonRequired]
    public bool Readonly { get; set; } = false;
    [JsonRequired]
    public List<ThemeModuleBase> Modules { get; set; } = new();

    /// <summary>
    /// To distinguish between an empty category and a theme with no modules
    /// </summary>
    [JsonRequired]
    public bool IsCategory { get; set; } = false;
    [JsonRequired]
    public List<ThemeNodeModel> Nodes { get; set; } = new();

    public ThemeNodeModel(string name, bool isCategory, bool readOnly = false, List<ThemeModuleBase>? modules = null)
    {
        Name = name;
        IsCategory = isCategory;
        Readonly = readOnly;
        if (modules != null)
        {
            Modules = modules;
        }
    }

    public override string? ToString() => Name;

    public override bool Equals(object? obj)
    {
        if (obj is ThemeNodeModel other)
        {
            return Id == other.Id && Name == other.Name && Readonly == other.Readonly && IsCategory == other.IsCategory && Modules.SequenceEqual(other.Modules);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name, Readonly, IsCategory, Modules);
    }

    public ThemeNodeModel Clone()
    {
        return new ThemeNodeModel(Name, IsCategory, Readonly, Modules.Select(m => m.Clone()).ToList());
    }
}
