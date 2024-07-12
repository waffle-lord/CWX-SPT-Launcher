using CWX_SPT_Launcher.Helpers;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;

namespace CWX_SPT_Launcher;

public partial class Main : Form
{
    private SettingsHelper _settingsHelper = null;
    private bool closeWhenIMeanIt = false;
    public static readonly string _appPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "EftApp\\Resources");
    
    public Main()
    {
        _settingsHelper = SettingsHelper.Instance;
        
        SetUpNotificationIcon();
        
        InitializeComponent(_settingsHelper.GetSettings());
        
        if (_settingsHelper.GetSettings().DebugSettings.DebugLocation)
        {
            SetLocationToName();
        }
        
        SetUpBlazorWebView();
        
        this.FormClosing += (sender, args) => Main_FormClosing(sender, args);
    }
    
    private void SetUpNotificationIcon()
    {
        this.notifyIcon = new NotifyIcon();
        this.notifyIcon.Icon = new System.Drawing.Icon(Path.Combine(_appPath, "app.ico"));
        this.notifyIcon.Visible = true;
        this.notifyIcon.Text = "Eft App";
        this.notifyIcon.MouseClick += new MouseEventHandler(NotifyIconOnClick);
        this.notifyIcon.ContextMenuStrip = new ContextMenuStrip();
        this.notifyIcon.ContextMenuStrip.Items.Add("Open", null, (s, e) =>
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        });
        this.notifyIcon.ContextMenuStrip.Items.Add("Hide", null, (s, e) =>
        {
            this.Hide();
            this.WindowState = FormWindowState.Minimized;
        });
        this.notifyIcon.ContextMenuStrip.Items.Add("Exit", null, (s, e) =>
        {
            closeWhenIMeanIt = true;
            Application.Exit();
        });
    }
    
    private void SetLocationToName()
    {
        Task.Factory.StartNew(async () =>
        {
            var historicLocationPoint = new Point(0, 0);
            var historicSize = new Size(0, 0);

            while (true)
            {
                await Task.Delay(3000);
                var location = this.Location;
                var size = this.Size;

                if (location == historicLocationPoint && size == historicSize)
                    continue;

                historicLocationPoint = location;
                historicSize = size;
                this.Text = $"LOCATION: {historicLocationPoint.ToString()}, SIZE: {historicSize.ToString()}";
            }
        });
    }
    
    private void NotifyIconOnClick(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }
    }

    private void Main_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (_settingsHelper.GetSettings().AppSettings.CloseToTray && !closeWhenIMeanIt)
        {
            e.Cancel = true;
            this.Hide();
            this.WindowState = FormWindowState.Minimized;
        }
    }
    
    private void SetUpBlazorWebView()
    {
        var services = new ServiceCollection();
        services.AddWindowsFormsBlazorWebView();
        services.AddBlazorWebViewDeveloperTools();

        var blazorWebView = new BlazorWebView()
        {
            Dock = DockStyle.Fill,
            HostPage = Path.Combine(_appPath, "index.html"),
            Services = services.BuildServiceProvider()
        };

        blazorWebView.RootComponents.Add<BlazorApp>("#app");
        Controls.Add(blazorWebView);
    }
}