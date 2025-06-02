using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AudioVisualizer.Messages;

public record NotifyMessageContent(string Title, string Message, NotificationType Type);

class NotifyMessage : ValueChangedMessage<NotifyMessageContent>
{
    public NotifyMessage(NotifyMessageContent value) : base(value) { }
}
