using AudioVisualizer.Models;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AudioVisualizer.Messages;

class SettingsUpdatedMessage : ValueChangedMessage<SettingsModel>
{
    public SettingsUpdatedMessage(SettingsModel value) : base(value) { }
}
