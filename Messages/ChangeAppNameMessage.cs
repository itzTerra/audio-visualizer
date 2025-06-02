using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AudioVisualizer.Messages;

class ChangeAppNameMessage : ValueChangedMessage<string?>
{
    public ChangeAppNameMessage(string? value) : base(value) { }
}
