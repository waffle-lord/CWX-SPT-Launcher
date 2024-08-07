using System.Text.Json.Serialization;

namespace CWX_SPT_Launcher_Backend.SPT.Response;

public class PasswordChangeResponse
{
    [JsonPropertyName("response")] public bool Response { get; set; }
    [JsonPropertyName("profiles")] public List<ServerProfile> Profiles { get; set; } = [];
}