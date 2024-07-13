using CWX_SPT_Launcher.Helpers;

namespace CWX_SPT_Launcher;

static class Program
{
    [STAThread]
    static void Main()
    {
        SetupHelper.Instance.SetupDirectories();
        SetupHelper.Instance.SetupResources();
        ApplicationConfiguration.Initialize();
        Application.Run(new Main());
    }
}