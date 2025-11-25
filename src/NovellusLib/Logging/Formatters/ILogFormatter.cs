namespace NovellusLib.Logging.Formatters;

interface ILogFormatter
{
    string Format(LogEntry entry);
}
