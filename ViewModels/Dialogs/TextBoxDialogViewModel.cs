using System;
using AudioVisualizer.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Irihi.Avalonia.Shared.Contracts;

namespace AudioVisualizer.ViewModels.Dialogs;

public partial class TextBoxDialogViewModel : ViewModelBase, IDialogContext
{
    public string Title { get; }
    public string? Label { get; }

    [ObservableProperty]
    private string _text = "";

    public TextBoxDialogViewModel(string title, string? label, string? prefill = null)
    {
        Title = title;
        Label = label;
        Text = prefill ?? "";
    }

    public event EventHandler<object?>? RequestClose;

    [RelayCommand]
    private void OK()
    {
        WeakReferenceMessenger.Default.Send(new TextBoxDialogResultMessage(Text));
        RequestClose?.Invoke(this, true);
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
