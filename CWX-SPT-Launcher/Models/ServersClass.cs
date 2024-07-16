using System.ComponentModel.DataAnnotations;

namespace CWX_SPT_Launcher.Models;

public class ServersClass
{
    [Required] public string Ip { get; set; } = "";
    public string Name { get; set; } = "";
    public string ServerId { get; private set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(); // just a unix timestamp
}