namespace CWX_SPT_Launcher;

static class Program
{
    [STAThread]
    static void Main()
    {
        
        ApplicationConfiguration.Initialize();
        Application.Run(new Main());
    }
}