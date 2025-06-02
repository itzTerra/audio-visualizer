namespace AudioVisualizer.IO.Json.Serializers;

public interface ISerializer<T>
{
    public T? Deserialize(string? textContent);
    public string Serialize(T value);
}
