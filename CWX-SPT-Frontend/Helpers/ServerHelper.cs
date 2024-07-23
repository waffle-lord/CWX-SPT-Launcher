using System.Net;
using System.Net.Http.Json;
using System.Security.Authentication;
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
    public HttpClient NetClient = null;

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

    public async Task IsServerReachable(Servers server)
    {
        NetClient = new HttpClient();
        NetClient.BaseAddress = new Uri("http://" + server.Ip);
        var task = await NetClient.GetAsync("/launcher/ping");
    }
    
    public async Task IsServerReachable(Servers server, CancellationToken token)
    {
        NetClient = new HttpClient();
        NetClient.Timeout = TimeSpan.FromSeconds(3);
        NetClient.BaseAddress = new Uri("http://" + server.Ip);
        var task = await NetClient.GetAsync("/launcher/ping", token);
    }

    public async Task GetServerProfiles()
    {
        var task = await NetClient.GetAsync("/launcher/profiles");
        var result = SimpleZlib.Decompress(await task.Content.ReadAsByteArrayAsync(), null);
        ProfileList = JsonSerializer.Deserialize<List<ServerProfile>>(result);
    }
    
    public async Task GetServerProfiles(CancellationToken token)
    {
        var task = await NetClient.GetAsync("/launcher/profiles", token);
        var result = SimpleZlib.Decompress(await task.Content.ReadAsByteArrayAsync(), null);
        ProfileList = JsonSerializer.Deserialize<List<ServerProfile>>(result);
    }

    public async Task GetCreationTypes()
    {
        var task = await NetClient.GetAsync("/launcher/server/connect");
        var result = SimpleZlib.Decompress(await task.Content.ReadAsByteArrayAsync(), null);
        ServerInfo = JsonSerializer.Deserialize<ServerInfo>(result);
    }
    
    public async Task GetCreationTypes(CancellationToken token)
    {
        var task = await NetClient.GetAsync("/launcher/server/connect", token);
        var result = SimpleZlib.Decompress(await task.Content.ReadAsByteArrayAsync(), null);
        ServerInfo = JsonSerializer.Deserialize<ServerInfo>(result);
    }

    public async Task SendProfileRegister(RegisterRequest request)
    {
        var task = await NetClient.PutAsync("/launcher/profile/register",
            new ByteArrayContent(SimpleZlib.CompressToBytes(JsonSerializer.Serialize(request),
                zlibConst.Z_BEST_COMPRESSION)));
        
        var result = SimpleZlib.Decompress(await task.Content.ReadAsByteArrayAsync(), null);
        Console.WriteLine(result); // currently returns "FAILED | OK"
        await GetServerProfiles(); // get them again to repopulate TODO: rework server side API
    }
    
    public async Task SendProfileRegister(RegisterRequest request, CancellationToken token)
    {
        var task = await NetClient.PutAsync("/launcher/profile/register",
            new ByteArrayContent(SimpleZlib.CompressToBytes(JsonSerializer.Serialize(request),
                zlibConst.Z_BEST_COMPRESSION)), token);
        
        var result = SimpleZlib.Decompress(await task.Content.ReadAsByteArrayAsync(), null);
        Console.WriteLine(result); // currently returns "FAILED | OK"
        await GetServerProfiles(); // get them again to repopulate TODO: rework server side API
    }

    public async Task SendProfileDelete(ServerProfile profile)
    {
        NetClient.DefaultRequestHeaders.Add("Cookie", $"PHPSESSID={profile.profileId}");
        NetClient.DefaultRequestHeaders.Add("SessionId", profile.profileId);
        
        var task = await NetClient.GetAsync("/launcher/profile/remove");
        var result = SimpleZlib.Decompress(await task.Content.ReadAsByteArrayAsync(), null);
        Console.WriteLine(result); // True or False as a string
        await GetServerProfiles();
        
        NetClient.DefaultRequestHeaders.Remove("Cookie");
        NetClient.DefaultRequestHeaders.Remove("SessionId");
    }
    
    public async Task SendProfileDelete(ServerProfile profile, CancellationToken token)
    {
        NetClient.DefaultRequestHeaders.Add("Cookie", $"PHPSESSID={profile.profileId}");
        NetClient.DefaultRequestHeaders.Add("SessionId", profile.profileId);
        
        var task = await NetClient.GetAsync("/launcher/profile/remove", token);
        var result = SimpleZlib.Decompress(await task.Content.ReadAsByteArrayAsync(), null);
        Console.WriteLine(result); // True or False as a string
        await GetServerProfiles();
        
        NetClient.DefaultRequestHeaders.Remove("Cookie");
        NetClient.DefaultRequestHeaders.Remove("SessionId");
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