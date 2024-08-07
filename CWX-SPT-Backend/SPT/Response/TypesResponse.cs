using System.Text.Json.Serialization;

namespace CWX_SPT_Launcher_Backend.SPT.Response;

public class TypesResponse
{
    [JsonPropertyName("response")] public Dictionary<string, string> Response { get; set; } = new Dictionary<string, string>();
}