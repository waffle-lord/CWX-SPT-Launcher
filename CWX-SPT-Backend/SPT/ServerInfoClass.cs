namespace CWX_SPT_Backend.Models.SPT;

public class ServerInfoClass
{
    public string backendUrl { get; set; }
    public string name { get; set; }
    public List<string> editions { get; set; }
    public Dictionary<string, string> profileDescriptions { get; set; }
}