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
    public List<ServerProfileClass> ProfileList = new List<ServerProfileClass>();
    public ServerInfoClass ServerInfo = new ServerInfoClass();
    public string ConnectedServerId = "";
    public bool SelectedProfile = false;
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

    public async Task<bool> IsServerReachable(ServersClass server)
    {
        NetClient.BaseAddress = new Uri("http://" + server.Ip);
        var task = await NetClient.GetAsync("/launcher/ping");
        
        return true;
    }
    
    public async Task<bool> GetServerProfiles(ServersClass server)
    {
        var task = await NetClient.GetAsync("/launcher/profiles");
        var result = SimpleZlib.Decompress(await task.Content.ReadAsByteArrayAsync(), null);
        ProfileList = JsonSerializer.Deserialize<List<ServerProfileClass>>(result);
        return true;
    }

    public async Task<bool> GetCreationTypes(ServersClass server)
    {
        var task = await NetClient.GetAsync("/launcher/server/connect");
        var result = SimpleZlib.Decompress(await task.Content.ReadAsByteArrayAsync(), null);
        ServerInfo = JsonSerializer.Deserialize<ServerInfoClass>(result);
        return true;
    }

    public void LogoutAndDispose()
    {
        // null profiles
        ProfileList = new List<ServerProfileClass>();
        ServerInfo = new ServerInfoClass();
        ConnectedServerId = "";
        SelectedProfile = false;
        NetClient = new HttpClient();
    }

    public void Login(string serverId)
    {
        ConnectedServerId = serverId;
    }
}