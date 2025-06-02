using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Timers;
using AudioVisualizer.IO;
using AudioVisualizer.Models;
using AudioVisualizer.Models.Validators;
using AudioVisualizer.Utils;

namespace AudioVisualizer.ViewModels.Observables;

public class SettingsObservableModel : ObservableModelBase
{
    private Type _appThemeVariantEnum = typeof(AppThemeVariant);
    public Type AppThemeVariantEnum
    {
        get => _appThemeVariantEnum;
        set => SetProperty(ref _appThemeVariantEnum, value, true);
    }
    private Type _langEnum = typeof(Language);
    public Type LangEnum
    {
        get => _langEnum;
        set => SetProperty(ref _langEnum, value, true);
    }

    private string _defaultExportDirectory;
    [ValidDirPath(AllowEmptyPath = true)]
    public string DefaultExportDirectory
    {
        get => _defaultExportDirectory;
        set => SetProperty(ref _defaultExportDirectory, value, true);
    }

    private int _resolutionWidth;
    [Range(0, 8192)]
    public int ResolutionWidth
    {
        get => _resolutionWidth;
        set => SetProperty(ref _resolutionWidth, value, true);
    }

    private int _resolutionHeight;
    [Range(0, 8192)]
    public int ResolutionHeight
    {
        get => _resolutionHeight;
        set => SetProperty(ref _resolutionHeight, value, true);
    }

    private AppThemeVariant _theme;
    public AppThemeVariant Theme
    {
        get => _theme;
        set => SetProperty(ref _theme, value, true);
    }

    private Language _language;
    public Language Language
    {
        get => _language;
        set => SetProperty(ref _language, value, true);
    }

    private double _mediaPlayerColWidth;
    public double MediaPlayerColWidth
    {
        get => _mediaPlayerColWidth;
        set
        {
            if (SetProperty(ref _mediaPlayerColWidth, value, true))
            {
                // Restart the debounce timer on property change
                _debounceTimer.Stop();
                _debounceTimer.Start();
            }
        }
    }

    private double _themeExplorerRowHeight;
    public double ThemeExplorerRowHeight
    {
        get => _themeExplorerRowHeight;
        set
        {
            if (SetProperty(ref _themeExplorerRowHeight, value, true))
            {
                // Restart the debounce timer on property change
                _debounceTimer.Stop();
                _debounceTimer.Start();
            }
        }
    }

    private Timer _debounceTimer;
    private const int DebounceInterval = 2000;
    private static readonly IStorageService _storageService = new FileStorageService();

    public SettingsObservableModel(SettingsModel? model = null)
    {
        model ??= new SettingsModel();

        _defaultExportDirectory = model.DefaultExportDirectory;
        _resolutionWidth = model.ResolutionWidth;
        _resolutionHeight = model.ResolutionHeight;
        _theme = model.Theme;
        _language = model.Language;
        _mediaPlayerColWidth = model.MediaPlayerColWidth;
        _themeExplorerRowHeight = model.ThemeExplorerRowHeight;

        _debounceTimer = new Timer(DebounceInterval);
        _debounceTimer.AutoReset = false;
        _debounceTimer.Elapsed += AutoSave;
    }

    public static SettingsObservableModel FromStorage()
    {
        return new SettingsObservableModel(_storageService.LoadSettings());
    }

    public static async Task<SettingsObservableModel> FromStorageAsync()
    {
        return new SettingsObservableModel(await _storageService.LoadSettingsAsync());
    }

    public SettingsModel ToModel()
    {
        return new SettingsModel
        {
            DefaultExportDirectory = DefaultExportDirectory,
            ResolutionWidth = ResolutionWidth,
            ResolutionHeight = ResolutionHeight,
            Theme = Theme,
            Language = Language,
            MediaPlayerColWidth = MediaPlayerColWidth,
            ThemeExplorerRowHeight = ThemeExplorerRowHeight
        };
    }

    // To save GridSplitters' position
    private void AutoSave(object? sender, ElapsedEventArgs e)
    {
        _ = _storageService.SaveSettings(ToModel());
    }
}
