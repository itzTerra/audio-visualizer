using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AudioVisualizer.Messages;

class TextBoxDialogResultMessage : ValueChangedMessage<string>
{
    public TextBoxDialogResultMessage(string value) : base(value) { }
}
