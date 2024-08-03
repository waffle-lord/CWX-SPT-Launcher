namespace CWX_SPT_Launcher_Backend.SPT;

public class ServerInfo
{
    public string backendUrl { get; set; }
    public string name { get; set; }
    public List<string> editions { get; set; }
    public Dictionary<string, string> profileDescriptions { get; set; }
}