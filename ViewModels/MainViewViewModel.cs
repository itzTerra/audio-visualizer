using System;
using System.Threading.Tasks;
using AudioVisualizer.Controls.Dialogs;
using AudioVisualizer.Messages;
using AudioVisualizer.Models;
using AudioVisualizer.Utils;
using AudioVisualizer.ViewModels.Dialogs;
using AudioVisualizer.ViewModels.Observables;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Ursa.Controls;

namespace AudioVisualizer.ViewModels;

public partial class MainViewViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _importPath = string.Empty;

    private ThemeNodeObservableModel? _selectedTheme;
    public ThemeNodeObservableModel? SelectedTheme
    {
        get => _selectedTheme;
        set
        {
            if (value is not null && value.IsCategory) return;
            SetProperty(ref _selectedTheme, value);
        }
    }

    [ObservableProperty]
    private AvaloniaDictionary<string, ThemeNodeObservableModel> _dirtyThemes = new();

    private ThemeNodeModel? _currentTheme;
    public ThemeNodeModel? CurrentTheme
    {
        get => _currentTheme;
        set
        {
            if (value is not null && value.IsCategory) return;
            SetProperty(ref _currentTheme, value);
            WeakReferenceMessenger.Default.Send(new ChangeAppNameMessage(value?.Name));
        }
    }

    [ObservableProperty]
    private SettingsObservableModel _settings;

    public MediaPlayerViewModel MediaPlayer { get; }
    public ThemeExplorerViewModel ThemeExplorer { get; }
    public ThemeInspectorViewModel ThemeInspector { get; }


    public MainViewViewModel()
    {
        Settings = SettingsObservableModel.FromStorage();
        MediaPlayer = new MediaPlayerViewModel(this);
        ThemeExplorer = new ThemeExplorerViewModel(this);
        ThemeInspector = new ThemeInspectorViewModel(this);

        WeakReferenceMessenger.Default.Register<SettingsUpdatedMessage>(this, (r, m) =>
        {
            Settings = new SettingsObservableModel(m.Value);
        });

        DirtyThemes.CollectionChanged += (s, e) =>
        {
            OnPropertyChanged(nameof(DirtyThemes));
        };
    }

    partial void OnSettingsChanged(SettingsObservableModel value)
    {
        var settingsThemeVariant = value.Theme switch
        {
            AppThemeVariant.Light => ThemeVariant.Light,
            AppThemeVariant.Dark => ThemeVariant.Dark,
            AppThemeVariant.System => ThemeVariant.Default,
            _ => throw new ArgumentOutOfRangeException(nameof(value.Theme), value.Theme, null)
        };
        if (Application.Current!.RequestedThemeVariant != settingsThemeVariant)
        {
            Application.Current!.RequestedThemeVariant = settingsThemeVariant;
        }
    }

    [RelayCommand]
    private void NavigateSettings()
    {
        WeakReferenceMessenger.Default.Send(new NavigateMessage(ViewType.Settings));
    }

    [RelayCommand]
    private void SelectCurrentTheme()
    {
        if (CurrentTheme is null) return;
        SelectedTheme = new ThemeNodeObservableModel(CurrentTheme);
    }

    [RelayCommand]
    private void Import()
    {
        if (string.IsNullOrEmpty(ImportPath))
        {
            return;
        }
        WeakReferenceMessenger.Default.Send(new AudioImportMessage(ImportPath));
    }

    [RelayCommand]
    private async Task Export()
    {
        await OverlayDialog.ShowCustomModal<ExportDialog, ExportDialogViewModel, DialogResult>(new ExportDialogViewModel(Settings.DefaultExportDirectory), null, options: new OverlayDialogOptions()
        {
            FullScreen = false,
            HorizontalAnchor = HorizontalPosition.Center,
            VerticalAnchor = VerticalPosition.Center,
            Mode = DialogMode.Question,
            CanDragMove = true,
            IsCloseButtonVisible = true,
            CanResize = false,
        });
    }
}
