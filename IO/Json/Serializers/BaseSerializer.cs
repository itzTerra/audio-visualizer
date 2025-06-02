using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using AudioVisualizer.IO.Json.Converters;

namespace AudioVisualizer.IO.Json.Serializers;

public class BaseSerializer<T> : ISerializer<T>
{
    public static readonly JsonSerializerOptions DefaultOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        // NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
        // ReferenceHandler = ReferenceHandler.Preserve,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
            new ColorJsonConverter(),
            new Vector2Converter(),
            // new ThemeModuleJsonConverter()
        }
    };

    public virtual T? Deserialize(string? textContent)
    {
        if (string.IsNullOrEmpty(textContent))
            return default;
        try
        {
            return JsonSerializer.Deserialize<T>(textContent, DefaultOptions);
        }
        catch
        {
            return default;
        }
    }
    public virtual string Serialize(T value)
    {
        return JsonSerializer.Serialize(value, DefaultOptions);
    }
}
