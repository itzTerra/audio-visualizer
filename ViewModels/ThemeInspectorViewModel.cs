using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using AudioVisualizer.Models.ThemeModules;
using AudioVisualizer.ViewModels.Observables;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AudioVisualizer.ViewModels;

public partial class ThemeInspectorViewModel : ViewModelBase
{
    public MainViewViewModel Parent { get; }

    public ReadOnlyCollection<ThemeModuleBase> ThemeModuleList => ThemeModules.GetThemeModules().AsReadOnly();

    // Inspector's copy of MainView's selected theme to be edited and synced on save
    [ObservableProperty]
    private ThemeNodeObservableModel? _editableTheme;

    public ThemeInspectorViewModel(MainViewViewModel parent)
    {
        Parent = parent;

        Parent.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(Parent.SelectedTheme))
            {
                NewThemeModuleCommand.NotifyCanExecuteChanged();
                RenameThemeCommand.NotifyCanExecuteChanged();
                SaveAsThemeCommand.NotifyCanExecuteChanged();
                DeleteThemeCommand.NotifyCanExecuteChanged();
                UseThemeCommand.NotifyCanExecuteChanged();
                SaveThemeCommand.NotifyCanExecuteChanged();

                if (Parent.SelectedTheme is null)
                {
                    EditableTheme = null;
                    return;
                }

                if (Parent.DirtyThemes.ContainsKey(Parent.SelectedTheme.Id))
                {
                    EditableTheme = Parent.DirtyThemes[Parent.SelectedTheme.Id];
                }
                else
                {
                    EditableTheme = new ThemeNodeObservableModel(Parent.SelectedTheme.ToModel());
                }
            }
        };
    }

    private void UpdateDirty(ThemeNodeObservableModel node)
    {
        if (node.Id != Parent.SelectedTheme?.Id)
        {
            UnsubscribeFromNodeChanges(node);
            return;
        }
        if (node.Equals(Parent.SelectedTheme))
        {
            Parent.DirtyThemes.Remove(node.Id);
        }
        else
        {
            Parent.DirtyThemes[node.Id] = node;
        }
    }

    partial void OnEditableThemeChanged(ThemeNodeObservableModel? oldValue, ThemeNodeObservableModel? newValue)
    {
        if (oldValue is not null)
        {
            UnsubscribeFromNodeChanges(oldValue);
        }
        if (newValue is not null)
        {
            SubscribeToNodeChanges(newValue);
        }
    }

    #region ReactiveLogic
    private readonly Dictionary<string, NotifyCollectionChangedEventHandler> _nodeCollectionChangedHandlers = new();
    private readonly Dictionary<string, PropertyChangedEventHandler> _modulePropertyChangedHandlers = new();
    private readonly Dictionary<string, PropertyChangedEventHandler> _moduleThemePropertyChangedHandlers = new();

    private void SubscribeToNodeChanges(ThemeNodeObservableModel node)
    {
        node.PropertyChanged += NodePropertyChanged;

        foreach (var module in node.Modules)
        {
            SubscribeToModuleChanges(node, module);
        }

        NotifyCollectionChangedEventHandler collectionChangedHandler = (s, e) => NodeModulesCollectionChanged(node, s, e);
        _nodeCollectionChangedHandlers[node.Id] = collectionChangedHandler;
        node.Modules.CollectionChanged += collectionChangedHandler;
    }

    private void UnsubscribeFromNodeChanges(ThemeNodeObservableModel node)
    {
        node.PropertyChanged -= NodePropertyChanged;

        foreach (var module in node.Modules)
        {
            UnsubscribeFromModuleChanges(node, module);
        }

        if (_nodeCollectionChangedHandlers.TryGetValue(node.Id, out var collectionChangedHandler))
        {
            node.Modules.CollectionChanged -= collectionChangedHandler;
            _nodeCollectionChangedHandlers.Remove(node.Id);
        }
    }

    private void SubscribeToModuleChanges(ThemeNodeObservableModel node, ThemeModuleObservableModel module)
    {
        PropertyChangedEventHandler modulePropertyChangedHandler = (itemSender, itemArgs) => UpdateDirty(node); ;
        PropertyChangedEventHandler moduleThemePropertyChangedHandler = (itemSender, itemArgs) => UpdateDirty(node); ;

        _modulePropertyChangedHandlers[node.Id] = modulePropertyChangedHandler;
        _moduleThemePropertyChangedHandlers[node.Id] = moduleThemePropertyChangedHandler;

        module.PropertyChanged += modulePropertyChangedHandler;
        module.ThemeModule.PropertyChanged += moduleThemePropertyChangedHandler;
    }

    private void UnsubscribeFromModuleChanges(ThemeNodeObservableModel node, ThemeModuleObservableModel module)
    {
        if (_modulePropertyChangedHandlers.TryGetValue(node.Id, out var modulePropertyChangedHandler))
        {
            module.PropertyChanged -= modulePropertyChangedHandler;
            _modulePropertyChangedHandlers.Remove(node.Id);
        }

        if (_moduleThemePropertyChangedHandlers.TryGetValue(node.Id, out var moduleThemePropertyChangedHandler))
        {
            module.ThemeModule.PropertyChanged -= moduleThemePropertyChangedHandler;
            _moduleThemePropertyChangedHandlers.Remove(node.Id);
        }
    }

    private void NodePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is ThemeNodeObservableModel node)
        {
            UpdateDirty(node);
        }
    }

    private void NodeModulesCollectionChanged(ThemeNodeObservableModel node, object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (ThemeModuleObservableModel module in e.NewItems)
            {
                SubscribeToModuleChanges(node, module);
            }
        }
        if (e.OldItems != null)
        {
            foreach (ThemeModuleObservableModel module in e.OldItems)
            {
                UnsubscribeFromModuleChanges(node, module);
            }
        }
        UpdateDirty(node);
    }
    #endregion

    [RelayCommand(CanExecute = nameof(IsSelected))]
    private void NewThemeModule()
    {
        if (EditableTheme is null) return;
        EditableTheme.Modules.Add(new(ThemeModules.Empty));
    }

    [RelayCommand]
    private void DeleteThemeModule(ThemeModuleObservableModel module)
    {
        if (EditableTheme is null || module is null) return;
        EditableTheme.Modules.Remove(module);
    }

    [RelayCommand]
    private void DiscardChanges()
    {
        if (EditableTheme is null || Parent.SelectedTheme is null) return;
        EditableTheme.Assign(Parent.SelectedTheme);
        SubscribeToNodeChanges(EditableTheme);
        UpdateDirty(EditableTheme);
    }

    /// <summary>
    /// Logically part of the ThemeExplorer, as such it delegates the logic to it.
    /// </summary>
    [RelayCommand(CanExecute = nameof(IsSelectedEditable))]
    private void RenameTheme()
    {
        if (Parent.SelectedTheme is null) return;
        Parent.ThemeExplorer.RenameThemeCommand.Execute(Parent.SelectedTheme.Id);
    }

    /// <summary>
    /// Logically part of the ThemeExplorer, as such it delegates the logic to it.
    /// </summary>
    [RelayCommand(CanExecute = nameof(IsSelected))]
    private async Task SaveAsTheme()
    {
        if (Parent.SelectedTheme is null || EditableTheme is null) return;
        await Parent.ThemeExplorer.SaveAsThemeCustom(EditableTheme);
    }

    /// <summary>
    /// Logically part of the ThemeExplorer, as such it delegates the logic to it.
    /// </summary>
    [RelayCommand(CanExecute = nameof(IsSelectedEditable))]
    private void DeleteTheme()
    {
        if (Parent.SelectedTheme is null) return;
        Parent.ThemeExplorer.DeleteThemeCommand.Execute(Parent.SelectedTheme.Id);
    }

    [RelayCommand(CanExecute = nameof(IsSelectedEditable))]
    private void SaveTheme()
    {
        if (Parent.SelectedTheme is null || EditableTheme is null) return;
        Parent.SelectedTheme.Assign(EditableTheme);
        UpdateDirty(EditableTheme);
    }

    [RelayCommand(CanExecute = nameof(IsSelected))]
    private void UseTheme()
    {
        if (Parent.SelectedTheme is null || EditableTheme is null) return;
        Parent.ThemeExplorer.UseThemeCustom(EditableTheme);
    }

    private bool IsSelected()
    {
        return Parent.SelectedTheme is not null;
    }

    private bool IsSelectedEditable()
    {
        return Parent.SelectedTheme is not null && !Parent.SelectedTheme.Readonly;
    }
}
