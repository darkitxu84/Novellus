namespace NovellusLib.Logging.Formatters;

public class ReleaseFormatter : ILogFormatter
{
    public string Format(LogEntry entry)
    {
        return $"[{entry.Timestamp:HH:mm:ss}][{entry.Level.ToStringUpper()}] {entry.Message}";
    }
}
