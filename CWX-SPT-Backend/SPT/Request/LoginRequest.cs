using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CWX_SPT_Launcher_Backend.SPT.Request;

public class LoginRequest
{
    [Required] [JsonPropertyName("username")] public string Username { get; set; } = "";

    [Required] [JsonPropertyName("password")] public string Password { get; set; } = "";
}