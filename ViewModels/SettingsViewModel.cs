using System.Threading.Tasks;
using AudioVisualizer.IO;
using AudioVisualizer.Messages;
using AudioVisualizer.ViewModels.Observables;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AudioVisualizer.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    [ObservableProperty]
    private SettingsObservableModel _settings;

    private readonly IStorageService _storageService = new FileStorageService();

    public SettingsViewModel()
    {
        Settings = new(_storageService.LoadSettings());
        WeakReferenceMessenger.Default.Send(new SettingsUpdatedMessage(Settings.ToModel()));
    }

    [RelayCommand]
    private async Task Apply()
    {
        await _storageService.SaveSettings(Settings.ToModel());
        WeakReferenceMessenger.Default.Send(new SettingsUpdatedMessage(Settings.ToModel()));
    }

    [RelayCommand]
    private async Task Close()
    {
        WeakReferenceMessenger.Default.Send(new NavigateMessage(ViewType.Main));
        Settings = new(await _storageService.LoadSettingsAsync());
    }
}
