namespace Cadtastic.JobHost.SDK.Interfaces;

/// <summary>
/// Represents the result of a task execution.
/// </summary>
public interface ITaskResult
{
    /// <summary>
    /// Gets whether the task execution was successful.
    /// </summary>
    bool IsSuccess { get; }

    /// <summary>
    /// Gets the error message if the task execution failed.
    /// </summary>
    string? ErrorMessage { get; }

    /// <summary>
    /// Gets the exception that caused the task to fail, if any.
    /// </summary>
    Exception? Exception { get; }

    /// <summary>
    /// Gets the data produced by the task execution.
    /// </summary>
    object? Data { get; }

    /// <summary>
    /// Gets when the task execution started.
    /// </summary>
    DateTime StartTime { get; }

    /// <summary>
    /// Gets when the task execution ended.
    /// </summary>
    DateTime? EndTime { get; }

    /// <summary>
    /// Gets the duration of the task execution.
    /// </summary>
    TimeSpan Duration { get; }

    /// <summary>
    /// Gets whether the task execution has completed (either successfully or with failure).
    /// </summary>
    bool IsCompleted { get; }
} 