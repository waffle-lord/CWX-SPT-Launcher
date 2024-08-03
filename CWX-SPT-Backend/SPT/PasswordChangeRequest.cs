using System.ComponentModel.DataAnnotations;

namespace CWX_SPT_Launcher_Backend.SPT;

public class PasswordChangeRequest : LoginRequest
{
    [Required] public string change { get; set; } = "";
}