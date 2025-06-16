namespace Cadtastic.JobHost.SDK.Interfaces;

/// <summary>
/// Interface for tracking job execution history and statistics.
/// Provides methods to store and query historical job execution results.
/// </summary>
/// <summary>
/// Represents the execution history of a job.
/// </summary>
public interface IJobExecutionHistory
{
    /// <summary>
    /// Gets the unique identifier of the job.
    /// </summary>
    string JobId { get; }

    /// <summary>
    /// Gets the type of the job.
    /// </summary>
    string JobType { get; }

    /// <summary>
    /// Gets the name of the job.
    /// </summary>
    string JobName { get; }

    /// <summary>
    /// Gets or sets the list of execution results for the job.
    /// </summary>
    IList<IJobExecutionResult> Results { get; set; }

    /// <summary>
    /// Gets or sets the total number of executions.
    /// </summary>
    int TotalExecutions { get; set; }

    /// <summary>
    /// Gets or sets the total number of successful executions.
    /// </summary>
    int TotalSucceeded { get; set; }

    /// <summary>
    /// Gets or sets the total number of failed executions.
    /// </summary>
    int TotalFailed { get; set; }

    /// <summary>
    /// Gets the total number of successful executions.
    /// </summary>
    int SuccessCount { get; }

    /// <summary>
    /// Gets the total number of failed executions.
    /// </summary>
    int FailureCount { get; }

    /// <summary>
    /// Gets the total number of cancelled executions.
    /// </summary>
    int CancelledCount { get; }

    /// <summary>
    /// Gets the average duration of successful executions.
    /// </summary>
    TimeSpan AverageDuration { get; }

    /// <summary>
    /// Gets the last execution result.
    /// </summary>
    IJobExecutionResult? LastResult { get; }

    /// <summary>
    /// Gets the last successful execution result.
    /// </summary>
    IJobExecutionResult? LastSuccessfulResult { get; }

    /// <summary>
    /// Gets the last failed execution result.
    /// </summary>
    IJobExecutionResult? LastFailedResult { get; }
}
