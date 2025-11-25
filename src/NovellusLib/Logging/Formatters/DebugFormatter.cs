namespace NovellusLib.Logging.Formatters;

internal class DebugFormatter : ILogFormatter
{
    public string Format(LogEntry entry)
    {
        return $"[{entry.Timestamp:HH:mm:ss}][{entry.MemberName}-{entry.LineNumber}][{entry.Level.ToStringUpper()}] {entry.Message}";
    }
}
