using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AudioVisualizer.ViewModels;
using AudioVisualizer.ViewModels.Observables;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactions.DragAndDrop;

namespace AudioVisualizer.Behaviors;

// Inspired by:
// https://github.com/AvaloniaUI/Avalonia.Xaml.Behaviors/tree/master/samples/DragAndDropSample
// https://github.com/AvaloniaUI/Avalonia/discussions/10877
public class NodesTreeViewDropHandler : DropHandlerBase
{
    private enum DragDirection
    {
        Up,
        Down
    }

    private const string DraggingUpClassName = "dragging-up";
    private const string DraggingDownClassName = "dragging-down";
    private const int EdgeThreshold = 4;

    private struct DndData
    {
        public DndData() { }
        public TreeView DestTreeView = null!;
        public ThemeNodeObservableModel SrcNode = null!;
        public ThemeNodeObservableModel? DestNode = null!;
        public ThemeNodeObservableModel? SrcParent;
        public ThemeNodeObservableModel? DestParent;
        public IList<ThemeNodeObservableModel> SrcNodes = null!;
        public IList<ThemeNodeObservableModel> DestNodes = null!;
        public int SrcIndex = -1;
        public int DestIndex = -1;
        public DragDirection Direction;
        public bool IsNearEdge;

        public override string ToString()
        {
            return $"SrcNode: {SrcNode}, SrcParent: {SrcParent}, DestParent: {DestParent}, SrcNodes: {SrcNodes}, DestNodes: {DestNodes}, SrcIndex: {SrcIndex}, DestIndex: {DestIndex}, Direction: {Direction}";
        }
    }

    private DndData _ctx = new();

    private bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext)
    {
        if (e.Source is not Control
            || sender is not TreeView treeView
            || sourceContext is not ThemeNodeObservableModel srcNode
            || treeView.GetVisualAt(e.GetPosition(treeView)) is not Control targetControl
            || (targetControl.DataContext is not ThemeNodeObservableModel && targetControl.DataContext is not ThemeExplorerViewModel))
        {
            return false;
        }
        ThemeNodeObservableModel? destNode = targetControl.DataContext as ThemeNodeObservableModel;
        if (srcNode == destNode?.Parent)
        {
            return false;
        }

        // Fix: handler's behaviour loses the targetContext on view switch
        if (targetContext is not ThemeExplorerViewModel vm)
        {
            targetContext = targetControl.GetVisualAncestors()
                                         .OfType<UserControl>()
                                         .FirstOrDefault()?.DataContext;
            if (targetContext is not ThemeExplorerViewModel)
            {
                return false;
            }
            vm = (ThemeExplorerViewModel)targetContext;
        }

        _ctx.DestTreeView = treeView;
        _ctx.SrcNode = srcNode;
        _ctx.SrcParent = srcNode.Parent;
        _ctx.SrcNodes = _ctx.SrcParent is not null ? _ctx.SrcParent.Nodes : vm.Nodes;
        _ctx.SrcIndex = _ctx.SrcNodes.IndexOf(srcNode);

        _ctx.DestNode = destNode;
        _ctx.DestParent = destNode?.Parent;
        _ctx.DestNodes = _ctx.DestParent is not null ? _ctx.DestParent.Nodes : vm.Nodes;
        _ctx.DestIndex = destNode is not null ? _ctx.DestNodes.IndexOf(destNode) : Math.Max(_ctx.DestNodes.Count - 1, 0);

        var pos = e.GetPosition(targetControl);
        _ctx.Direction = targetControl.DesiredSize.Height / 2 > pos.Y ? DragDirection.Up : DragDirection.Down;
        _ctx.IsNearEdge = pos.Y < EdgeThreshold || pos.Y > targetControl.DesiredSize.Height - EdgeThreshold;
        return _ctx.SrcIndex >= 0 && _ctx.DestIndex >= 0;
    }

    public override bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        return Validate(sender, e, sourceContext, targetContext);
    }

    public override bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        if (!Validate(sender, e, sourceContext, targetContext))
        {
            return false;
        }

        // Move to open space at the bottom of the treeview
        if (_ctx.DestNode is null)
        {
            _ctx.SrcNode.Parent = null;
            if (_ctx.SrcNodes == _ctx.DestNodes)
            {
                // The node gets removed first before the insert, so -1 is needed
                MoveItem(_ctx.SrcNodes, _ctx.SrcIndex, Math.Max(_ctx.DestNodes.Count - 1, 0));
            }
            else
            {
                MoveItem(_ctx.SrcNodes, _ctx.DestNodes, _ctx.SrcIndex, _ctx.DestNodes.Count);
            }
            return true;
        }

        if (_ctx.SrcIndex > _ctx.DestIndex && _ctx.Direction == DragDirection.Down)
            _ctx.DestIndex++;
        else if (_ctx.SrcIndex < _ctx.DestIndex && _ctx.Direction == DragDirection.Up)
            _ctx.DestIndex--;

        if (_ctx.DestNode.IsCategory && !_ctx.IsNearEdge && _ctx.SrcNode != _ctx.DestNode)
        {
            if (_ctx.SrcNodes == _ctx.DestNode.Nodes)
            {
                return true;
            }
            _ctx.SrcNode.Parent = _ctx.DestNode;
            MoveItem(_ctx.SrcNodes, _ctx.DestNode.Nodes, _ctx.SrcIndex, _ctx.DestNode.Nodes.Count);
        }
        else if (_ctx.SrcNodes == _ctx.DestNodes)
        {
            MoveItem(_ctx.SrcNodes, _ctx.SrcIndex, _ctx.DestIndex);
        }
        else
        {
            _ctx.SrcNode.Parent = _ctx.DestParent;
            MoveItem(_ctx.SrcNodes, _ctx.DestNodes, _ctx.SrcIndex, _ctx.DestIndex);
        }
        return true;
    }

    public override void Enter(object? sender, DragEventArgs e, object? sourceContext,
                               object? targetContext)
    {
        if (!Validate(sender, e, sourceContext, targetContext))
        {
            e.DragEffects = DragDropEffects.None;
            e.Handled = true;
            return;
        }

        string className = _ctx.Direction switch
        {
            DragDirection.Down => DraggingDownClassName,
            DragDirection.Up => DraggingUpClassName,
            _ => throw new UnreachableException($"Invalid drag direction: {_ctx.Direction}")
        };
        _ctx.DestTreeView.Classes.Add(className);

        e.DragEffects |= DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link;
        e.Handled = true;
    }

    public override void Over(object? sender, DragEventArgs e, object? sourceContext,
                              object? targetContext)
    {
        if (!Validate(sender, e, sourceContext, targetContext))
        {
            e.DragEffects = DragDropEffects.None;
            e.Handled = true;
            return;
        }

        e.DragEffects |= DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link;
        e.Handled = true;

        (string toAdd, string toRemove) classUpdate = _ctx.Direction switch
        {
            DragDirection.Down => (DraggingDownClassName, DraggingUpClassName),
            DragDirection.Up => (DraggingUpClassName, DraggingDownClassName),
            _ => throw new UnreachableException($"Invalid drag direction: {_ctx.Direction}")
        };
        if (_ctx.DestTreeView.Classes.Contains(classUpdate.toAdd))
            return;

        _ctx.DestTreeView.Classes.Remove(classUpdate.toRemove);
        _ctx.DestTreeView.Classes.Add(classUpdate.toAdd);
    }

    public override void Leave(object? sender, RoutedEventArgs e)
    {
        base.Leave(sender, e);
        RemoveDraggingClass(sender as TreeView);
    }

    public override void Drop(object? sender, DragEventArgs e, object? sourceContext,
                              object? targetContext)
    {
        RemoveDraggingClass(sender as TreeView);
        base.Drop(sender, e, sourceContext, targetContext);
    }

    private static void RemoveDraggingClass(TreeView? control)
    {
        if (control is not null && !control.Classes.Remove(DraggingUpClassName))
            control.Classes.Remove(DraggingDownClassName);
    }
}
