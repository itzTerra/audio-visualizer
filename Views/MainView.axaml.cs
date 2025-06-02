using AudioVisualizer.ViewModels;
using Avalonia.Controls;

namespace AudioVisualizer.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        DataContext = new MainViewViewModel();
    }
}
