namespace CWX_SPT_Backend.CWX;

public class SettingsClass
{
    public bool FirstRun { get; set; }
    public AppSettingsClass AppSettings { get; set; }
    public DebugSettingsClass DebugSettings { get; set; }
    public List<ServersClass> Servers { get; set; }
}