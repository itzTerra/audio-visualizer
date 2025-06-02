using AudioVisualizer.Services;
using AudioVisualizer.ViewModels;
using Ursa.Controls;

namespace AudioVisualizer.Views;

public partial class MainWindow : UrsaWindow
{
    public readonly Notifier Notifier;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
        Notifier = new Notifier(this);
    }
}
