using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia.Media;

namespace AudioVisualizer.IO.Json.Converters;

public class ColorJsonConverter : JsonConverter<Color>
{
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var hex = reader.GetString();
        return Color.Parse(hex ?? "#000000");
    }

    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}

