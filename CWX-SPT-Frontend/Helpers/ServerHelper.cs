using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using ComponentAce.Compression.Libs.zlib;
using CWX_SPT_Backend.CWX;
using CWX_SPT_Backend.Models.SPT;

namespace CWX_SPT_Frontend.Helpers;

public class ServerHelper
{
    private static ServerHelper _instance = null;
    private static readonly object Lock = new object();
    public List<ServerProfile> ProfileList = new List<ServerProfile>();
    public ServerInfo ServerInfo = new ServerInfo();
    public Servers ConnectedServer = null;
    public HttpClient NetClient = new HttpClient();

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

    public async Task<bool> IsServerReachable(Servers server)
    {
        NetClient.BaseAddress = new Uri("http://" + server.Ip);
        var task = await NetClient.GetAsync("/launcher/ping");

        return true;
    }

    public async Task<bool> GetServerProfiles(Servers server)
    {
        var task = await NetClient.GetAsync("/launcher/profiles");
        var result = SimpleZlib.Decompress(await task.Content.ReadAsByteArrayAsync(), null);
        ProfileList = JsonSerializer.Deserialize<List<ServerProfile>>(result);
        return true;
    }

    public async Task<bool> GetCreationTypes(Servers server)
    {
        var task = await NetClient.GetAsync("/launcher/server/connect");
        var result = SimpleZlib.Decompress(await task.Content.ReadAsByteArrayAsync(), null);
        ServerInfo = JsonSerializer.Deserialize<ServerInfo>(result);
        return true;
    }

    public async Task<bool> SendProfileRegister(RegisterRequest request)
    {
        var task = await NetClient.PutAsync("/launcher/profile/register",
            new ByteArrayContent(SimpleZlib.CompressToBytes(JsonSerializer.Serialize(request),
                zlibConst.Z_BEST_COMPRESSION)));
        
        var result = SimpleZlib.Decompress(await task.Content.ReadAsByteArrayAsync(), null);
        Console.WriteLine(result); // currently returns "FAILED | OK"
        await GetServerProfiles(ConnectedServer); // get them again to repopulate TODO: rework server side API
        return true;
    }

    public async Task<bool> SendProfileDelete(ServerProfile profile)
    {
        NetClient.DefaultRequestHeaders.Add("Cookie", $"PHPSESSID={profile.profileId}");
        NetClient.DefaultRequestHeaders.Add("SessionId", profile.profileId);
        
        var task = await NetClient.GetAsync("/launcher/profile/remove");
        var result = SimpleZlib.Decompress(await task.Content.ReadAsByteArrayAsync(), null);
        Console.WriteLine(result); // True or False as a string
        await GetServerProfiles(ConnectedServer);
        
        NetClient.DefaultRequestHeaders.Remove("Cookie");
        NetClient.DefaultRequestHeaders.Remove("SessionId");
        return true;
    }

    public void LogoutAndDispose()
    {
        // null profiles
        ProfileList = new List<ServerProfile>();
        ServerInfo = new ServerInfo();
        ConnectedServer = null;
        NetClient = new HttpClient();
    }

    public void Login(Servers server)
    {
        ConnectedServer = server;
    }
}