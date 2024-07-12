using System.Text.Json;
using CWX_SPT_Launcher.Models;

namespace CWX_SPT_Launcher.Helpers;

public class SettingsHelper
{
    private static SettingsHelper _instance = null;
    private static readonly object _lock = new object();
    private static string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    private static string settingsPath = Path.Combine(appDataPath, "EftApp", "Settings.json");
    private SettingsClass _settings = null;

    private SettingsHelper()
    {
        PopulateSettings();
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

    public void PopulateSettings()
    {
        if (File.Exists(settingsPath))
        {
            _settings = JsonSerializer.Deserialize<SettingsClass>(File.ReadAllText(settingsPath));
        }
        else
        {
            var settings = new SettingsClass
            {
                FirstRun = true,
                AppSettings = new AppSettingsClass
                {
                    StartLocation = new StartLocationClass()
                    {
                        X = 0,
                        Y = 0
                    },
                    StartSize = new StartSizeClass
                    {
                        Width = 0,
                        Height = 0
                    },
                    CloseToTray = false
                },
                DebugSettings = new DebugSettingsClass
                {
                    DebugLocation = false
                }
            };
            
            File.WriteAllText(settingsPath, JsonSerializer.Serialize(settings));
            _settings = settings;
        }
    }
    
    public SettingsClass GetSettings()
    {
        return _settings;
    }

    public void SaveSettings()
    {
        File.WriteAllText(settingsPath, JsonSerializer.Serialize(_settings));
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