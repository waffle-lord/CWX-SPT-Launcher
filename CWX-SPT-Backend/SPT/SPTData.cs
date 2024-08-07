using System.Text.Json.Serialization;

namespace CWX_SPT_Launcher_Backend.SPT;

public class SPTData
{
    [JsonPropertyName("version")] public string Version { get; set; } = "";
}