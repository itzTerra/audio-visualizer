using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AudioVisualizer.Messages;

class AudioImportMessage : ValueChangedMessage<string>
{
    public AudioImportMessage(string filePath) : base(filePath) { }
}
