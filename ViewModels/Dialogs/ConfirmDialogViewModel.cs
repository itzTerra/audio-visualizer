using CommunityToolkit.Mvvm.ComponentModel;

namespace AudioVisualizer.ViewModels.Dialogs;

public partial class ConfirmDialogViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _message = "Are you sure you want to proceed?";

    public ConfirmDialogViewModel() { }

    public ConfirmDialogViewModel(string message)
    {
        Message = message;
    }
}
