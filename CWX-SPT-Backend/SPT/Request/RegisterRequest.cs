using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CWX_SPT_Launcher_Backend.SPT.Request;

public class RegisterRequest : LoginRequest
{
    [Required] [JsonPropertyName("edition")] public string Edition { get; set; } = "Standard";
}