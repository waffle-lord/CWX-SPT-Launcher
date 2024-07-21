using System.Reflection;

namespace CWX_SPT_Frontend.Helpers;

public class SetupHelper
{
    private static SetupHelper _instance;
    private static readonly object Lock = new();
    private Dictionary<string, string> _resourcePathing = new Dictionary<string, string>();

    public static SetupHelper Instance
    {
        get
        {
            lock (Lock)
            {
                return _instance ??= new SetupHelper();
            }
        }
    }

    public void SetupDirectories()
    {
        if (!Directory.Exists(Main.AppPath))
        {
            Directory.CreateDirectory(Main.AppPath);
        }
    }

    public void SetupResources()
    {
        PrecheckResources();
        var assembly = Assembly.GetExecutingAssembly();

        foreach (var resource in _resourcePathing)
        {
            using var stream = assembly.GetManifestResourceStream(resource.Key);
            if (stream == null)
            {
                Console.WriteLine($@"Resource {resource.Key} not found.");
                return;
            }

            using (var fileStream = new FileStream(Path.Combine(Main.AppPath, resource.Value), FileMode.Create,
                       FileAccess.Write))
            {
                stream.CopyTo(fileStream);
            }

            Console.WriteLine($@"Saved {resource.Key} to {Main.AppPath}\{resource.Value}");
        }
    }

    private bool debugMode = true;

    private void PrecheckResources()
    {
        if (debugMode)
        {
            _resourcePathing.Add("CWX_SPT_Launcher.Resources.index.html", "index.html");
            _resourcePathing.Add("CWX_SPT_Launcher.Resources.app.js", "app.js");
            _resourcePathing.Add("CWX_SPT_Launcher.Resources.app.css", "app.css");
            _resourcePathing.Add("CWX_SPT_Launcher.Resources.app.ico", "app.ico");
        }
        else
        {
            if (!File.Exists(Path.Combine(Main.AppPath, "index.html")))
            {
                _resourcePathing.Add("CWX_SPT_Launcher.Resources.index.html", "index.html");
            }

            if (!File.Exists(Path.Combine(Main.AppPath, "app.js")))
            {
                _resourcePathing.Add("CWX_SPT_Launcher.Resources.app.js", "app.js");
            }

            if (!File.Exists(Path.Combine(Main.AppPath, "app.css")))
            {
                _resourcePathing.Add("CWX_SPT_Launcher.Resources.app.css", "app.css");
            }

            if (!File.Exists(Path.Combine(Main.AppPath, "app.ico")))
            {
                _resourcePathing.Add("CWX_SPT_Launcher.Resources.app.ico", "app.ico");
            }

            if (!File.Exists(Path.Combine(Main.AppPath, "settings.json")))
            {
                _resourcePathing.Add("CWX_SPT_Launcher.Resources.settings.json", "settings.json");
            }
        }
    }
}