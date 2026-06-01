namespace Novellus.Lib.Backend.Logging.Formatters;

public sealed class DebugFormatter : ILogFormatter
{
    public string Format(LogEntry entry)
    {
        return $"[{entry.MemberName}-{entry.LineNumber}][{entry.Level.ToStringUpper()}] {entry.Message}";
    }
}
