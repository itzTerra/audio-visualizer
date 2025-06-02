using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Labs.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Skia;
using Avalonia.Threading;
using SkiaSharp;

namespace AudioVisualizer.Controls;

// https://github.com/AvaloniaUI/Avalonia.Labs/blob/main/src/Avalonia.Labs.Controls/SKCanvasView/SKCanvasView.cs
public partial class CustomSKCanvasView : Decorator
{
    public static readonly StyledProperty<int> CanvasPixelWidthProperty =
        AvaloniaProperty.Register<CustomSKCanvasView, int>(nameof(CanvasPixelWidth), 1920);

    public static readonly StyledProperty<int> CanvasPixelHeightProperty =
        AvaloniaProperty.Register<CustomSKCanvasView, int>(nameof(CanvasPixelHeight), 1080);

    public int CanvasPixelWidth
    {
        get => GetValue(CanvasPixelWidthProperty);
        set => SetValue(CanvasPixelWidthProperty, value);
    }

    public int CanvasPixelHeight
    {
        get => GetValue(CanvasPixelHeightProperty);
        set => SetValue(CanvasPixelHeightProperty, value);
    }

    /// <summary>
    /// Event to externally paint the Skia surface (using the <see cref="SKCanvas"/>).
    /// </summary>
    public event EventHandler<SKPaintSurfaceEventArgs>? PaintSurface;

    private static readonly Vector Dpi = new Vector(96, 96);

    private WriteableBitmap? _writeableBitmap = default;

    private bool _IgnorePixelScaling;

    private int _pixelWidth;
    private int _pixelHeight;
    private double _scale = 1;

    private readonly static StyledProperty<IBrush?> BackgroundProperty =
        Border.BackgroundProperty.AddOwner<SKCanvasView>();

    static CustomSKCanvasView()
    {
        AffectsRender<CustomSKCanvasView>(BackgroundProperty, CanvasPixelWidthProperty, CanvasPixelHeightProperty);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SKCanvasView"/> class.
    /// </summary>
    public CustomSKCanvasView()
    {
    }

    /// <summary>
    /// Gets the current pixel size of the canvas.
    /// Any scaling factor is already applied.
    /// </summary>
    public Size CanvasSize { get; private set; }

    /// <summary>
    /// Gets the current render scaling applied to the control.
    /// </summary>

    public readonly static DirectProperty<SKCanvasView, double> ScaleProperty =
        AvaloniaProperty.RegisterDirect<SKCanvasView, double>(nameof(Scale),
            v => v.Scale);

    public double Scale { get => _scale; protected internal set => SetAndRaise(ScaleProperty, ref _scale, value); }
    /// <summary>
    /// Gets or sets a value indicating whether the canvas's resolution and scale
    /// will be automatically adjusted to match physical device pixels.
    /// </summary>
    public bool IgnorePixelScaling
    {
        get => this._IgnorePixelScaling;
        set
        {
            this._IgnorePixelScaling = value;
            this.InvalidateSurface();
        }
    }

    /// <summary>
    /// Invalidates the canvas causing the surface to be repainted.
    /// This will fire the <see cref="PaintSurface"/> event.
    /// </summary>
    public void InvalidateSurface()
    {
        Dispatcher.UIThread.Post(RepaintSurface);
    }

    /// <summary>
    /// Repaints the Skia surface and canvas.
    /// </summary>
    private void RepaintSurface()
    {
        if (!this.IsVisible)
        {
            return;
        }

        int pixelWidth = CanvasPixelWidth;
        int pixelHeight = CanvasPixelHeight;

        if (pixelWidth == 0 || pixelHeight == 0)
        {
            this.SetCurrentValue(BackgroundProperty, null);
            return;
        }

        var bitmap = _writeableBitmap ??= new WriteableBitmap(
            new PixelSize(pixelWidth, pixelHeight),
            Dpi,
            PixelFormat.Bgra8888,
            AlphaFormat.Premul);
        var scale = this.Scale;

        using (var framebuffer = bitmap.Lock())
        {
            var info = new SKImageInfo(
                framebuffer.Size.Width,
                framebuffer.Size.Height,
                framebuffer.Format.ToSkColorType(),
                SKAlphaType.Premul);

            var properties = new SKSurfaceProperties(SKPixelGeometry.RgbHorizontal);

            using (var surface = SKSurface.Create(info, framebuffer.Address, framebuffer.RowBytes, properties))
            {
                if (!this.IgnorePixelScaling)
                {
                    surface.Canvas.Scale(Convert.ToSingle(scale));
                }

                this.OnPaintSurface(new SKPaintSurfaceEventArgs(surface, info, info));
            }

            properties.Dispose();
        }

        this.SetCurrentValue(BackgroundProperty, new ImageBrush(bitmap)
        {
            AlignmentX = AlignmentX.Left,
            AlignmentY = AlignmentY.Top,
            Stretch = Stretch.Fill
        }.ToImmutable());

        return;
    }

    [System.ComponentModel.Browsable(false)]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed override void Render(DrawingContext context)
    {
        base.Render(context);
        if (GetValue(BackgroundProperty) is IBrush background)
        {
            context.FillRectangle(background, Bounds);
        }
    }

    /// <inheritdoc/>
    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);

        InvalidateSurface();
        return;
    }

    /// <summary>
    /// Called when the canvas should repaint its surface.
    /// </summary>
    /// <param name="e">The event args.</param>
    protected virtual void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        this.PaintSurface?.Invoke(this, e);
    }

    /// <inheritdoc/>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsVisibleProperty)
        {
            this.InvalidateSurface();
        }
        if (change.Property == BoundsProperty || change.Property == CanvasPixelWidthProperty || change.Property == CanvasPixelHeightProperty)
        {
            // Display scaling is important to consider here:
            // The bitmap itself should be sized to match physical device pixels.
            // This ensures it is never pixelated and renders properly to the display.
            // However, in several cases physical pixels do not match the logical pixels.
            // We also don't want to have to consider scaling in external code when calculating graphics.
            // To make this easiest, the layout scaling factor is calculated and then used
            // to find the size of the bitmap. This ensures it will match device pixels.
            // Then the canvas undoes this by setting a scale factor itself.
            // This means external code can use logical pixel size and the canvas will transform as needed.
            // Then the underlying bitmap is still at physical device pixel resolution.

            if (this.IgnorePixelScaling)
            {
                this.Scale = 1;
            }
            else
            {
                this.Scale = LayoutHelper.GetLayoutScale(this);
            }
            var scale = this.Scale;
            var bounds = change.GetNewValue<Rect>();
            _pixelWidth = Convert.ToInt32(bounds.Width * scale);
            _pixelHeight = Convert.ToInt32(bounds.Height * scale);
            this.CanvasSize = new Size(_pixelWidth, _pixelHeight);
            _writeableBitmap?.Dispose();
            _writeableBitmap = null;
            this.InvalidateSurface();
        }
        return;
    }

    public async Task SaveToFile(string filePath, int quality)
    {
        if (_writeableBitmap is null)
        {
            return;
        }
        await Task.Run(() => _writeableBitmap.Save(filePath, quality));
    }
}
