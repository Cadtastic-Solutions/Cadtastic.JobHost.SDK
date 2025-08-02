using Cadtastic.JobHost.SDK.Interfaces;

namespace Cadtastic.JobHost.SDK.Defaults;

/// <summary>
/// Default implementation of ITaskResult.
/// </summary>
public class DefaultTaskResult : ISettableTaskResult
{
    private readonly Dictionary<string, object> _data = new();

    /// <summary>
    /// Gets or sets the unique identifier of the task that produced this result.
    /// </summary>
    public string TaskId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the task execution was successful.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Gets or sets the error message if the task failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the exception if the task failed with an exception.
    /// </summary>
    public Exception? Exception { get; set; }

    /// <summary>
    /// Gets the data produced by the task execution.
    /// </summary>
    public IDictionary<string, object> Data => _data;

    /// <summary>
    /// Gets or sets the start time of the task execution.
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Gets or sets the end time of the task execution.
    /// </summary>
    public DateTime EndTime { get; set; }

    /// <summary>
    /// Gets the duration of the task execution.
    /// </summary>
    public TimeSpan Duration => EndTime - StartTime;

    /// <summary>
    /// Gets or sets a value indicating whether subsequent tasks should be blocked from executing.
    /// This is useful when a task completes successfully but determines that no further processing is needed
    /// (e.g., when Design Manager returns no files to archive, or when no files are found in Vault).
    /// </summary>
    public bool BlockSubsequentTasks { get; set; }

    /// <summary>
    /// Gets typed data from the result.
    /// </summary>
    /// <typeparam name="T">The type of data to retrieve.</typeparam>
    /// <param name="key">The key of the data.</param>
    /// <returns>The typed data if found and can be cast to T, default(T) otherwise.</returns>
    public T? GetData<T>(string key)
    {
        if (_data.TryGetValue(key, out var value) && value is T typedValue)
        {
            return typedValue;
        }
        return default;
    }

    /// <summary>
    /// Sets data in the result.
    /// </summary>
    /// <param name="key">The key for the data.</param>
    /// <param name="value">The value to store.</param>
    public void SetData(string key, object value)
    {
        _data[key] = value;
    }

    /// <summary>
    /// Creates a successful task result.
    /// </summary>
    /// <param name="taskId">The task ID.</param>
    /// <param name="data">Optional data to include.</param>
    /// <returns>A successful task result.</returns>
    public static DefaultTaskResult Success(string taskId, IDictionary<string, object>? data = null)
    {
        var result = new DefaultTaskResult
        {
            TaskId = taskId,
            IsSuccess = true,
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow
        };

        if (data != null)
        {
            foreach (var kvp in data)
            {
                result.SetData(kvp.Key, kvp.Value);
            }
        }

        return result;
    }

    /// <summary>
    /// Creates a failed task result.
    /// </summary>
    /// <param name="taskId">The task ID.</param>
    /// <param name="errorMessage">The error message.</param>
    /// <param name="exception">Optional exception.</param>
    /// <returns>A failed task result.</returns>
    public static DefaultTaskResult Failure(string taskId, string errorMessage, Exception? exception = null)
    {
        return new DefaultTaskResult
        {
            TaskId = taskId,
            IsSuccess = false,
            ErrorMessage = errorMessage,
            Exception = exception,
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow
        };
    }
}