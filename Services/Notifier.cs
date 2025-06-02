using AudioVisualizer.Messages;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.Messaging;

namespace AudioVisualizer.Services;

public class Notifier
{
    private readonly WindowNotificationManager _notificationManager;

    public Notifier(TopLevel mainWindow)
    {
        _notificationManager = new WindowNotificationManager(mainWindow)
        {
            MaxItems = 3,
            Position = NotificationPosition.TopRight
        };

        WeakReferenceMessenger.Default.Register<NotifyMessage>(this, (r, m) =>
        {
            Show(m.Value.Message, m.Value.Title, m.Value.Type);
        });
    }

    public void Show(string message, string title = "Notification", NotificationType type = NotificationType.Information)
    {
        _notificationManager.Show(new Notification(title, message), type: type);
    }

    public static void Error(string message, string title = "Error")
    {
        WeakReferenceMessenger.Default.Send(new NotifyMessage(new NotifyMessageContent(title, message, NotificationType.Error)));
    }
    public static void Warning(string message, string title = "Warning")
    {
        WeakReferenceMessenger.Default.Send(new NotifyMessage(new NotifyMessageContent(title, message, NotificationType.Warning)));
    }
    public static void Info(string message, string title = "Info")
    {
        WeakReferenceMessenger.Default.Send(new NotifyMessage(new NotifyMessageContent(title, message, NotificationType.Information)));
    }
    public static void Success(string message, string title = "Success")
    {
        WeakReferenceMessenger.Default.Send(new NotifyMessage(new NotifyMessageContent(title, message, NotificationType.Success)));
    }
}
