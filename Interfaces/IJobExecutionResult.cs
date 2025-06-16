namespace Cadtastic.JobHost.SDK.Interfaces;

/// <summary>
/// Represents the result state of a job execution.
/// </summary>
public enum ResultState
{
    /// <summary>
    /// The job execution failed due to an error.
    /// </summary>
    Failed = 0,

    /// <summary>
    /// The job execution completed successfully.
    /// </summary>
    Successful = 1,

    /// <summary>
    /// The job execution was cancelled before completion.
    /// </summary>
    Cancelled = 2,

    /// <summary>
    /// The job execution result state is unknown or cannot be determined.
    /// </summary>
    Unknown = 4
}

/// <summary>
/// Result of job execution
/// </summary>
public interface IJobExecutionResult
{
    /// <summary>
    /// Whether the job execution was successful
    /// </summary>
    bool IsSuccess { get; }

    /// <summary>
    /// The specific result state of the job execution
    /// </summary>
    ResultState State { get; }

    /// <summary>
    /// Error message if execution failed
    /// </summary>
    string? ErrorMessage { get; }

    /// <summary>
    /// Additional details about the execution
    /// </summary>
    string? Details { get; }

    /// <summary>
    /// When the execution started
    /// </summary>
    DateTime StartTime { get; }

    /// <summary>
    /// When the execution ended
    /// </summary>
    DateTime? EndTime { get; }

    /// <summary>
    /// Duration of the execution
    /// </summary>
    TimeSpan? Duration => (EndTime - StartTime);
}
