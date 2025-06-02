using System;
using System.Text.Json;
using AudioVisualizer.Models;

namespace AudioVisualizer.IO.Json.Serializers;

public class ThemeNodesSerializer : ISerializer<ThemeNodeModel[]>
{
    public ThemeNodesSerializer() { }

    public ThemeNodeModel[] Deserialize(string? textContent)
    {
        if (string.IsNullOrEmpty(textContent))
            return Array.Empty<ThemeNodeModel>();
        try
        {
            return JsonSerializer.Deserialize<ThemeNodeModel[]>(textContent, BaseSerializer<object>.DefaultOptions) ?? Array.Empty<ThemeNodeModel>();
        }
        catch (Exception)
        {
            return Array.Empty<ThemeNodeModel>();
        }
    }

    public string Serialize(ThemeNodeModel[] value)
    {
        return JsonSerializer.Serialize(value, BaseSerializer<object>.DefaultOptions);
    }
}
