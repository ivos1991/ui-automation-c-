using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using WikiAutomation.Framework.Serialization;

namespace WikiAutomation.Framework.Logging;

/// <summary>
/// Provides test-scoped logging that writes timestamped lines to the standard test output stream.
/// </summary>
public static class FrameworkLogger
{
    private static readonly AsyncLocal<TestLogContext?> CurrentContext = new();
    private static int _maxMessageLength = 10_000;

    /// <summary>
    /// Configures the maximum message size allowed before a log entry is truncated.
    /// </summary>
    /// <param name="maxMessageLength">The maximum size for a single rendered log message.</param>
    public static void Configure(int maxMessageLength)
    {
        _maxMessageLength = Math.Max(256, maxMessageLength);
    }

    /// <summary>
    /// Starts a new test logging scope so subsequent log entries include test metadata and are buffered.
    /// </summary>
    /// <param name="testName">The current NUnit test name.</param>
    public static void StartTest(string testName)
    {
        CurrentContext.Value = new TestLogContext(
            testName,
            "running",
            DateTimeOffset.UtcNow,
            new StringBuilder());
    }

    /// <summary>
    /// Updates the current test status without resetting the buffered log output.
    /// </summary>
    /// <param name="status">The latest test status.</param>
    public static void UpdateTestStatus(string status)
    {
        if (CurrentContext.Value is null)
        {
            return;
        }

        CurrentContext.Value.Status = status;
    }

    /// <summary>
    /// Returns the runtime of the current test logging scope when one is active.
    /// </summary>
    /// <returns>The current test runtime, or <see langword="null"/> when no test scope exists.</returns>
    public static TimeSpan? GetCurrentTestRuntime()
    {
        return CurrentContext.Value is null
            ? null
            : DateTimeOffset.UtcNow - CurrentContext.Value.StartedAt;
    }

    /// <summary>
    /// <summary>
    /// Clears the current test logging scope after teardown finishes.
    /// </summary>
    public static void ClearCurrentTest()
    {
        CurrentContext.Value = null;
    }

    /// <summary>
    /// Writes a timestamped informational message to the console and the current test buffer.
    /// </summary>
    /// <param name="message">The message to render.</param>
    /// <param name="source">Optional logical logger name.</param>
    /// <param name="memberName">The calling member name supplied by the compiler.</param>
    /// <param name="filePath">The calling file path supplied by the compiler.</param>
    public static void Info(
        string message,
        string? source = null,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "")
    {
        Write("INFO", message, source, memberName, filePath);
    }

    /// <summary>
    /// Writes a timestamped warning message to the console and the current test buffer.
    /// </summary>
    /// <param name="message">The message to render.</param>
    /// <param name="source">Optional logical logger name.</param>
    /// <param name="memberName">The calling member name supplied by the compiler.</param>
    /// <param name="filePath">The calling file path supplied by the compiler.</param>
    public static void Warn(
        string message,
        string? source = null,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "")
    {
        Write("WARN", message, source, memberName, filePath);
    }

    /// <summary>
    /// Writes a timestamped error message to the console and the current test buffer.
    /// </summary>
    /// <param name="message">The message to render.</param>
    /// <param name="source">Optional logical logger name.</param>
    /// <param name="memberName">The calling member name supplied by the compiler.</param>
    /// <param name="filePath">The calling file path supplied by the compiler.</param>
    public static void Error(
        string message,
        string? source = null,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "")
    {
        Write("ERROR", message, source, memberName, filePath);
    }

    /// <summary>
    /// Serializes a value into formatted JSON and logs it as a structured payload.
    /// </summary>
    /// <param name="label">A short label describing the payload.</param>
    /// <param name="value">The value to serialize.</param>
    /// <param name="source">Optional logical logger name.</param>
    /// <param name="memberName">The calling member name supplied by the compiler.</param>
    /// <param name="filePath">The calling file path supplied by the compiler.</param>
    public static void Json(
        string label,
        object value,
        string? source = null,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "")
    {
        var payload = JsonDefaults.SerializeIndented(value);
        Write("JSON", $"{label}{Environment.NewLine}{payload}", source, memberName, filePath);
    }

    private static void Write(
        string level,
        string message,
        string? source,
        string memberName,
        string filePath)
    {
        var renderedSource = ResolveSource(source, memberName, filePath);
        var renderedMessage = Truncate(message);
        var renderedLine = FormatLine(level, renderedSource, renderedMessage);

        Console.WriteLine(renderedLine);

        if (CurrentContext.Value is not null)
        {
            CurrentContext.Value.Buffer.AppendLine(renderedLine);
        }
    }

    private static string FormatLine(string level, string source, string message)
    {
        var context = CurrentContext.Value;
        var testName = context?.TestName ?? "-";
        var status = context?.Status ?? "-";

        return string.Create(
            CultureInfo.InvariantCulture,
            $"[{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {source} [test={testName} status={status}] - {message}");
    }

    private static string ResolveSource(string? source, string memberName, string filePath)
    {
        if (!string.IsNullOrWhiteSpace(source))
        {
            return source.Trim();
        }

        var fileName = Path.GetFileNameWithoutExtension(filePath);
        return string.IsNullOrWhiteSpace(fileName) ? memberName : $"{fileName}.{memberName}";
    }

    private static string Truncate(string message)
    {
        if (message.Length <= _maxMessageLength)
        {
            return message;
        }

        return $"{message[.._maxMessageLength]} [TRUNCATED]";
    }

    private sealed class TestLogContext(
        string testName,
        string status,
        DateTimeOffset startedAt,
        StringBuilder buffer)
    {
        public string TestName { get; } = testName;

        public string Status { get; set; } = status;

        public DateTimeOffset StartedAt { get; } = startedAt;

        public StringBuilder Buffer { get; } = buffer;
    }
}
