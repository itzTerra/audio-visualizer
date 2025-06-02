using System;
using System.IO;
using System.Threading.Tasks;
using SoundFlow.Abstracts;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Components;
using SoundFlow.Enums;
using SoundFlow.Exceptions;
using SoundFlow.Providers;

namespace AudioVisualizer.Services.Audio;

public class AudioPlayerService : IDisposable
{
    private readonly AudioEngine _audioEngine;

    private SoundPlayer? _soundPlayer;
    public SoundPlayer? SoundPlayer
    {
        get => _soundPlayer;
        private set
        {
            IsReady = false;
            if (_soundPlayer != null)
            {
                _soundPlayer.Stop();
                Mixer.Master.RemoveComponent(_soundPlayer);
            }
            _soundPlayer = value;
            if (_soundPlayer != null)
            {
                _soundPlayer.IsLooping = _isLooping;
                Mixer.Master.AddComponent(_soundPlayer);
                _soundPlayer.PlaybackEnded += (s, e) =>
                {
                    if (!_isLooping)
                    {
                        OnPlaybackEnded(EventArgs.Empty);
                    }
                };
                IsReady = true;
            }
        }
    }

    public event EventHandler? AudioReady;
    public event EventHandler? PlaybackStarted;
    public event EventHandler? PlaybackPaused;
    public event EventHandler? PlaybackEnded;

    private bool _isReady = false;
    public bool IsReady
    {
        get => _isReady;
        private set
        {
            if (_isReady != value)
            {
                _isReady = value;
                if (value)
                {
                    OnAudioReady(EventArgs.Empty);
                }
            }
        }
    }

    public bool IsPlaying => SoundPlayer?.State == PlaybackState.Playing;

    public float Time
    {
        get
        {
            if (SoundPlayer == null) return 0;
            return SoundPlayer.Time;
        }
    }

    private bool _isLooping = false;
    public bool IsLooping
    {
        get => _isLooping;
        set
        {
            _isLooping = value;
            if (SoundPlayer != null)
            {
                SoundPlayer.IsLooping = value;
            }
        }
    }

    private bool _isMuted = false;
    public bool IsMuted
    {
        get => _isMuted;
        set
        {
            _isMuted = value;

            // Mute stops the sound processing, which pauses the playback
            // SoundPlayer.Mute = value;
            if (value)
            {
                Mixer.Master.Volume = 0f;
                Mixer.Master.Volume = 0f;
            }
            else
            {
                Mixer.Master.Volume = _volume;
                Mixer.Master.Volume = _volume;
            }
        }
    }

    private float _volume = 0.5f;
    public float Volume
    {
        get => _volume;
        set
        {
            _volume = value;
            if (!_isMuted)
            {
                // The volume is lerped with previous volume (idk why)
                Mixer.Master.Volume = value;
                Mixer.Master.Volume = value;
            }
        }
    }

    public AudioPlayerService(int sampleRate)
    {
        try
        {
            _audioEngine = AudioEngine.Instance;
        }
        catch (BackendException)
        {
            _audioEngine = new MiniAudioEngine(sampleRate, Capability.Playback);
        }

        Mixer.Master.Pan = 0.5f; // Fixes https://github.com/LSXPrime/SoundFlow/issues/31
        Volume = _volume;
    }

    /// <summary>
    /// Can throw
    /// </summary>
    public async Task Load(string filePath)
    {
        var stream = await Task.Run(() => File.OpenRead(filePath));
        SoundPlayer = new SoundPlayer(new StreamDataProvider(stream));
    }

    public float Duration
    {
        get
        {
            if (SoundPlayer == null) return 0;
            return SoundPlayer.Duration;
        }
    }

    public void Play()
    {
        if (SoundPlayer?.State == PlaybackState.Stopped && (Duration - SoundPlayer.Time < 1f))
        {
            // This should happen automatically on SoundPlayer.Play, but doesn't
            SoundPlayer?.Seek(0);
        }
        SoundPlayer?.Play();
        OnPlaybackStarted(EventArgs.Empty);
    }

    public void Pause()
    {
        SoundPlayer?.Pause();
        OnPlaybackPaused(EventArgs.Empty);
    }

    public void Seek(float time)
    {
        SoundPlayer?.Seek(time);
    }

    public bool ToggleMute()
    {
        IsMuted = !IsMuted;
        return IsMuted;
    }

    public void Step(float time)
    {
        try
        {
            SoundPlayer?.Seek(SoundPlayer.Time + time);
        }
        catch (ArgumentOutOfRangeException)
        {
            SoundPlayer!.Seek(0);
        }
    }

    public bool ToggleLooping()
    {
        IsLooping = !IsLooping;
        return IsLooping;
    }

    public void Stop()
    {
        SoundPlayer?.Stop();
    }

    public void Dispose()
    {
        SoundPlayer = null;
        _audioEngine.Dispose();
    }

    protected virtual void OnAudioReady(EventArgs e)
    {
        AudioReady?.Invoke(this, e);
    }

    protected virtual void OnPlaybackStarted(EventArgs e)
    {
        PlaybackStarted?.Invoke(this, e);
    }

    protected virtual void OnPlaybackPaused(EventArgs e)
    {
        PlaybackPaused?.Invoke(this, e);
    }

    protected virtual void OnPlaybackEnded(EventArgs e)
    {
        PlaybackEnded?.Invoke(this, e);
    }
}
