namespace Cadtastic.JobHost.SDK.Models;

/// <summary>
/// Represents the status of a task execution.
/// </summary>
public enum TaskStatus
{
    /// <summary>
    /// The task is pending execution.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// The task is currently running.
    /// </summary>
    Running = 1,

    /// <summary>
    /// The task has completed successfully.
    /// </summary>
    Completed = 2,

    /// <summary>
    /// The task execution failed.
    /// </summary>
    Failed = 3,

    /// <summary>
    /// The task execution was cancelled.
    /// </summary>
    Cancelled = 4,

    /// <summary>
    /// The task execution was skipped.
    /// </summary>
    Skipped = 5
} 