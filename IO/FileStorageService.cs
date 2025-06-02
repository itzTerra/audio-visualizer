using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AudioVisualizer.Extensions;
using AudioVisualizer.IO.Json.Serializers;
using AudioVisualizer.Models;
using AudioVisualizer.Services;
using AudioVisualizer.Utils;

namespace AudioVisualizer.IO;

public class FileStorageService : IStorageService
{
    private readonly string _settingsConfigPath;
    private readonly string _themesPath;

    private readonly SettingsSerializer _settingsSerializer = new();
    private readonly ThemeNodesSerializer _themeNodeSerializer = new();

    public FileStorageService()
    {
        _settingsConfigPath = GetPath("settings.json");
        _themesPath = GetPath("themes.json");
    }

    private string GetAppDataPath()
    {
        if (AppConfig.IsDebug)
        {
            return Path.Join(Environment.CurrentDirectory, "Data");
        }
        return Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppConfig.AppName);
    }

    private string GetPath(string path)
    {
        return Path.Join(GetAppDataPath(), path);
    }

    /// <summary>
    /// Writes text to a file, creating any missing intermediate directories.
    /// </summary>
    private async Task WriteText(string filePath, string content)
    {
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        await File.WriteAllTextAsync(filePath, content);
    }

    private async Task WriteTextSafe(string filePath, string content, bool notify = true)
    {
        try
        {
            await WriteText(filePath, content);
        }
        catch (Exception ex)
        {
            if (notify)
            {
                Notifier.Error(string.Format("Error writing to file {0}: {1}", filePath, ex.Message));
            }
        }
    }

    private string? ReadText(string filePath)
    {
        if (!File.Exists(filePath))
            return null;

        return File.ReadAllText(filePath);
    }

    private async Task<string?> ReadTextAsync(string filePath)
    {
        if (!File.Exists(filePath))
            return null;

        return await File.ReadAllTextAsync(filePath);
    }


    public SettingsModel LoadSettings()
    {
        var defaultSettings = new SettingsModel();
        if (!File.Exists(_settingsConfigPath))
        {
            _ = WriteTextSafe(_settingsConfigPath, _settingsSerializer.Serialize(defaultSettings), false);
            return defaultSettings;
        }
        var textContent = ReadText(_settingsConfigPath);
        var existingSettings = _settingsSerializer.Deserialize(textContent);
        if (existingSettings is null)
        {
            Notifier.Error("Error loading settings, restoring defaults...");
            _ = WriteTextSafe(_settingsConfigPath, _settingsSerializer.Serialize(defaultSettings), false);
            return defaultSettings;
        }
        return existingSettings;
    }

    public async Task<SettingsModel> LoadSettingsAsync()
    {
        var defaultSettings = new SettingsModel();
        if (!File.Exists(_settingsConfigPath))
        {
            _ = WriteTextSafe(_settingsConfigPath, _settingsSerializer.Serialize(defaultSettings), false);
            return defaultSettings;
        }
        var textContent = await ReadTextAsync(_settingsConfigPath);
        var existingSettings = _settingsSerializer.Deserialize(textContent);
        if (existingSettings is null)
        {
            Notifier.Error("Error loading settings, restoring defaults...");
            _ = WriteTextSafe(_settingsConfigPath, _settingsSerializer.Serialize(defaultSettings), false);
            return defaultSettings;
        }
        return existingSettings;
    }

    public async Task SaveSettings(SettingsModel settings)
    {
        var textContent = _settingsSerializer.Serialize(settings);
        try
        {
            await WriteText(_settingsConfigPath, textContent);
        }
        catch
        {
            Notifier.Error("Error saving settings");
        }
    }

    public ThemeNodeModel[] LoadThemes()
    {
        var defaultThemes = AppConfig.DefaultThemes;
        if (!File.Exists(_themesPath))
        {
            _ = WriteTextSafe(_themesPath, _themeNodeSerializer.Serialize(defaultThemes), false);
            return defaultThemes;
        }
        var textContent = ReadText(_themesPath);
        var existingThemes = _themeNodeSerializer.Deserialize(textContent);
        if (existingThemes is null || existingThemes.Length == 0)
        {
            Notifier.Error("Error loading custom themes.");
            _ = WriteTextSafe(_themesPath, _themeNodeSerializer.Serialize(defaultThemes), false);
            return defaultThemes;
        }

        var x = GetThemesWithDefaults(existingThemes, defaultThemes);
        return x;
    }

    public async Task<ThemeNodeModel[]> LoadThemesAsync()
    {
        var defaultThemes = AppConfig.DefaultThemes;
        if (!File.Exists(_themesPath))
        {
            _ = WriteTextSafe(_themesPath, _themeNodeSerializer.Serialize(defaultThemes), false);
            return defaultThemes;
        }
        var textContent = await ReadTextAsync(_themesPath);
        var existingThemes = _themeNodeSerializer.Deserialize(textContent);
        if (existingThemes is null || existingThemes.Length == 0)
        {
            Notifier.Error("Error loading custom themes.");
            _ = WriteTextSafe(_themesPath, _themeNodeSerializer.Serialize(defaultThemes), false);
            return defaultThemes;
        }
        return GetThemesWithDefaults(existingThemes, defaultThemes);
    }

    private ThemeNodeModel? FindNodeById(ThemeNodeModel[] nodes, string id)
    {
        foreach (var node in nodes)
        {
            if (node.Id == id)
            {
                return node;
            }

            var childNode = FindNodeById(node.Nodes.ToArray(), id);
            if (childNode != null)
            {
                return childNode;
            }
        }
        return null;
    }

    private ThemeNodeModel[] GetThemesWithDefaults(ThemeNodeModel[] existingThemes, ThemeNodeModel[] defaultThemes)
    {
        // If any of the default themes are missing (check by ID), add them under the "Default" theme node
        // Search recursively for the default node
        IEnumerable<ThemeNodeModel> existingThemesFlat = existingThemes.SelectManyRecursive(t => t.Nodes);
        var defaultNode = FindNodeById(existingThemes, AppConfig.DefaultThemeNodeId);
        bool addedNodes = false;
        foreach (var theme in defaultThemes[0].Nodes)
        {
            if (existingThemesFlat.All(t => t.Id != theme.Id))
            {
                addedNodes = true;
                if (defaultNode is null)
                {
                    defaultNode = new ThemeNodeModel("Default", true, false)
                    {
                        Id = AppConfig.DefaultThemeNodeId
                    };
                    existingThemes = existingThemes.Prepend(defaultNode).ToArray();
                }
                defaultNode.Nodes.Add(theme);
            }
        }
        if (addedNodes)
        {
            _ = WriteTextSafe(_themesPath, _themeNodeSerializer.Serialize(existingThemes), false);
        }
        return existingThemes;
    }

    public async Task SaveThemes(ThemeNodeModel[] themes)
    {
        var textContent = _themeNodeSerializer.Serialize(themes);
        try
        {
            await WriteText(_themesPath, textContent);
        }
        catch
        {
            Notifier.Error("Error saving custom themes");
        }
    }
}
