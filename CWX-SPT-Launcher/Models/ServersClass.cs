using System.ComponentModel.DataAnnotations;

namespace CWX_SPT_Launcher.Models;

public class ServersClass
{
    [Required]
    public string Ip { get; set; }
    public string DefaultProfile { get; set; }
    public bool RemoteServer { get; set; }
    public string Password { get; set; }
}