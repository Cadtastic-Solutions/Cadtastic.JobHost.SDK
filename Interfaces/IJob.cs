namespace Cadtastic.JobHost.SDK.Interfaces;

/// <summary>
/// Represents the current execution state of a job.
/// </summary>
public enum JobState
{
    /// <summary>
    /// The job is not currently active and is not running.
    /// </summary>
    Inactive = 0,

    /// <summary>
    /// The job is active and available for execution.
    /// </summary>
    Active = 1,

    /// <summary>
    /// The job is currently being processed or executed.
    /// </summary>
    Processing = 2,

    /// <summary>
    /// The job state is unknown or cannot be determined.
    /// </summary>
    Unknown = 4
}

/// <summary>
/// Base interface for all job types in the system
/// </summary>
public interface IJob
{
    /// <summary>
    /// Gets the unique identifier of the job.
    /// </summary>
    string Id { get; set; }

    /// <summary>
    /// Gets the name of the job.
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Gets the type of the job.
    /// </summary>
    string JobType { get; set; }

    /// <summary>
    /// Gets the current state of the job.
    /// </summary>
    JobState State { get; set; }

    /// <summary>
    /// Gets the configuration data for the job.
    /// </summary>
    object Configuration { get; set; }

    /// <summary>
    /// Gets or sets the execution history for a job.
    /// </summary>
    IJobExecutionHistory ExecutionHistory { get; set; }
}
