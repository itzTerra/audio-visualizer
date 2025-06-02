using System;
using System.Linq;
using AudioVisualizer.Messages;
using AudioVisualizer.Services;
using AudioVisualizer.Services.Audio;
using AudioVisualizer.Services.Visualizer;
using AudioVisualizer.Utils;
using Avalonia.Labs.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SkiaSharp;
using SoundFlow.Abstracts;

namespace AudioVisualizer.ViewModels;

public partial class MediaPlayerViewModel : ViewModelBase
{
    const int TimerInterval = 250;

    private readonly AudioPlayerService _audioPlayerService;
    private readonly VisualizerService _visualizer;
    private DispatcherTimer? _timer;

    // Event for views hosting the SKCanvasView to subscribe to
    public event EventHandler? RequestCanvasInvalidate;

    public MainViewViewModel Parent { get; }

    public MediaPlayerViewModel(MainViewViewModel parent)
    {
        _audioPlayerService = new AudioPlayerService(AppConfig.AudioSampleRate);

        _visualizer = new VisualizerService();
        _visualizer.VisualizationUpdated += OnCanvasInvalidationRequested;

        Parent = parent;

        Parent.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(Parent.CurrentTheme))
            {
                RegisterVisualizers();
            }
        };
        RegisterVisualizers();

        _audioPlayerService.AudioReady += (s, e) =>
        {
            _visualizer.ConnectTo(_audioPlayerService.SoundPlayer!);

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(TimerInterval)
            };
            _timer.Tick += (_, __) =>
            {
                OnPropertyChanged(nameof(CurrentTimeSeconds));
            };

            OnPropertyChanged(nameof(IsAudioReady));
            OnPropertyChanged(nameof(DurationSeconds));
            OnPropertyChanged(nameof(CurrentTimeSeconds));
            OnPropertyChanged(nameof(IsPlaying));
            TogglePlaybackCommand.NotifyCanExecuteChanged();
            StepBackwardCommand.NotifyCanExecuteChanged();
            StepForwardCommand.NotifyCanExecuteChanged();
            ToggleMuteCommand.NotifyCanExecuteChanged();
        };
        _audioPlayerService.PlaybackStarted += (s, e) =>
        {
            _timer?.Start();
            _visualizer.Start();
        };
        _audioPlayerService.PlaybackEnded += (s, e) =>
        {
            OnPropertyChanged(nameof(IsPlaying));
            _timer?.Stop();
            _visualizer.Stop();
        };

        WeakReferenceMessenger.Default.Register<AudioImportMessage>(this, async (r, m) =>
        {
            try
            {
                await _audioPlayerService.Load(m.Value);
            }
            catch (Exception ex)
            {
                Notifier.Error($"Failed to load audio file: {ex.Message}");
            }
        });
    }

    public void RegisterVisualizers()
    {
        if (Parent.CurrentTheme is null)
        {
            return;
        }

        var wasRunning = _visualizer.IsRunning;
        _visualizer.Dispose();
        _visualizer.RegisterVisualizers((Parent.CurrentTheme?.Modules ?? []).Where(m => m.IsVisible).Select(m => m.CreateVisualizer()!).Where(m => m is not null));
        if (wasRunning)
        {
            _visualizer.Start();
        }
    }

    protected virtual void OnCanvasInvalidationRequested(object? sender, EventArgs e)
    {
        RequestCanvasInvalidate?.Invoke(sender, e);
    }

    // Bound to labs:SKCanvasView, calls all Painter subscribers to the Paint event, passing the SKCanvas
    public void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        SKSurface surface = e.Surface;
        SKCanvas canvas = surface.Canvas;

        // Clear the canvas
        canvas.Clear();

        // Create a custom IVisualizationContext that wraps the Canvas
        var context = new SkiaVisualizationContext(canvas);

        Dispatcher.UIThread.Invoke(() =>
        {
            _visualizer.Render(context);
        });
    }

    // ############################## Audio Player Controls ##############################
    private AudioProcessCallback? _audioProcessCallback;

    public int CurrentTimeSeconds
    {
        get => (int)_audioPlayerService.Time;
        set
        {
            if (value == (int)_audioPlayerService.Time) return;

            // To visualize when seeking while paused
            if (!_visualizer.IsRunning)
            {
                _visualizer.Start();
                _audioPlayerService.Seek(value);

                if (_audioProcessCallback == null)
                {
                    _audioProcessCallback = new((data, c) =>
                    {
                        _audioPlayerService.Pause();
                        AudioEngine.OnAudioProcessed -= _audioProcessCallback;
                        _audioProcessCallback = null;
                    });
                    AudioEngine.OnAudioProcessed += _audioProcessCallback;
                }

                _audioPlayerService.Play();
                _visualizer.Stop();
            }
            else
            {
                _audioPlayerService.Seek(value);
            }
            OnPropertyChanged(nameof(CurrentTimeSeconds));
        }
    }

    public int DurationSeconds => (int)_audioPlayerService.Duration;

    public int Volume
    {
        get => (int)(_audioPlayerService.Volume * 100);
        set
        {
            _audioPlayerService.Volume = value / 100f;
            OnPropertyChanged(nameof(Volume));
        }
    }

    public bool IsLooping
    {
        get => _audioPlayerService.IsLooping;
        set
        {
            _audioPlayerService.IsLooping = value;
            OnPropertyChanged(nameof(IsLooping));
        }
    }

    public bool IsAudioReady => _audioPlayerService.IsReady;
    public bool IsMuted => _audioPlayerService.IsMuted;
    public bool IsPlaying => _audioPlayerService.IsPlaying;

    [RelayCommand(CanExecute = nameof(IsAudioReady))]
    public void TogglePlayback()
    {
        if (_audioPlayerService.IsPlaying)
        {
            _audioPlayerService.Pause();
            _timer?.Stop();
            _visualizer.Stop();
        }
        else
        {
            _audioPlayerService.Play();
        }
        OnPropertyChanged(nameof(IsPlaying));
    }

    [RelayCommand]
    public void ToggleMute()
    {
        _audioPlayerService.ToggleMute();
        OnPropertyChanged(nameof(IsMuted));
    }

    [RelayCommand(CanExecute = nameof(IsAudioReady))]
    public void StepBackward()
    {
        _audioPlayerService.Step(-AppConfig.StepButtonTimeSeconds);
    }

    [RelayCommand(CanExecute = nameof(IsAudioReady))]
    public void StepForward()
    {
        _audioPlayerService.Step(AppConfig.StepButtonTimeSeconds);
    }
}
