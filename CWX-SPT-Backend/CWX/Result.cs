using Serilog;

namespace CWX_SPT_Launcher_Backend.CWX;

public class Result<T> where T : class
{
    private readonly string _typeName;
    public readonly string Message;
    public readonly bool Succeeded;
    public bool HasException => Exception != null;
    public readonly Exception? Exception;
    
    private Result(string typeName, string message, bool succeeded, Exception? exception = null)
    {
        _typeName = typeName;
        Message = message;
        Succeeded = succeeded;
        Exception = exception;

        if (HasException)
        {
            Log.ForContext("Context", typeName).Error(Exception, "An exception was thrown");
            return;
        }

        if (Succeeded)
        {
            Log.ForContext("Context", typeName).Information(message);
            return;
        }
        
        Log.ForContext("Context", typeName).Error(Message);
    }
    
    public static Result<T> FromSuccess(string message) => new(nameof(T) , message, true);
    public static Result<T> FromError(string message) => new(nameof(T), message, false);
    public static Result<T> FromException(Exception ex, string message = "") => new(nameof(T), message, false, ex);
}