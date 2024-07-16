using CWX_SPT_Launcher.Models;

namespace CWX_SPT_Launcher.Helpers;

public class ServerHelper
{
    private static ServerHelper _instance = null;
    private static readonly object Lock = new object();
    private object _profiles = new object();
    public string ConnectedServerId = "";
    public bool SelectedProfile = false;
    
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
        await Task.Delay(1000);
        return true;
    }
    
    public async Task<bool> GetServerProfiles(ServersClass server)
    {
        await Task.Delay(1000);
        return true;
    }
    
    // TODO: Remove
    public async Task<bool> GetFakeMessage1(ServersClass server)
    {
        await Task.Delay(1000);
        return true;
    }
    
    // TODO: Remove
    public async Task<bool> GetFakeMessage2(ServersClass server)
    {
        await Task.Delay(1000);
        return true;
    }
    
    // TODO: Remove
    public async Task<bool> GetFakeMessage3(ServersClass server)
    {
        await Task.Delay(1000);
        return true;
    }

    public async Task LogoutAndDispose()
    {
        // null profiles
        _profiles = null;
        ConnectedServerId = "";
        SelectedProfile = false;
    }

    public async Task Login(string serverId)
    {
        ConnectedServerId = serverId;
    }
}