using Cadtastic.JobHost.SDK.Interfaces;

using Microsoft.Extensions.Logging;

namespace Cadtastic.JobHost.SDK.Defaults;

/// <summary>
/// Default implementation of ITaskContext.
/// </summary>
public class DefaultTaskContext : ITaskContext
{
    private readonly IReadOnlyDictionary<string, ITaskResult> _taskResults;

    /// <summary>
    /// Gets the job context containing job-level information and shared data.
    /// </summary>
    public IJobContext JobContext { get; }

    /// <summary>
    /// Gets the unique identifier of the current task being executed.
    /// </summary>
    public string TaskId { get; }

    /// <summary>
    /// Gets a value indicating whether this task is critical.
    /// If a critical task fails, the entire job will fail.
    /// </summary>
    public bool IsCritical { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the current task execution was successful.
    /// This property is typically set by the task execution logic based on the task result.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether subsequent tasks should be blocked from executing.
    /// This is useful when a task completes successfully but determines that no further processing is needed
    /// (e.g., when Design Manager returns no files to archive, or when no files are found in Vault).
    /// </summary>
    public bool BlockSubsequentTasks { get; set; }

    /// <summary>
    /// Gets the results from previously executed tasks.
    /// </summary>
    public IReadOnlyDictionary<string, ITaskResult> PreviousTaskResults => _taskResults;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultTaskContext"/> class.
    /// </summary>
    /// <param name="jobContext">The job context.</param>
    /// <param name="taskId">The current task ID.</param>
    /// <param name="isCritical">Whether this task is critical to job success.</param>
    /// <param name="taskResults">The results from previously executed tasks.</param>
    public DefaultTaskContext(IJobContext jobContext, string taskId, bool isCritical, IReadOnlyDictionary<string, ITaskResult> taskResults)
    {
        JobContext = jobContext ?? throw new ArgumentNullException(nameof(jobContext));
        TaskId = taskId ?? throw new ArgumentNullException(nameof(taskId));
        IsCritical = isCritical;
        IsSuccess = true; // Default to true, will be updated based on task result
        BlockSubsequentTasks = false; // Default to false
        _taskResults = taskResults ?? new Dictionary<string, ITaskResult>();
    }

    /// <summary>
    /// Gets the result of a specific previous task by its ID.
    /// </summary>
    /// <param name="taskId">The ID of the task to get the result for.</param>
    /// <returns>The task result if found, null otherwise.</returns>
    public ITaskResult? GetPreviousTaskResult(string taskId)
    {
        return _taskResults.TryGetValue(taskId, out var result) ? result : null;
    }

    /// <summary>
    /// Gets typed data from a previous task result.
    /// </summary>
    /// <typeparam name="T">The type of data to retrieve.</typeparam>
    /// <param name="taskId">The ID of the task that produced the data.</param>
    /// <param name="key">The key of the data in the task result.</param>
    /// <returns>The typed data if found and can be cast to T, default(T) otherwise.</returns>
    public T? GetPreviousTaskData<T>(string taskId, string key)
    {
        var result = GetPreviousTaskResult(taskId);
        return result != null ? result.GetData<T>(key) : default;
    }

    /// <summary>
    /// Reports progress for the current task execution.
    /// </summary>
    /// <param name="percentage">The completion percentage (0-100).</param>
    /// <param name="message">Optional progress message.</param>
    /// <param name="messageLevel">Sets the log level of the message, so that the message can be displayed in the proper context (Debug, Warning, Error, Info, etc.)</param>
    public void ReportTaskProgress(int percentage, string? message = null, JobLogLevel messageLevel = JobLogLevel.Information)
    {
        var fullMessage = string.IsNullOrEmpty(message)
            ? $"Task '{TaskId}': {percentage}% complete"
            : $"Task '{TaskId}': {percentage}% - {message}";

        JobContext.Log(messageLevel, fullMessage);
    }
}