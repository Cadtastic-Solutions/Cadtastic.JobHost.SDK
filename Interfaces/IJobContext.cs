using System.Collections.Concurrent;

namespace Cadtastic.JobHost.SDK.Interfaces;

/// <summary>
/// Represents the context for job execution that is passed to the job executor and tasks.
/// Provides access to job configuration, shared data, and execution control.
/// </summary>
public interface IJobContext
{
    /// <summary>
    /// Gets the unique identifier of the job being executed.
    /// </summary>
    string JobId { get; }

    /// <summary>
    /// Gets the name of the job being executed.
    /// </summary>
    string JobName { get; }

    /// <summary>
    /// Gets the type of the job being executed.
    /// </summary>
    string JobType { get; }

    /// <summary>
    /// Gets the job configuration.
    /// </summary>
    object Configuration { get; }

    /// <summary>
    /// Gets the cancellation token for the job execution.
    /// All tasks should honor this cancellation token.
    /// </summary>
    CancellationToken CancellationToken { get; }

    /// <summary>
    /// Gets the shared data dictionary for passing data between tasks.
    /// This is thread-safe and can be used by concurrent tasks.
    /// </summary>
    ConcurrentDictionary<string, object> SharedData { get; }

    /// <summary>
    /// Reports progress for the overall job execution.
    /// </summary>
    /// <param name="percentage">The completion percentage (0-100).</param>
    /// <param name="message">Optional progress message.</param>
    void ReportProgress(int percentage, string? message = null);

    /// <summary>
    /// Logs a message to the job execution log.
    /// </summary>
    /// <param name="level">The log level.</param>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">Optional exception to log.</param>
    void Log(JobLogLevel level, string message, Exception? exception = null);
}

/// <summary>
/// Defines the log levels for job execution logging.
/// </summary>
public enum JobLogLevel
{
    /// <summary>
    /// Trace level logging for detailed debugging.
    /// </summary>
    Trace = 0,

    /// <summary>
    /// Debug level logging.
    /// </summary>
    Debug = 1,

    /// <summary>
    /// Information level logging.
    /// </summary>
    Information = 2,

    /// <summary>
    /// Warning level logging.
    /// </summary>
    Warning = 3,

    /// <summary>
    /// Error level logging.
    /// </summary>
    Error = 4,

    /// <summary>
    /// Critical error level logging.
    /// </summary>
    Critical = 5
}