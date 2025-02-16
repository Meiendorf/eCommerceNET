using Serilog;

namespace eCommerce.SharedLibrary.Logs;

public static class LogExceptions
{
    public static void LogException(Exception ex)
    {
        LogToFile(ex.Message);
        LogToConsole(ex.Message);
        LogToDebugger(ex.Message);
    }

    public static void LogToDebugger(string exMessage)
    {
        Log.Debug(exMessage);
    }

    public static void LogToConsole(string exMessage)
    {
        Log.Warning(exMessage);
    }

    public static void LogToFile(string exMessage)
    {
        Log.Information(exMessage);
    }
}