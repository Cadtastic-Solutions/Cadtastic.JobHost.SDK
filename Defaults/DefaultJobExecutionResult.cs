using Cadtastic.JobHost.SDK.Interfaces;

namespace Cadtastic.JobHost.SDK.Defaults;

/// <summary>
/// Default implementation of IJobExecutionResult.
/// </summary>
public class DefaultJobExecutionResult : IJobExecutionResult
{
    private Dictionary<string, ITaskResult> _taskResults = new();

    /// <summary>
    /// Gets or sets the unique identifier of the job execution.
    /// </summary>
    public string ExecutionId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets the job identifier.
    /// </summary>
    public string JobId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the job type.
    /// </summary>
    public string JobType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the job execution was successful.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Gets or sets the overall status of the job execution.
    /// </summary>
    public JobExecutionStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the error message if the job failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the exception if the job failed with an exception.
    /// </summary>
    public Exception? Exception { get; set; }

    /// <summary>
    /// Gets or sets when the job execution started.
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Gets or sets when the job execution ended.
    /// </summary>
    public DateTime EndTime { get; set; }

    /// <summary>
    /// Gets the duration of the job execution.
    /// </summary>
    public TimeSpan Duration => EndTime - StartTime;

    /// <summary>
    /// Gets or sets the collection of task results from this job execution.
    /// </summary>
    public IReadOnlyDictionary<string, ITaskResult> TaskResults
    {
        get => _taskResults;
        set => _taskResults = value?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ?? new();
    }

    /// <summary>
    /// Gets the count of successful tasks.
    /// </summary>
    public int SuccessfulTaskCount => _taskResults.Count(kvp => kvp.Value.IsSuccess);

    /// <summary>
    /// Gets the count of failed tasks.
    /// </summary>
    public int FailedTaskCount => _taskResults.Count(kvp => !kvp.Value.IsSuccess);

    /// <summary>
    /// Gets the total count of tasks executed.
    /// </summary>
    public int TotalTaskCount => _taskResults.Count;

    /// <summary>
    /// Gets or sets the execution history record for this job execution.
    /// </summary>
    public IJobExecutionHistory ExecutionHistory { get; set; } = null!;
}