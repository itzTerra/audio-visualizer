using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AudioVisualizer.Models;
using AudioVisualizer.Models.ThemeModules;

namespace AudioVisualizer.ViewModels.Observables;

public class ThemeNodeObservableModel : ObservableModelBase
{
    public string Id { get; }

    private string _name = "";
    [Required]
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value, true);
    }

    private bool _readonly;
    public bool Readonly { get => _readonly; set => SetProperty(ref _readonly, value); }

    private ObservableCollection<ThemeModuleObservableModel> _modules = new();
    public ObservableCollection<ThemeModuleObservableModel> Modules
    {
        get => _modules;
        set => SetProperty(ref _modules, value);
    }

    private bool _isCategory;
    public bool IsCategory { get => _isCategory; set => SetProperty(ref _isCategory, value); }

    private ObservableCollection<ThemeNodeObservableModel> _nodes = new();
    public ObservableCollection<ThemeNodeObservableModel> Nodes
    {
        get => _nodes;
        set => SetProperty(ref _nodes, value);
    }

    private ThemeNodeObservableModel? _parent;
    public ThemeNodeObservableModel? Parent
    {
        get => _parent;
        set => SetProperty(ref _parent, value);
    }

    public ThemeNodeObservableModel()
    {
        Id = Guid.NewGuid().ToString();
        Name = "New Theme";
        Readonly = false;
        Modules = new ObservableCollection<ThemeModuleObservableModel>();
        IsCategory = false;
        Nodes = new ObservableCollection<ThemeNodeObservableModel>();
    }

    public ThemeNodeObservableModel(ThemeNodeModel model)
    {
        Id = model.Id;
        Name = model.Name;
        Readonly = model.Readonly;
        Modules = new ObservableCollection<ThemeModuleObservableModel>(model.Modules.Select(x => new ThemeModuleObservableModel(x)));
        IsCategory = model.IsCategory;
        Nodes = new ObservableCollection<ThemeNodeObservableModel>();
        foreach (var node in model.Nodes)
        {
            var nodeModel = new ThemeNodeObservableModel(node);
            nodeModel.Parent = this;
            Nodes.Add(nodeModel);
        }
    }

    public ThemeNodeModel ToModel()
    {
        return new ThemeNodeModel(Name, IsCategory, Readonly)
        {
            Id = Id,
            Modules = new List<ThemeModuleBase>(Modules.Select(m => m.ThemeModule)),
            Nodes = Nodes.Select(x => x.ToModel()).ToList()
        };
    }

    public override string ToString() => Name;

    public override bool Equals(object? obj)
    {
        if (obj is ThemeNodeObservableModel other)
        {
            return Id == other.Id &&
                   Name == other.Name &&
                   Readonly == other.Readonly &&
                   IsCategory == other.IsCategory &&
                   Modules.SequenceEqual(other.Modules);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name, Readonly, IsCategory, Modules);
    }

    public void Assign(ThemeNodeObservableModel other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        Name = other.Name;
        Readonly = other.Readonly;
        IsCategory = other.IsCategory;
        Modules = new ObservableCollection<ThemeModuleObservableModel>(other.Modules.Select(m => new ThemeModuleObservableModel(m.ThemeModule)));
    }
}
