using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AudioVisualizer.Messages;

class ExportImageMessage : ValueChangedMessage<string>
{
    public ExportImageMessage(string filePath) : base(filePath) { }
}
