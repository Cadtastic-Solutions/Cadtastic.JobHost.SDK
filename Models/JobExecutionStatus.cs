namespace Cadtastic.JobHost.SDK.Models;

/// <summary>
/// Represents the current execution status of a job.
/// </summary>
public enum JobExecutionStatus
{
    /// <summary>
    /// The job is waiting to be executed.
    /// </summary>
    Waiting = 0,

    /// <summary>
    /// The job is currently running.
    /// </summary>
    Running = 1,

    /// <summary>
    /// The job has completed successfully.
    /// </summary>
    Completed = 2,

    /// <summary>
    /// The job execution was cancelled.
    /// </summary>
    Cancelled = 3,

    /// <summary>
    /// The job execution failed due to an error.
    /// </summary>
    Failed = 4
} 