using System.ComponentModel.DataAnnotations;

namespace CWX_SPT_Launcher_Backend.SPT;

public class RegisterRequest
{
    [Required] public string username { get; set; }
    public string password { get; set; } = "";
    [Required] public string edition { get; set; }
}