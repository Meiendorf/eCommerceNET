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

    private static void LogToDebugger(string exMessage)
    {
        Log.Debug(exMessage);
    }

    private static void LogToConsole(string exMessage)
    {
        Log.Warning(exMessage);
    }

    private static void LogToFile(string exMessage)
    {
        Log.Information(exMessage);
    }
}