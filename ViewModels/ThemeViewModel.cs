using System.Collections.Generic;
using AudioVisualizer.ViewModels;
using AudioVisualizer.ViewModels.Observables;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AudioVisualizer.ViewModel;

public partial class ThemeViewModel : ViewModelBase
{
    [ObservableProperty]
    private ThemeNodeObservableModel _theme;

    public ThemeViewModel(ThemeNodeObservableModel theme)
    {
        Theme = theme;
    }

    public override bool Equals(object? obj)
    {
        return obj is ThemeViewModel model &&
               EqualityComparer<ThemeNodeObservableModel>.Default.Equals(Theme, model.Theme);
    }

    public override int GetHashCode()
    {
        return Theme.GetHashCode();
    }
}
