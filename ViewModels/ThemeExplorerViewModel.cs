using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AudioVisualizer.Controls.Dialogs;
using AudioVisualizer.Extensions;
using AudioVisualizer.IO;
using AudioVisualizer.Messages;
using AudioVisualizer.Models;
using AudioVisualizer.ViewModels.Dialogs;
using AudioVisualizer.ViewModels.Observables;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Ursa.Controls;

namespace AudioVisualizer.ViewModels;

public partial class ThemeExplorerViewModel : ViewModelBase
{
    public MainViewViewModel Parent { get; }
    private readonly IStorageService _storageService = new FileStorageService();

    [ObservableProperty]
    private ObservableCollection<ThemeNodeObservableModel> _nodes;

    public ThemeExplorerViewModel(MainViewViewModel parent)
    {
        Parent = parent;

        _nodes = new ObservableCollection<ThemeNodeObservableModel>(_storageService.LoadThemes().Select(x => new ThemeNodeObservableModel(x)));

        Nodes.CollectionChanged += async (s, e) => await SaveNodes();
        foreach (var node in _nodes)
        {
            SubscribeToNodeChanges(node);
        }
    }

    private async Task SaveNodes()
    {
        await _storageService.SaveThemes(Nodes.Select(x => x.ToModel()).ToArray());
    }

    private void SubscribeToNodeChanges(ThemeNodeObservableModel node)
    {
        node.PropertyChanged += async (s, e) => await SaveNodes();
        node.Nodes.CollectionChanged += (s, e) =>
        {
            _ = SaveNodes();
            if (e.NewItems != null)
            {
                foreach (ThemeNodeObservableModel newNode in e.NewItems)
                {
                    SubscribeToNodeChanges(newNode);
                }
            }
        };
        foreach (var child in node.Nodes)
        {
            SubscribeToNodeChanges(child);
        }
    }

    [RelayCommand]
    private void AddTheme()
    {
        var newTheme = new ThemeNodeObservableModel(new ThemeNodeModel("New Theme", false));
        Nodes.Add(newTheme);
        SubscribeToNodeChanges(newTheme);
    }

    [RelayCommand]
    private void AddCategory()
    {
        var newCategory = new ThemeNodeObservableModel(new ThemeNodeModel("New Category", true));
        Nodes.Add(newCategory);
        SubscribeToNodeChanges(newCategory);
    }

    [RelayCommand]
    private void UseSelectedTheme()
    {
        if (Parent.SelectedTheme is null) return;
        Parent.CurrentTheme = Parent.SelectedTheme.ToModel();
    }

    public void UseThemeCustom(ThemeNodeObservableModel themeNode)
    {
        if (themeNode is null) return;
        Parent.CurrentTheme = themeNode.ToModel().Clone();
    }

    [RelayCommand]
    private void UseTheme(string nodeId)
    {
        if (Nodes.SelectManyRecursive(n => n.Nodes).FirstOrDefault(x => x.Id == nodeId) is { } node && !node.IsCategory)
        {
            UseThemeCustom(node);
        }
    }

    [RelayCommand]
    private void AddThemeToNode(string nodeId)
    {
        if (Nodes.SelectManyRecursive(n => n.Nodes).FirstOrDefault(x => x.Id == nodeId) is { } node)
        {
            var newTheme = new ThemeNodeObservableModel(new ThemeNodeModel("New Theme", false));
            node.Nodes.Add(newTheme);
            newTheme.Parent = node;
            SubscribeToNodeChanges(newTheme);
        }
    }

    [RelayCommand]
    private void AddCategoryToNode(string nodeId)
    {
        if (Nodes.SelectManyRecursive(n => n.Nodes).FirstOrDefault(x => x.Id == nodeId) is { } node)
        {
            var newCategory = new ThemeNodeObservableModel(new ThemeNodeModel("New Category", true));
            node.Nodes.Add(newCategory);
            newCategory.Parent = node;
            SubscribeToNodeChanges(newCategory);
        }
    }

    [RelayCommand]
    public async Task RenameTheme(string nodeId)
    {
        if (Nodes.SelectManyRecursive(n => n.Nodes).FirstOrDefault(x => x.Id == nodeId) is { } node)
        {
            WeakReferenceMessenger.Default.Register<TextBoxDialogResultMessage>(this, (r, m) =>
            {
                node.Name = m.Value;
                if (Parent.CurrentTheme?.Id == node.Id)
                {
                    Parent.CurrentTheme = node.ToModel();
                }
            });
            var result = await ShowTextBoxDialog("Rename Theme", "New name:", node.Name);
            WeakReferenceMessenger.Default.Unregister<TextBoxDialogResultMessage>(this);
        }
    }

    public async Task SaveAsThemeCustom(ThemeNodeObservableModel themeNode)
    {
        WeakReferenceMessenger.Default.Register<TextBoxDialogResultMessage>(this, (r, m) =>
        {
            var clonedTheme = themeNode.ToModel().Clone();
            clonedTheme.Name = m.Value;
            clonedTheme.Readonly = false;
            var newTheme = new ThemeNodeObservableModel(clonedTheme);
            if (themeNode.Parent is not null)
            {
                themeNode.Parent.Nodes.Add(newTheme);
                newTheme.Parent = themeNode.Parent;
            }
            else
            {
                Nodes.Add(newTheme);
            }
            SubscribeToNodeChanges(newTheme);
        });
        var result = await ShowTextBoxDialog("Save as Theme", "New Theme name:");
        WeakReferenceMessenger.Default.Unregister<TextBoxDialogResultMessage>(this);
    }

    [RelayCommand]
    private async Task SaveAsTheme(string nodeId)
    {
        if (Nodes.SelectManyRecursive(n => n.Nodes).FirstOrDefault(x => x.Id == nodeId) is { } node)
        {
            await SaveAsThemeCustom(node);
        }
    }

    [RelayCommand]
    private async Task DeleteTheme(string nodeId)
    {
        if (Nodes.SelectManyRecursive(n => n.Nodes).FirstOrDefault(x => x.Id == nodeId) is { } node)
        {
            var containingNodes = node.Parent?.Nodes ?? Nodes;
            var result = await ShowConfirmDialog("Delete Theme", $"Are you sure you want to delete the theme '{node.Name}'?");
            if (result == DialogResult.Yes)
            {
                if (Parent.CurrentTheme?.Id == node.Id)
                {
                    Parent.CurrentTheme = null;
                }
                if (Parent.SelectedTheme?.Id == node.Id)
                {
                    Parent.SelectedTheme = null;
                }
                if (Parent.DirtyThemes.ContainsKey(node.Id))
                {
                    Parent.DirtyThemes.Remove(node.Id);
                }
                containingNodes.Remove(node);
            }
        }
    }

    private async Task<DialogResult> ShowTextBoxDialog(string title, string? label = null, string? prefill = null)
    {
        return await OverlayDialog.ShowCustomModal<TextBoxDialog, TextBoxDialogViewModel, DialogResult>(new TextBoxDialogViewModel(title, label, prefill), null, options: new OverlayDialogOptions()
        {
            FullScreen = false,
            HorizontalAnchor = HorizontalPosition.Center,
            VerticalAnchor = VerticalPosition.Center,
            Mode = DialogMode.Question,
            Buttons = DialogButton.OKCancel,
            Title = title,
            CanDragMove = true,
            IsCloseButtonVisible = true,
            CanResize = false,
        });
    }

    private async Task<DialogResult> ShowConfirmDialog(string title, string message)
    {
        return await OverlayDialog.ShowModal<ConfirmDialog, ConfirmDialogViewModel>(new ConfirmDialogViewModel(message), null, options: new OverlayDialogOptions()
        {
            FullScreen = false,
            HorizontalAnchor = HorizontalPosition.Center,
            VerticalAnchor = VerticalPosition.Center,
            Mode = DialogMode.Error,
            Buttons = DialogButton.YesNo,
            Title = title,
            CanDragMove = true,
            IsCloseButtonVisible = true,
            CanResize = false,
        });
    }
}
