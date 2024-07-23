using System.ComponentModel.DataAnnotations;

namespace CWX_SPT_Backend.CWX;

public class Servers
{
    [Required] public string Ip { get; set; }
    public string Name { get; set; }
    public string ServerId { get; set; }
}