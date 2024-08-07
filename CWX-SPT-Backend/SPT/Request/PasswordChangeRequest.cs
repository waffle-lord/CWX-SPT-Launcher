using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CWX_SPT_Launcher_Backend.SPT.Request;

public class PasswordChangeRequest : LoginRequest
{
    [Required] [JsonPropertyName("change")] public string Change { get; set; } = "";
}