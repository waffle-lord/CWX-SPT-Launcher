using System.Text.Json.Serialization;

namespace CWX_SPT_Launcher_Backend.SPT.Response;

public class RegisterResponse : ISptResponse<bool>
{
    public bool Response { get; set; }

    public List<ServerProfile> Profiles { get; set; } = [];
}