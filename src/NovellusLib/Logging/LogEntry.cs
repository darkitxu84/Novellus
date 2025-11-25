namespace NovellusLib.Logging;

public record LogEntry
{
    public LogLevel Level { get; init; }
    public string? Message { get; init; }
    public int LineNumber { get; init; }
    public string? MemberName { get; init; }
    public DateTime Timestamp { get; init; }
}
