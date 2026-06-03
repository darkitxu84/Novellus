using Novellus.Lib.Backend.Logging.Formatters;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;

namespace Novellus.Lib.Backend.Logging;

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

    private const int LogsBufferSize = 30;
    private const int FlushIntervalMs = 350;
    private const int FileBufferSize = 4096;

    private readonly static List<LogEntry> _logsBuffer = new(LogsBufferSize);
    private readonly static PeriodicTimer _flushTimer = new(TimeSpan.FromMilliseconds(FlushIntervalMs));
    private readonly static StringBuilder _sb = new();

    private readonly static long _startTimestamp = Stopwatch.GetTimestamp();
    private readonly static DateTime _now = DateTime.Now;
    private static DateTime _cachedNow = DateTime.Now;
    private static long _lastCachedTicks = 0;
    private static DateTime CachedNow
    {
        get
        {
            long elapsed = Stopwatch.GetTimestamp();
            if (elapsed - _lastCachedTicks >= Stopwatch.Frequency)
            {
                _cachedNow = (_now + Stopwatch.GetElapsedTime(_startTimestamp)).ToLocalTime();
                _lastCachedTicks = elapsed;
            }
            return _cachedNow;
        }
    }

    private readonly static StreamWriter _fileLogWriter = new("novellus.log", append: false, bufferSize: FileBufferSize, encoding: Encoding.UTF8)
    {
        AutoFlush = false,
    };
    private static int _fileBufferUsed = 0;

    private readonly static Task _flushLogsTask;

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

    private static void FlushLogsBuffer()
    {
        // the gui will listen to this event and will receive the logs buffer to print them in a console window
        // consider pass a formatted string and the log level instead of the whole log entries
        OnFlush?.Invoke(_logsBuffer);
        _logsBuffer.Clear();

        Console.Write(_sb);
        _fileLogWriter.Write(_sb);
        _fileBufferUsed += _sb.Length;

        // flush to file if we have more than 4KB of logs buffered (disk sector size)
        if (_fileBufferUsed >= 4096)
        {
            _fileLogWriter.Flush();
            _fileBufferUsed = 0;
        }

        _sb.Clear();
    }

    private static async Task ProcessLogsAsync()
    {
        var readTask = _logsChannel.Reader.WaitToReadAsync().AsTask();
        var timerTask = _flushTimer.WaitForNextTickAsync().AsTask();

        while (true)
        {
            var completed = await Task.WhenAny(readTask, timerTask);

            if (completed == readTask)
            {
                if (!readTask.Result)
                    break;

                while (_logsChannel.Reader.TryRead(out LogEntry log))
                {
                    string formatedLog = _logFormatter.Format(log);
                    _sb.AppendLine(formatedLog);
                    _logsBuffer.Add(log);

                    if (_logsBuffer.Count >= LogsBufferSize)
                        FlushLogsBuffer();
                }
                readTask = _logsChannel.Reader.WaitToReadAsync().AsTask();
            }
            else
            {
                if (_logsBuffer.Count > 0)
                    FlushLogsBuffer();
                timerTask = _flushTimer.WaitForNextTickAsync().AsTask();
            }
        }

        if (_logsBuffer.Count > 0)
            FlushLogsBuffer();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    // public methods for different log levels
    [Conditional("DEBUG")]
    public static void Debug(string message,
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = "") =>
        EnqueueLog(LogLevel.Debug, message, line, member);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Info(string message,
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = "")
        => EnqueueLog(LogLevel.Info, message, line, member);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Warn(string message,
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = "")
        => EnqueueLog(LogLevel.Warn, message, line, member);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Error(string message,
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = "")
        => EnqueueLog(LogLevel.Error, message, line, member);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Fatal(string message,
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string member = "")
        => EnqueueLog(LogLevel.Fatal, message, line, member);

    // the gui or the application should call this method on exit
    public static void Shutdown()
    {
        _logsChannel.Writer.Complete();
        _flushLogsTask.Wait();

        if (_fileBufferUsed > 0)
            _fileLogWriter.Flush();

        _fileLogWriter.WriteLine($"----- Log ended at {CachedNow:G} -----");
        _flushTimer.Dispose();
        _fileLogWriter.Dispose();
    }

    // Event triggered when a log chunk is flushed
    // so we can hook into it from outside the library
    // for example in the GUI
    public static event Action<IReadOnlyList<LogEntry>>? OnFlush;
}
