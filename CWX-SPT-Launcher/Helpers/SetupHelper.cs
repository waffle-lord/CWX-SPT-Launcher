using System.Reflection;

namespace CWX_SPT_Launcher.Helpers;

public class SetupHelper
{
    private static SetupHelper? _instance;
    private static readonly object Lock = new();
    private readonly string _appPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "EftApp\\Resources");
    private Dictionary<string, string> _resourcePathing;

    private SetupHelper()
    {
        _resourcePathing = new Dictionary<string, string>();
        _resourcePathing.Add("WinformsBlazor.Resources.index.html", "index.html");
        _resourcePathing.Add("WinformsBlazor.Resources.app.js", "app.js");
        _resourcePathing.Add("WinformsBlazor.Resources.app.css", "app.css");
        _resourcePathing.Add("WinformsBlazor.Resources.app.ico", "app.ico");
    }

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
        if (!Directory.Exists(_appPath))
        {
            Directory.CreateDirectory(_appPath);
        }
    }

    public void SetupResources()
    {
        var assembly = Assembly.GetExecutingAssembly();

        foreach (var resource in _resourcePathing)
        {
            using var stream = assembly.GetManifestResourceStream(resource.Key);
            if (stream == null)
            {
                Console.WriteLine($@"Resource {resource.Key} not found.");
                return;
            }

            using (var fileStream = new FileStream(Path.Combine(_appPath, resource.Value), FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(fileStream);
            }

            Console.WriteLine($@"Saved {resource.Key} to {resource.Value}");
        }
    }
}