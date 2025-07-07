namespace Cadtastic.JobHost.SDK.Models;

/// <summary>
/// Represents the status of a job in the system.
/// </summary>
public enum JobStatus
{
    /// <summary>
    /// The job is pending execution.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// The job is waiting to be executed.
    /// </summary>
    Waiting = 1,

    /// <summary>
    /// The job is currently running.
    /// </summary>
    Running = 2,

    /// <summary>
    /// The job has completed successfully.
    /// </summary>
    Completed = 3,

    /// <summary>
    /// The job execution failed.
    /// </summary>
    Failed = 4,

    /// <summary>
    /// The job execution was cancelled.
    /// </summary>
    Cancelled = 5,

    /// <summary>
    /// The job failed validation.
    /// </summary>
    ValidationFailed = 6
} 