using AudioVisualizer.ViewModels;
using Avalonia.Controls;

namespace AudioVisualizer.Views;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();
        DataContext = new SettingsViewModel();
    }
}
