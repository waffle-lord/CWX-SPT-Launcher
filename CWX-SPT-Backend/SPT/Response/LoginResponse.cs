using System.Text.Json.Serialization;

namespace CWX_SPT_Launcher_Backend.SPT.Response;

public class LoginResponse : ISptResponse<bool>
{
    public bool Response { get; set; }
}