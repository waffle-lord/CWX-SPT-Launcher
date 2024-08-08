using System.IO;
using System.Text.Json;
using CWX_SPT_Launcher_Backend.CWX;
using MudBlazor;

namespace CWX_SPT_Frontend.Helpers;

public class SettingsHelper
{
    private static SettingsHelper _instance;
    private static readonly object Lock = new object();
    private Settings _settings;
    private static readonly string AppPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "CWX-SPT-Launcher\\Resources");

    public readonly DialogOptions DialogOptions = new DialogOptions
    {
        Position = DialogPosition.Center,
        MaxWidth = MaxWidth.ExtraSmall,
        BackdropClick = true,
        CloseOnEscapeKey = true,
        NoHeader = true,
        FullWidth = true,
        BackgroundClass = "dialog-backdrop-class"
    };

    private SettingsHelper()
    {
        LoadSettingsFromFile();
    }

    public static SettingsHelper Instance
    {
        get
        {
            lock (Lock)
            {
                return _instance ??= new SettingsHelper();
            }
        }
    }

    private void LoadSettingsFromFile()
    {
        // check if exists
        if (!File.Exists(Path.Combine(AppPath, "settings.json")))
        {
            SaveDefaults();
        }
        // if not save
        _settings = JsonSerializer.Deserialize<Settings>(
            File.ReadAllText(Path.Combine(AppPath, "settings.json")));
    }

    public Settings GetSettings()
    {
        return _settings;
    }

    public void SaveSettings()
    {
        File.WriteAllText(Path.Combine(AppPath, "settings.json"), JsonSerializer.Serialize(_settings));
    }

    public void SetClientSizeSettings(int height, int width)
    {
        _settings.AppSettings.StartSize.Height = height;
        _settings.AppSettings.StartSize.Width = width;
        SaveSettings();
    }

    public void SetClientLocationSettings(int x, int y)
    {
        _settings.AppSettings.StartLocation.X = x;
        _settings.AppSettings.StartLocation.Y = y;
        SaveSettings();
    }

    public void SetFirstRun(bool firstRun)
    {
        _settings.FirstRun = firstRun;
        SaveSettings();
    }

    public void SetServerSettings(List<Servers> servers)
    {
        _settings.Servers = servers;
        SaveSettings();
    }

    public void SetCloseToTray(bool closeToTray)
    {
        _settings.AppSettings.CloseToTray = closeToTray;
        SaveSettings();
    }

    public void SetMinimizeOnLaunch(bool minimizeOnLaunch)
    {
        _settings.AppSettings.MinimizeOnLaunch = minimizeOnLaunch;
        SaveSettings();
    }

    public void SetAlwaysOnTop(bool alwaysOnTop)
    {
        _settings.AppSettings.AlwaysTop = alwaysOnTop;
        SaveSettings();
        MainWindow.ChangeTopMostSetting(alwaysOnTop);
    }

    public void SetAdvancedUser(bool advancedUser)
    {
        _settings.AppSettings.AdvancedUser = advancedUser;
        SaveSettings();
    }

    public void SetSptPath(string path)
    {
        _settings.AppSettings.SptPath = path;
        SaveSettings();
    }

    public void SetDebugUser(bool debugUser)
    {
        _settings.DebugSettings.DebugUser = debugUser;
        SaveSettings();
    }

    public void SetDebugTitle(bool debug)
    {
        // _settings.DebugSettings.DebugLocation = debug;
        // SaveSettings();
        // call to Main to set title
    }

    public void SetUseProfileColors(bool profileColors)
    {
        _settings.AppSettings.UseProfileColors = profileColors;
        SaveSettings();
    }

    private void SaveDefaults()
    {
        Directory.CreateDirectory(AppPath);
        File.WriteAllText(Path.Combine(AppPath, "settings.json"), GetDefaults());
    }

    private string GetDefaults()
    {
        // work around not being able to read embedded json
        var settings = new Settings
        {
            FirstRun = true,
            AppSettings = new AppSettings
            {
                StartLocation = new StartLocation
                {
                    X = 0,
                    Y = 0
                },
                StartSize = new StartSize
                {
                    Height = 0,
                    Width = 0
                },
                CloseToTray = false,
                MinimizeOnLaunch = false,
                AlwaysTop = false,
                UseProfileColors = true,
                AdvancedUser = false,
                SptPath = @"C:\\SPT\\spt310"
            },
            Servers =
            [
                new Servers
                {
                    Ip = "127.0.0.1:6969",
                    Name = "LocalHost",
                    ServerId = "1721162719"
                }
            ],
            DebugSettings = new DebugSettings()
            {
                DebugLocation = false,
                DebugUser = false
            }
        };

        return JsonSerializer.Serialize(settings);
    }
}