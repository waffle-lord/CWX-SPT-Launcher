using System.ComponentModel.DataAnnotations;

namespace CWX_SPT_Backend.Models.SPT;

public class PasswordChangeRequest : LoginRequest
{
    [Required] public string change { get; set; }
}