namespace NovellusLib.Logging;

public enum LogLevel
{
    Debug,
    Info,
    Warn,
    Error,
    Fatal
}

public static class LogLevelExt
{
    public static string ToStringUpper(this LogLevel severity)
    {
        return severity switch
        {
            LogLevel.Debug => "DEBUG",
            LogLevel.Info => "INFO",
            LogLevel.Warn => "WARN",
            LogLevel.Error => "ERROR",
            LogLevel.Fatal => "FATAL",
            _ => "UNKNOWN"
        };
    }
}
