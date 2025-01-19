using System.Text.Json.Serialization;

namespace CWX_SPT_Launcher_Backend.SPT.Response;

public class ModsResponse : ISptResponse<Dictionary<string, SPTMod>>
{
    public Dictionary<string, SPTMod> Response { get; set; } = new Dictionary<string, SPTMod>();
}