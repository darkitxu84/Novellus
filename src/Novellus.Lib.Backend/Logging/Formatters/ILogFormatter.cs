namespace Novellus.Lib.Backend.Logging.Formatters;

internal interface ILogFormatter
{
    string Format(LogEntry entry);
}
