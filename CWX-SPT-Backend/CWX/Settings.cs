namespace CWX_SPT_Backend.CWX;

public class Settings
{
    public bool FirstRun { get; set; }
    public AppSettings AppSettings { get; set; }
    public DebugSettings DebugSettings { get; set; }
    public List<Servers> Servers { get; set; }
}