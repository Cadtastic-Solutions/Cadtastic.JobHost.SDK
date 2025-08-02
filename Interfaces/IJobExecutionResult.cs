namespace Cadtastic.JobHost.SDK.Interfaces;

/// <summary>
/// Represents the result of a complete job execution.
/// </summary>
public interface IJobExecutionResult
{
    /// <summary>
    /// Gets the unique identifier of the job execution.
    /// </summary>
    string ExecutionId { get; }

    /// <summary>
    /// Gets the job identifier.
    /// </summary>
    string JobId { get; }

    /// <summary>
    /// Gets the job type.
    /// </summary>
    string JobType { get; }

    /// <summary>
    /// Gets a value indicating whether the job execution was successful.
    /// </summary>
    bool IsSuccess { get; }

    /// <summary>
    /// Gets the overall status of the job execution.
    /// </summary>
    JobExecutionStatus Status { get; }

    /// <summary>
    /// Gets the error message if the job failed.
    /// </summary>
    string? ErrorMessage { get; }

    /// <summary>
    /// Gets the exception if the job failed with an exception.
    /// </summary>
    Exception? Exception { get; }

    /// <summary>
    /// Gets when the job execution started.
    /// </summary>
    DateTime StartTime { get; }

    /// <summary>
    /// Gets when the job execution ended.
    /// </summary>
    DateTime EndTime { get; }

    /// <summary>
    /// Gets the duration of the job execution.
    /// </summary>
    TimeSpan Duration { get; }

    /// <summary>
    /// Gets the collection of task results from this job execution.
    /// Key is the task ID, value is the task result.
    /// </summary>
    IReadOnlyDictionary<string, ITaskResult> TaskResults { get; }

    /// <summary>
    /// Gets the count of successful tasks.
    /// </summary>
    int SuccessfulTaskCount { get; }

    /// <summary>
    /// Gets the count of failed tasks.
    /// </summary>
    int FailedTaskCount { get; }

    /// <summary>
    /// Gets the total count of tasks executed.
    /// </summary>
    int TotalTaskCount { get; }

    /// <summary>
    /// Gets the execution history record for this job execution.
    /// </summary>
    IJobExecutionHistory ExecutionHistory { get; }
}
