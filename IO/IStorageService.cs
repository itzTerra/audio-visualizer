using System.Threading.Tasks;
using AudioVisualizer.Models;

namespace AudioVisualizer.IO;

public interface IStorageService
{
    SettingsModel LoadSettings();
    Task<SettingsModel> LoadSettingsAsync();
    Task SaveSettings(SettingsModel settings);

    ThemeNodeModel[] LoadThemes();
    Task<ThemeNodeModel[]> LoadThemesAsync();
    Task SaveThemes(ThemeNodeModel[] themes);
}
