namespace NovellusLib.Logging.Formatters;

public sealed class DebugFormatter : ILogFormatter
{
    public string Format(LogEntry entry)
    {
        return $"[{entry.Timestamp:HH:mm:ss}][{entry.MemberName}-{entry.LineNumber}][{entry.Level.ToStringUpper()}] {entry.Message}";
    }
}
