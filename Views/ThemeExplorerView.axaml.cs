using System;
using System.Linq;
using AudioVisualizer.Behaviors;
using AudioVisualizer.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactions.Custom;
using Avalonia.Xaml.Interactions.DragAndDrop;
using Avalonia.Xaml.Interactivity;

namespace AudioVisualizer.Views;

public partial class ThemeExplorerView : UserControl
{
    public ThemeExplorerView()
    {
        InitializeComponent();
        this.AttachedToVisualTree += OnAttachedToVisualTree;
    }

    private void ThemeNode_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        if (DataContext is ThemeExplorerViewModel vm)
        {
            vm.UseSelectedThemeCommand.Execute(null);
        }
        e.Handled = true;
    }

    // Behaviours defined in ThemeExplorerView styles stop working when navigating to a different view (e.g. settings)
    #region LostBehavioursOnViewSwitchFix

    private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        var treeView = this.FindControl<TreeView>("ThemeTreeView");
        if (treeView is null) return;

        var behaviors = Interaction.GetBehaviors(treeView);

        if (behaviors.Count == 0)
        {
            behaviors.Add(new ContextDropBehavior
            {
                Handler = new NodesTreeViewDropHandler()
            });
        }
        AttachBehaviorsRecursive(treeView);
    }

    private void AttachBehaviorsRecursive(ItemsControl control)
    {
        if (control.ItemCount == 0 || control.ItemContainerGenerator == null)
            return;

        for (int i = 0; i < control.ItemCount; i++)
        {
            var container = control.ContainerFromIndex(i);
            if (container is TreeViewItem item)
            {
                AttachBehaviors(item);
                AttachBehaviorsRecursive(item);
            }
        }
    }

    private void AttachBehaviors(TreeViewItem item)
    {
        item.SetValue(DragDrop.AllowDropProperty, true);

        var behaviors = Interaction.GetBehaviors(item);
        if (behaviors.Count > 0)
        {
            return;
        }

        behaviors.Add(new ContextDragBehavior());

        behaviors.Add(new RoutedEventTriggerBehavior
        {
            RoutedEvent = DragDrop.DragEnterEvent,
            RoutingStrategies = RoutingStrategies.Bubble,
            Actions =
            {
                new AddClassAction
                {
                    ClassName = "dragging",
                    RemoveIfExists = true
                }
            }
        });

        behaviors.Add(new RoutedEventTriggerBehavior
        {
            RoutedEvent = DragDrop.DragLeaveEvent,
            RoutingStrategies = RoutingStrategies.Bubble,
            Actions =
            {
                new RemoveClassAction
                {
                    ClassName = "dragging"
                }
            }
        });

        behaviors.Add(new RoutedEventTriggerBehavior
        {
            RoutedEvent = DragDrop.DropEvent,
            RoutingStrategies = RoutingStrategies.Bubble,
            Actions =
            {
                new RemoveClassAction
                {
                    ClassName = "dragging"
                }
            }
        });
    }
    #endregion
}
