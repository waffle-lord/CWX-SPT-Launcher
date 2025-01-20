using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Shapes;
using CWX_SPT_Frontend.Helpers;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using Serilog;
using Serilog.Events;
using SPT.Launcher;
using Path = System.IO.Path;

namespace CWX_SPT_Frontend;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private static Window WindowMain;
    private SettingsHelper _settings;

    public MainWindow()
    {
        _settings = SettingsHelper.Instance;

        InitializeComponent();
        CustomizeComponent();

        var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "user", "logs");

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.File(logPath,
                rollingInterval: RollingInterval.Day,
                restrictedToMinimumLevel: LogEventLevel.Debug,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Context}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            
            .CreateLogger();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging(builder => builder.AddSerilog());
        serviceCollection.AddWpfBlazorWebView();
        serviceCollection.AddBlazorWebViewDeveloperTools();
        serviceCollection.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopCenter;
            config.SnackbarConfiguration.PreventDuplicates = false;
            config.SnackbarConfiguration.VisibleStateDuration = 2000;
            config.SnackbarConfiguration.ShowTransitionDuration = 100;
            config.SnackbarConfiguration.HideTransitionDuration = 100;
        });
        
        Resources.Add("services", serviceCollection.BuildServiceProvider());

        IntPtr hWnd = new WindowInteropHelper(this).EnsureHandle();
        bool value = true;
        DwmSetWindowAttribute(
            hWnd,
            DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE,
            ref value,
            Marshal.SizeOf<bool>());

        WindowMain = this;
    }

    private void CustomizeComponent()
    {
        Topmost = _settings.GetSettings().AppSettings.AlwaysTop;

        if (_settings.GetSettings().FirstRun)
        {
            Width = MinWidth;
            Height = MinHeight;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
        else
        {
            Width = _settings.GetSettings().AppSettings.StartSize.Width;
            Height = _settings.GetSettings().AppSettings.StartSize.Height;

            WindowStartupLocation = WindowStartupLocation.Manual;

            Top = _settings.GetSettings().AppSettings.StartLocation.X;
            Left = _settings.GetSettings().AppSettings.StartLocation.Y;
        }
    }

    public static void ChangeTopMostSetting(bool setting)
    {
        WindowMain.Topmost = setting;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        _settings.SetClientLocationSettings((int) Top, (int) Left);
        _settings.SetClientSizeSettings((int) Height, (int) Width);
        _settings.SetFirstRun(false);
        base.OnClosing(e);
    }

    [DllImport("Dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(
        IntPtr hwnd,
        DWMWINDOWATTRIBUTE attribute,
        [In] ref bool pvAttribute,
        int cbAttribute);

    private enum DWMWINDOWATTRIBUTE
    {
        DWMWA_USE_IMMERSIVE_DARK_MODE = 20,
    }
}