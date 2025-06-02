using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AudioVisualizer.Messages;

class ExportImageResponseMessage : ValueChangedMessage<string?>
{
    public ExportImageResponseMessage(string? error) : base(error) { }
}
