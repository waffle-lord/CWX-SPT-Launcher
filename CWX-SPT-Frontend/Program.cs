using CWX_SPT_Frontend.Helpers;

namespace CWX_SPT_Frontend;

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