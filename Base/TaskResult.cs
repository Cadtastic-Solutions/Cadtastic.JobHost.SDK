using Cadtastic.JobHost.SDK.Interfaces;

namespace Cadtastic.JobHost.SDK.Base;

/// <summary>
/// Base class for task results that provides common functionality.
/// </summary>
public class TaskResult : ITaskResult
{
    /// <summary>
    /// Gets whether the task execution was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets the error message if the task execution failed.
    /// </summary>
    public string? ErrorMessage { get; }

    /// <summary>
    /// Gets the exception that caused the task to fail, if any.
    /// </summary>
    public Exception? Exception { get; }

    /// <summary>
    /// Gets the data produced by the task execution.
    /// </summary>
    public object? Data { get; }

    /// <summary>
    /// Gets when the task execution started.
    /// </summary>
    public DateTime StartTime { get; }

    /// <summary>
    /// Gets when the task execution ended.
    /// </summary>
    public DateTime? EndTime { get; }

    /// <summary>
    /// Gets the duration of the task execution.
    /// </summary>
    public TimeSpan Duration => EndTime.HasValue ? EndTime.Value - StartTime : TimeSpan.Zero;

    /// <summary>
    /// Gets whether the task execution has completed (either successfully or with failure).
    /// </summary>
    public bool IsCompleted => EndTime.HasValue;

    /// <summary>
    /// Creates a new instance of the TaskResult class.
    /// </summary>
    /// <param name="isSuccess">Whether the task execution was successful.</param>
    /// <param name="data">The data produced by the task execution.</param>
    /// <param name="errorMessage">The error message if the task execution failed.</param>
    /// <param name="exception">The exception that caused the task to fail, if any.</param>
    /// <param name="startTime">When the task execution started.</param>
    /// <param name="endTime">When the task execution ended.</param>
    public TaskResult(bool isSuccess, object? data = null, string? errorMessage = null, Exception? exception = null, DateTime? startTime = null, DateTime? endTime = null)
    {
        IsSuccess = isSuccess;
        Data = data;
        ErrorMessage = errorMessage;
        Exception = exception;
        StartTime = startTime ?? DateTime.UtcNow;
        EndTime = endTime ?? (isSuccess || !string.IsNullOrEmpty(errorMessage) ? DateTime.UtcNow : null);
    }

    /// <summary>
    /// Creates a successful task result.
    /// </summary>
    /// <param name="data">The data produced by the task execution.</param>
    /// <param name="startTime">When the task execution started.</param>
    /// <param name="endTime">When the task execution ended.</param>
    /// <returns>A successful task result.</returns>
    public static TaskResult Success(object? data = null, DateTime? startTime = null, DateTime? endTime = null)
    {
        return new TaskResult(true, data, null, null, startTime, endTime);
    }

    /// <summary>
    /// Creates a failed task result.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    /// <param name="exception">The exception that caused the task to fail, if any.</param>
    /// <param name="startTime">When the task execution started.</param>
    /// <param name="endTime">When the task execution ended.</param>
    /// <returns>A failed task result.</returns>
    public static TaskResult Failure(string errorMessage, Exception? exception = null, DateTime? startTime = null, DateTime? endTime = null)
    {
        return new TaskResult(false, null, errorMessage, exception, startTime, endTime);
    }
} 