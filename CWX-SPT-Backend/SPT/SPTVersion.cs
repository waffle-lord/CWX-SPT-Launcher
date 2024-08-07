using System.Text.Json.Serialization;

namespace CWX_SPT_Launcher_Backend.SPT;

public class SPTVersion
{
    [JsonPropertyName("sptVersion")] public string SptVersion { get; set; } = "";
    [JsonPropertyName("eftVersion")] public string EftVersion { get; set; } = "";
}