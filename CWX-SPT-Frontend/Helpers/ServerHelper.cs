using System.Net;
using System.Text.Json;
using ComponentAce.Compression.Libs.zlib;
using CWX_SPT_Launcher_Backend.CWX;
using CWX_SPT_Launcher_Backend.SPT;

namespace CWX_SPT_Frontend.Helpers;

public class ServerHelper
{
    private static ServerHelper _instance;
    private static readonly object Lock = new object();
    public List<ServerProfile> ProfileList = [];
    public ServerInfo ServerInfo = new ServerInfo();
    public Servers ConnectedServer;
    private HttpClient _netClient;

    public static ServerHelper Instance
    {
        get
        {
            lock (Lock)
            {
                return _instance ??= new ServerHelper();
            }
        }
    }

    public async Task<bool> IsServerReachable(Servers server, CancellationToken token)
    {
        _netClient = new HttpClient();
        _netClient.BaseAddress = new Uri("http://" + server.Ip);
        var task = await _netClient.GetAsync("/launcher/ping", token);
        if (task.StatusCode != HttpStatusCode.OK)
        {
            return false;
        }
        
        var result = SimpleZlib.Decompress(await task.Content.ReadAsByteArrayAsync(token));
        return result == "\"pong!\"";
    }

    public async Task<bool> GetServerProfiles(CancellationToken token)
    {
        var task = await _netClient.GetAsync("/launcher/profiles", token);
        if (task.StatusCode != HttpStatusCode.OK)
        {
            return false;
        }

        var result = SimpleZlib.Decompress(await task.Content.ReadAsByteArrayAsync(token));
        ProfileList = JsonSerializer.Deserialize<List<ServerProfile>>(result);
        return true;
    }

    public async Task<bool> GetCreationTypes(CancellationToken token)
    {
        var task = await _netClient.GetAsync("/launcher/server/connect", token);
        if (task.StatusCode != HttpStatusCode.OK)
        {
            return false;
        }

        var result = SimpleZlib.Decompress(await task.Content.ReadAsByteArrayAsync(token));
        ServerInfo = JsonSerializer.Deserialize<ServerInfo>(result);
        return true;
    }

    public async Task<bool> SendProfileRegister(RegisterRequest request, CancellationToken token)
    {
        var task = await _netClient.PutAsync("/launcher/profile/register",
            new ByteArrayContent(SimpleZlib.CompressToBytes(JsonSerializer.Serialize(request),
                zlibConst.Z_BEST_COMPRESSION)), token);
        if (task.StatusCode != HttpStatusCode.OK)
        {
            return false;
        }

        var result = SimpleZlib.Decompress(await task.Content.ReadAsByteArrayAsync(token));
        if (result == "FAILED")
        {
            return false;
        }

        await GetServerProfiles(token);
        return true;
    }

    public async Task<bool> SendProfileDelete(ServerProfile profile, CancellationToken token)
    {
        _netClient.DefaultRequestHeaders.Add("Cookie", $"PHPSESSID={profile.profileId}");
        _netClient.DefaultRequestHeaders.Add("SessionId", profile.profileId);

        var task = await _netClient.GetAsync("/launcher/profile/remove", token);
        if (task.StatusCode != HttpStatusCode.OK)
        {
            return false;
        }
        
        var result = SimpleZlib.Decompress(await task.Content.ReadAsByteArrayAsync(token));
        if (result == "false")
        {
            return false;
        }
        await GetServerProfiles(token);

        _netClient.DefaultRequestHeaders.Remove("Cookie");
        _netClient.DefaultRequestHeaders.Remove("SessionId");
        return true;
    }

    public async Task<bool> SendPasswordChange(PasswordChangeRequest request, CancellationToken token)
    {
        var task = await _netClient.PutAsync("/launcher/profile/change/password",
            new ByteArrayContent(SimpleZlib.CompressToBytes(JsonSerializer.Serialize(request),
                zlibConst.Z_BEST_COMPRESSION)), token);
        var result = SimpleZlib.Decompress(await task.Content.ReadAsByteArrayAsync(token));

        if (result == "FAILED")
        {
            return false;
        }

        await GetServerProfiles(token);
        return true;
    }

    public void LogoutAndDispose()
    {
        ProfileList = [];
        ServerInfo = new ServerInfo();
        ConnectedServer = null;
        _netClient = new HttpClient();
    }

    public void Login(Servers server)
    {
        ConnectedServer = server;
    }
}