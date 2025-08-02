namespace Cadtastic.JobHost.SDK.Interfaces;

/// <summary>
/// Represents the context for task execution that is passed to each task executor.
/// Extends the job context with task-specific information and previous task results.
/// </summary>
public interface ITaskContext
{
    /// <summary>
    /// Gets the job context containing job-level information and shared data.
    /// </summary>
    IJobContext JobContext { get; }

    /// <summary>
    /// Gets the unique identifier of the current task being executed.
    /// </summary>
    string TaskId { get; }

    /// <summary>
    /// Gets a value indicating whether this task is critical.
    /// If a critical task fails, the entire job will fail.
    /// </summary>
    bool IsCritical { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the current task execution was successful.
    /// This property is typically set by the task execution logic based on the task result.
    /// </summary>
    bool IsSuccess { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether subsequent tasks should be blocked from executing.
    /// This is useful when a task completes successfully but determines that no further processing is needed
    /// (e.g., when Design Manager returns no files to archive, or when no files are found in Vault).
    /// </summary>
    bool BlockSubsequentTasks { get; set; }

    /// <summary>
    /// Gets the results from previously executed tasks.
    /// Key is the task ID, value is the task result.
    /// </summary>
    IReadOnlyDictionary<string, ITaskResult> PreviousTaskResults { get; }

    /// <summary>
    /// Gets the result of a specific previous task by its ID.
    /// </summary>
    /// <param name="taskId">The ID of the task to get the result for.</param>
    /// <returns>The task result if found, null otherwise.</returns>
    ITaskResult? GetPreviousTaskResult(string taskId);

    /// <summary>
    /// Gets typed data from a previous task result.
    /// </summary>
    /// <typeparam name="T">The type of data to retrieve.</typeparam>
    /// <param name="taskId">The ID of the task that produced the data.</param>
    /// <param name="key">The key of the data in the task result.</param>
    /// <returns>The typed data if found and can be cast to T, default(T) otherwise.</returns>
    T? GetPreviousTaskData<T>(string taskId, string key);

    /// <summary>
    /// Reports progress for the current task execution.
    /// </summary>
    /// <param name="percentage">The completion percentage (0-100).</param>
    /// <param name="message">Optional progress message.</param>
    /// <param name="messageLevel">Sets the log level of the message, so that the message can be displayed in the proper context (Debug, Warning, Error, Info, etc.)</param>
    void ReportTaskProgress(int percentage, string? message = null, JobLogLevel messageLevel = JobLogLevel.Information);
}