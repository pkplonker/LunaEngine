using System.Collections.Concurrent;

namespace Engine.Logging;

public static class Logger
{
	private static ConcurrentQueue<LogMessage> logQueue = new ConcurrentQueue<LogMessage>();
	private static List<ILogSink> sinks = new List<ILogSink>();
	private static bool isRunning;
	private static readonly Thread loggerThread;
	private const int threadInterval = 10;

	static Logger()
	{
		loggerThread = new Thread(new ThreadStart(ProcessLogs))
		{
			Name = "Logging thread",
			IsBackground = true,
		};
		loggerThread.Start();
	}

	public static void AddSink(ILogSink sink)
	{
		sinks.Add(sink);
	}

	private static void Log(LogLevel level, string message)
	{
		logQueue.Enqueue(new LogMessage {Level = level, Message = message, Timestamp = DateTime.UtcNow});
	}

	public static void Debug(string message)
	{
		Log(LogLevel.Debug, message);
	}

	public static void Info(string message)
	{
		Log(LogLevel.Info, message);
	}

	public static void Warning(string message)
	{
		Log(LogLevel.Warning, message);
	}

	public static void Error(string message)
	{
		Log(LogLevel.Error, message);
	}

	public static void Start()
	{
		isRunning = true;
	}

	public static void Stop()
	{
		isRunning = false;
	}

	private static void ProcessLogs()
	{
		while (isRunning)
		{
			while (logQueue.TryDequeue(out LogMessage message))
			{
				foreach (var sink in sinks)
				{
					sink.Emit(message);
				}
			}

			Thread.Sleep(threadInterval);
		}
	}
}