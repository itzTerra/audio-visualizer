namespace AudioVisualizer.Services;

using Microsoft.Extensions.Logging;

public class Logger
{
    private static ILogger? _instance;

    public ILogger Instance => _instance!;

    public Logger()
    {
        if (_instance is null)
        {
            using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
            _instance = factory.CreateLogger("App");
        }
    }
}
