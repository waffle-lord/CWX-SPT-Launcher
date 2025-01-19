using System.Text.Json.Serialization;

namespace CWX_SPT_Launcher_Backend.SPT;

public class ServerInfo
{
    public Dictionary<string, string> Types { get; set; } = new Dictionary<string, string>();
}