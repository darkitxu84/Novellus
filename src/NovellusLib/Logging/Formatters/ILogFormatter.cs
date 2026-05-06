namespace NovellusLib.Logging.Formatters;

internal interface ILogFormatter
{
    string Format(LogEntry entry);
}
