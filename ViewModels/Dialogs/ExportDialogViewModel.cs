using System;
using AudioVisualizer.Messages;
using AudioVisualizer.Models.Validators;
using AudioVisualizer.Services;
using AudioVisualizer.ViewModels.Observables;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Irihi.Avalonia.Shared.Contracts;

namespace AudioVisualizer.ViewModels.Dialogs;

public partial class ExportDialogViewModel : ObservableModelBase, IDialogContext
{
    [ObservableProperty]
    private int _selectedTypeIndex = 0;

    [ObservableProperty]
    private bool _includeAudio = true;

    public string SuggestedStartPath { get; }

    private string _destinationPath = "";
    [ValidDirPath(AllowEmptyPath = true)]
    public string DestinationPath
    {
        get => _destinationPath;
        set => SetProperty(ref _destinationPath, value, true);
    }

    [ObservableProperty]
    private bool _isExportLoading = false;

    public ExportDialogViewModel(string defaultExportDirectory)
    {
        SuggestedStartPath = defaultExportDirectory;
    }

    public event EventHandler<object?>? RequestClose;

    [RelayCommand]
    private void OK()
    {
        IsExportLoading = true;
        WeakReferenceMessenger.Default.Register<ExportImageResponseMessage>(this, (r, m) =>
        {
            WeakReferenceMessenger.Default.Unregister<ExportImageResponseMessage>(this);
            IsExportLoading = false;
            if (m.Value is null)
            {
                RequestClose?.Invoke(this, true);
            }
            else
            {
                Notifier.Error(m.Value);
            }
        });
        WeakReferenceMessenger.Default.Send(new ExportImageMessage(DestinationPath));
    }

    [RelayCommand]
    private void Cancel()
    {
        RequestClose?.Invoke(this, false);
    }

    public void Close()
    {
        RequestClose?.Invoke(this, null);
    }
}
