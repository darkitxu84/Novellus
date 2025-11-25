using NovellusLib.Logging.Formatters;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;

namespace NovellusLib.Logging;

/// <summary>
/// 
/// </summary>
public static class Logger
{
    private readonly static Channel<LogEntry> _logsChannel;

#if DEBUG
    private readonly static DebugFormatter _logFormatter = new();
#else
    private readonly static ReleaseFormatter _logFormatter = new();
#endif

    private const int LogsBufferSize = 10;
    private const int FlushIntervalMs = 300;

    private readonly static List<LogEntry> _logsBuffer = new(LogsBufferSize);
    private readonly static PeriodicTimer _flushTimer = new(TimeSpan.FromMilliseconds(FlushIntervalMs));
    private readonly static StringBuilder _sb = new();

    private readonly static Stopwatch _sw = Stopwatch.StartNew();
    private readonly static DateTime _now = DateTime.Now;
    private static DateTime CachedNow => (_now + _sw.Elapsed).ToLocalTime();

    private readonly static StreamWriter _fileLogWriter = new("novellus.log", append: false)
    {
        AutoFlush = true
    };
    private readonly static StringBuilder _fileLogSb = new();

    private readonly static Task _flushLogsTask;

    private static void FileFlushLogsBuffer()
    {
        _fileLogWriter.Write(_fileLogSb.ToString());
        _fileLogSb.Clear();
    }

    private static void FlushLogsBuffer()
    {
        // the gui will listen to this event and will receive the logs buffer to print them in a console window
        // consider pass a formatted string and the log level instead of the whole log entries
        OnFlush?.Invoke(_logsBuffer);
        _logsBuffer.Clear();

        var sbString = _sb.ToString();
        Console.Write(sbString);
        _fileLogSb.Append(sbString);

        // flush to file if we have more than 4KB of logs buffered
        if (_fileLogSb.Length >= 4096)
            FileFlushLogsBuffer();

        _sb.Clear();
    }

    private static async Task ProcessLogsAsync()
    {
        while (await _logsChannel.Reader.WaitToReadAsync())
        {
            while (_logsChannel.Reader.TryRead(out LogEntry? log))
            {
                // reduce system calls by buffering logs with StringBuilder
                string formatedLog = _logFormatter.Format(log);
                _sb.AppendLine(formatedLog);
                _logsBuffer.Add(log);

                if (_logsBuffer.Count >= LogsBufferSize)
                    FlushLogsBuffer();
            }

            if (_logsBuffer.Count > 0 && await _flushTimer.WaitForNextTickAsync())
                FlushLogsBuffer();
        }
    }

    private static void EnqueueLog(LogLevel level, string message, int line, string member)
    {
        var entry = new LogEntry
        {
            Level = level,
            Message = message,
            LineNumber = line,
            MemberName = member,
            Timestamp = CachedNow
        };

        _logsChannel.Writer.TryWrite(entry);
    }

    static Logger()
    {
        var options = new UnboundedChannelOptions()
        {
            SingleReader = true,
            SingleWriter = false
        };
        _logsChannel = Channel.CreateUnbounded<LogEntry>(options);

        _fileLogWriter.WriteLine($"----- Log started at {_now:G} -----");

        _flushLogsTask = Task.Run(ProcessLogsAsync);
    }

    // public methods for different log levels
    public static void Debug(string message,
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = "")
        => EnqueueLog(LogLevel.Debug, message, line, member);

    public static void Info(string message,
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = "")
        => EnqueueLog(LogLevel.Info, message, line, member);

    public static void Warn(string message,
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = "")
        => EnqueueLog(LogLevel.Warn, message, line, member);

    public static void Error(string message,
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = "")
        => EnqueueLog(LogLevel.Error, message, line, member);

    public static void Fatal(string message,
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = "")
        => EnqueueLog(LogLevel.Fatal, message, line, member);

    // the gui or the application should call this method on exit
    public static void Shutdown()
    {
        _logsChannel.Writer.Complete();
        _flushLogsTask.Wait();
        // flush any remaining logs to file
        if (_fileLogSb.Length > 0)
            FileFlushLogsBuffer();
        _fileLogWriter.Write(_sb.ToString());
        _fileLogWriter.WriteLine($"----- Log ended at {CachedNow:G} -----");
        _fileLogWriter.Dispose();
    }

    // Event triggered when a log chunk is flushed
    // so we can hook into it from outside the library
    // for example in the GUI
    public static event Action<List<LogEntry>>? OnFlush;
}
