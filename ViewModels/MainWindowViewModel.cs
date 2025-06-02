using System;
using AudioVisualizer.Messages;
using AudioVisualizer.Utils;
using AudioVisualizer.Views;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace AudioVisualizer.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _applicationTitle = AppConfig.AppName;

    private MainView _mainView;
    private SettingsView _settingsView;

    [ObservableProperty]
    private UserControl _currentView;

    public MainWindowViewModel()
    {
        _mainView = new MainView();
        _settingsView = new SettingsView();

        CurrentView = _mainView;

        WeakReferenceMessenger.Default.Register<ChangeAppNameMessage>(this, (r, m) =>
        {
            if (string.IsNullOrEmpty(m.Value))
            {
                ApplicationTitle = AppConfig.AppName;
                return;
            }
            ApplicationTitle = $"{m.Value} - {AppConfig.AppName}";
        });

        WeakReferenceMessenger.Default.Register<NavigateMessage>(this, (r, m) =>
        {
            CurrentView = m.Value switch
            {
                ViewType.Main => _mainView,
                ViewType.Settings => _settingsView,
                _ => throw new ArgumentOutOfRangeException(nameof(m.Value), m.Value, null)
            };
            WeakReferenceMessenger.Default.Send(new ChangeAppNameMessage(m.Value switch
            {
                ViewType.Main => (_mainView.DataContext as MainViewViewModel)!.CurrentTheme?.Name,
                ViewType.Settings => "Settings",
                _ => throw new ArgumentOutOfRangeException(nameof(m.Value), m.Value, null)
            }));
        });
    }
}
