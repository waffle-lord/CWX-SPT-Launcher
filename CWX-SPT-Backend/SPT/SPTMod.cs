using System.Text.Json.Serialization;

namespace CWX_SPT_Launcher_Backend.SPT;

public class SPTMod
{
    [JsonPropertyName("author")] public string Author { get; set; } = "";
    [JsonPropertyName("name")] public string Name { get; set; } = "";
    [JsonPropertyName("version")] public string Version { get; set; } = "";
    [JsonPropertyName("url")] public string Url { get; set; } = "";
}