namespace Cadtastic.JobHost.SDK.Interfaces;

/// <summary>
/// Represents the execution history of a job, including all task results.
/// </summary>
public interface IJobExecutionHistory
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
    /// Gets the job name.
    /// </summary>
    string JobName { get; }

    /// <summary>
    /// Gets the job type.
    /// </summary>
    string JobType { get; }

    /// <summary>
    /// Gets when the job execution started.
    /// </summary>
    DateTime StartTime { get; }

    /// <summary>
    /// Gets when the job execution ended.
    /// Null if the job is still running.
    /// </summary>
    DateTime? EndTime { get; }

    /// <summary>
    /// Gets the duration of the job execution.
    /// Null if the job is still running.
    /// </summary>
    TimeSpan? Duration { get; }

    /// <summary>
    /// Gets the overall status of the job execution.
    /// </summary>
    JobExecutionStatus Status { get; }

    /// <summary>
    /// Gets the error message if the job failed.
    /// </summary>
    string? ErrorMessage { get; }

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
    /// Adds a task result to the execution history.
    /// </summary>
    /// <param name="taskResult">The task result to add.</param>
    void AddTaskResult(ITaskResult taskResult);

    /// <summary>
    /// Marks the job execution as completed.
    /// </summary>
    /// <param name="status">The final status of the job.</param>
    /// <param name="errorMessage">Optional error message if the job failed.</param>
    void Complete(JobExecutionStatus status, string? errorMessage = null);
}

/// <summary>
/// Represents the status of a job execution.
/// </summary>
public enum JobExecutionStatus
{
    /// <summary>
    /// The job is currently running.
    /// </summary>
    Running = 0,

    /// <summary>
    /// The job completed successfully.
    /// </summary>
    Success = 1,

    /// <summary>
    /// The job failed to complete.
    /// </summary>
    Failed = 2,

    /// <summary>
    /// The job was cancelled.
    /// </summary>
    Cancelled = 3,

    /// <summary>
    /// The job completed with warnings.
    /// </summary>
    CompletedWithWarnings = 4
}
