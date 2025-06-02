using System;
using AudioVisualizer.Messages;
using AudioVisualizer.ViewModels;
using Avalonia.Controls;
using Avalonia.Labs.Controls;
using CommunityToolkit.Mvvm.Messaging;

namespace AudioVisualizer.Views;

public partial class MediaPlayerView : UserControl
{
    private MediaPlayerViewModel? _viewModel;

    public MediaPlayerView()
    {
        InitializeComponent();

        WeakReferenceMessenger.Default.Register<ExportImageMessage>(this, async (r, m) =>
        {
            try
            {
                await VisualizationCanvas.SaveToFile(m.Value, 100);
                WeakReferenceMessenger.Default.Send(new ExportImageResponseMessage(null));
            }
            catch (Exception ex)
            {
                WeakReferenceMessenger.Default.Send(new ExportImageResponseMessage(ex.Message));
            }
        });
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is MediaPlayerViewModel vm)
        {
            _viewModel = vm;
            vm.RequestCanvasInvalidate += (s, e) => VisualizationCanvas.InvalidateSurface();
        }
    }

    public void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        _viewModel?.OnPaintSurface(sender, e);
    }
}
