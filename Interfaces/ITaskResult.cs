namespace Cadtastic.JobHost.SDK.Interfaces;

/// <summary>
/// Represents the result of a task execution.
/// </summary>
public interface ITaskResult
{
    /// <summary>
    /// Gets the unique identifier of the task that produced this result.
    /// </summary>
    string TaskId { get; }

    /// <summary>
    /// Gets a value indicating whether the task execution was successful.
    /// </summary>
    bool IsSuccess { get; }

    /// <summary>
    /// Gets the error message if the task failed.
    /// </summary>
    string? ErrorMessage { get; }

    /// <summary>
    /// Gets the exception if the task failed with an exception.
    /// </summary>
    Exception? Exception { get; }

    /// <summary>
    /// Gets the data produced by the task execution.
    /// This data can be accessed by subsequent tasks.
    /// </summary>
    IDictionary<string, object> Data { get; }

    /// <summary>
    /// Gets the start time of the task execution.
    /// </summary>
    DateTime StartTime { get; }

    /// <summary>
    /// Gets the end time of the task execution.
    /// </summary>
    DateTime EndTime { get; }

    /// <summary>
    /// Gets the duration of the task execution.
    /// </summary>
    TimeSpan Duration { get; }

    /// <summary>
    /// Gets or sets a value indicating whether subsequent tasks should be blocked from executing.
    /// This is useful when a task completes successfully but determines that no further processing is needed
    /// (e.g., when Design Manager returns no files to archive, or when no files are found in Vault).
    /// </summary>
    bool BlockSubsequentTasks { get; set; }

    /// <summary>
    /// Gets typed data from the result.
    /// </summary>
    /// <typeparam name="T">The type of data to retrieve.</typeparam>
    /// <param name="key">The key of the data.</param>
    /// <returns>The typed data if found and can be cast to T, default(T) otherwise.</returns>
    T? GetData<T>(string key);

    /// <summary>
    /// Sets data in the result.
    /// </summary>
    /// <param name="key">The key for the data.</param>
    /// <param name="value">The value to store.</param>
    void SetData(string key, object value);
}