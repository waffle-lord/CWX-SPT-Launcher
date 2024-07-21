using System.ComponentModel.DataAnnotations;

namespace CWX_SPT_Frontend.Models;

public class ServersClass
{
    [Required] public string Ip { get; set; } = "";
    public string Name { get; set; } = "";
    public string ServerId { get; set; } // just a unix timestamp
}