using System.Linq;
using AudioVisualizer.Models.ThemeModules;

namespace AudioVisualizer.ViewModels.Observables;

public partial class ThemeModuleObservableModel : ViewModelBase
{
    private ThemeModuleBase _themeModule = null!;
    public ThemeModuleBase ThemeModule
    {
        get => _themeModule;
        set
        {
            if (value is not null)
            {
                SetProperty(ref _themeModule, value);
                var found = ThemeModules.Modules.FirstOrDefault(m => m.Identifier == value.Identifier);
                if (found is not null)
                {
                    var selectedIndex = ThemeModules.Modules.IndexOf(found);
                    if (selectedIndex != -1 && selectedIndex != SelectedIndex)
                    {
                        SelectedIndex = selectedIndex;
                    }
                }
            }
        }
    }

    private int _selectedIndex;
    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            if (value < 0 || value >= ThemeModules.Modules.Count)
                return;
            SetProperty(ref _selectedIndex, value);
            var selectedModule = ThemeModules.Modules[value];
            if (selectedModule.Identifier != ThemeModule.Identifier)
            {
                ThemeModule = selectedModule.Clone();
            }
        }
    }

    public ThemeModuleObservableModel(ThemeModuleBase themeModule)
    {
        _selectedIndex = 0;
        ThemeModule = themeModule.Clone();
    }

    public override string ToString() => ThemeModule.ToString();

    public override bool Equals(object? obj)
    {
        if (obj is ThemeModuleObservableModel other)
        {
            return ThemeModule.Equals(other.ThemeModule);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return ThemeModule.GetHashCode();
    }
}
