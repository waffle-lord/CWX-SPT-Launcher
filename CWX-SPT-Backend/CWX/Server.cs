using System.ComponentModel.DataAnnotations;

namespace CWX_SPT_Launcher_Backend.CWX;

public class Server
{
    [Required] public string Ip { get; set; }
    public string Name { get; set; }
    public string ServerId { get; set; }
    public Uri BackenUrl => new($"https://{Ip}");
}