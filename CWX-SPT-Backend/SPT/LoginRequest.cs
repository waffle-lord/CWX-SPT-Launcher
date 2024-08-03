using System.ComponentModel.DataAnnotations;

namespace CWX_SPT_Launcher_Backend.SPT;

public class LoginRequest
{
    [Required] public string username { get; set; } = "";
    [Required] public string password { get; set; } = "";
}