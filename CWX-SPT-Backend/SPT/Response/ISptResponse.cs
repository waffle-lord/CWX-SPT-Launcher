using System.Text.Json.Serialization;

namespace CWX_SPT_Launcher_Backend.SPT.Response;

public interface ISptResponse<T>
{
    public T Response { get; set; }
}