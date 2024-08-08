using System.Net.Http;
using System.Text.Json;
using ComponentAce.Compression.Libs.zlib;
using CWX_SPT_Launcher_Backend.CWX;
using CWX_SPT_Launcher_Backend.SPT;
using CWX_SPT_Launcher_Backend.SPT.Response;

namespace CWX_SPT_Frontend.Helpers;

public class ServerHelper
{
    private static ServerHelper _instance;
    private static readonly object Lock = new object();
    public List<ServerProfile> ProfileList = [];
    public Dictionary<string, string> ProfileTypes = new Dictionary<string, string>();
    public Dictionary<string, SPTMod> ModList = [];
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

    public async Task<bool> GetAsync<T>(string url, CancellationToken token)
    {
        var task = await _netClient.GetAsync(url, token);
        var result = JsonSerializer.Deserialize<T>(SimpleZlib.Decompress(await task.Content.ReadAsByteArrayAsync(token)));

        switch (result)
        {
            case ProfilesResponse casting1:
                ProfileList = casting1.Response;
                return true;
            case TypesResponse casting2:
                ProfileTypes = casting2.Response;
                return true;
            case ModsResponse casting3:
                ModList = casting3.Response;
                return true;
            case PingResponse casting4:
                return casting4.Response == "pong!";
            default:
                return false;
        }
    }

    public async Task<bool> PutAsync<T, U>(string url, U request, CancellationToken token)
    {
        var content = new ByteArrayContent(SimpleZlib.CompressToBytes(JsonSerializer.Serialize(request), zlibConst.Z_BEST_COMPRESSION));
        var task = await _netClient.PutAsync(url, content, token);
        var result = JsonSerializer.Deserialize<T>(SimpleZlib.Decompress(await task.Content.ReadAsByteArrayAsync(token)));

        switch (result)
        {
            case RegisterResponse casting1:
                ProfileList = casting1.Profiles;
                return casting1.Response;
            case RemoveResponse casting2:
                ProfileList = casting2.Profiles;
                return casting2.Response;
            case PasswordChangeResponse casting3:
                ProfileList = casting3.Profiles;
                return casting3.Response;
            default:
                return false;
        }
    }

    public void SetupHttpClient(Servers server)
    {
        _netClient = new HttpClient();
        _netClient.BaseAddress = new Uri("http://" + server.Ip);
    }

    public void LogoutAndDispose()
    {
        ProfileList = [];
        ModList = [];
        ProfileTypes = new Dictionary<string, string>();
        ConnectedServer = null;
        _netClient = null;
    }

    public void Login(Servers server)
    {
        ConnectedServer = server;
    }
}