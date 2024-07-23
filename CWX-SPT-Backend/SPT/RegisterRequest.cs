using System.ComponentModel.DataAnnotations;

namespace CWX_SPT_Backend.Models.SPT;

public class RegisterRequest
{
    [Required] public string username { get; set; }
    public string password { get; set; }
    [Required] public string edition { get; set; }
}