using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AudioVisualizer.Messages;

enum ViewType
{
    Main,
    Settings
}

class NavigateMessage : ValueChangedMessage<ViewType>
{
    public NavigateMessage(ViewType value) : base(value) { }
}
