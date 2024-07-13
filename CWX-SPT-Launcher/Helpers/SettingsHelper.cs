using System.Reflection;
using System.Text.Json;
using CWX_SPT_Launcher.Models;

namespace CWX_SPT_Launcher.Helpers;

public class SettingsHelper
{
    private static SettingsHelper _instance = null;
    private static readonly object _lock = new object();
    private SettingsClass _settings = null;

    private SettingsHelper()
    {
        LoadSettingsFromFile();
    }

    public static SettingsHelper Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new SettingsHelper();
                }

                return _instance;
            }
        }
    }

    private void LoadSettingsFromFile()
    {
        _settings = JsonSerializer.Deserialize<SettingsClass>(
            File.ReadAllText(Path.Combine(Main.AppPath, "settings.json")));
    }

    public SettingsClass GetSettings()
    {
        return _settings;
    }

    public void SaveSettings()
    {
        File.WriteAllText(Path.Combine(Main.AppPath, "settings.json"), JsonSerializer.Serialize(_settings));
    }

    public void SetClientSizeSettings(int height, int width)
    {
        _settings.AppSettings.StartSize.Height = height;
        _settings.AppSettings.StartSize.Width = width;
    }

    public void SetClientLocationSettings(int x, int y)
    {
        _settings.AppSettings.StartLocation.X = x;
        _settings.AppSettings.StartLocation.Y = y;
    }

    public void SetFirstRun(bool firstRun)
    {
        _settings.FirstRun = firstRun;
    }

    public void SetCloseToTray(bool closeToTray)
    {
        _settings.AppSettings.CloseToTray = closeToTray;
    }
}