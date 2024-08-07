using System.Text.Json.Serialization;

namespace CWX_SPT_Launcher_Backend.SPT.Response;

public class VersionResponse
{
    [JsonPropertyName("response")] public SPTVersion Response { get; set; } = new SPTVersion();
}