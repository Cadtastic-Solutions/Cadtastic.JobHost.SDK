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
    /// Unique identifier for the job
    /// </summary>
    string Id { get; set; }

    /// <summary>
    /// Display name of the job
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Type of job (e.g., "Archival", "Backup", "Purge")
    /// </summary>
    string JobType { get; set; }

    /// <summary>
    /// Current execution state of the job
    /// </summary>
    JobState State { get; set; }

    /// <summary>
    /// Configuration settings specific to the job type
    /// </summary>
    object Configuration { get; set; }

    /// <summary>
    /// Gets or sets the execution history for a job.
    /// </summary>
    IJobExecutionHistory ExecutionHistory { get; set; }
}

/// <summary>
/// Extension methods for <see cref="IJob"/> to provide convenient access
/// to job state management and validation.
/// </summary>
public static class JobExtensions
{
    /// <summary>
    /// Checks if the job is currently in a state that allows execution.
    /// </summary>
    /// <param name="job">The job to check.</param>
    /// <returns>True if the job can be executed, false otherwise.</returns>
    /// <exception cref="ArgumentNullException">Thrown when job is null.</exception>
    public static bool CanBeExecuted(this IJob job)
    {
        ArgumentNullException.ThrowIfNull(job);
        return job.State == JobState.Active;
    }

    /// <summary>
    /// Checks if the job is currently being processed.
    /// </summary>
    /// <param name="job">The job to check.</param>
    /// <returns>True if the job is currently processing, false otherwise.</returns>
    /// <exception cref="ArgumentNullException">Thrown when job is null.</exception>
    public static bool IsProcessing(this IJob job)
    {
        ArgumentNullException.ThrowIfNull(job);
        return job.State == JobState.Processing;
    }

    /// <summary>
    /// Sets the job state to Processing and returns the previous state.
    /// </summary>
    /// <param name="job">The job to update.</param>
    /// <returns>The previous job state.</returns>
    /// <exception cref="ArgumentNullException">Thrown when job is null.</exception>
    public static JobState SetToProcessing(this IJob job)
    {
        ArgumentNullException.ThrowIfNull(job);
        var previousState = job.State;
        job.State = JobState.Processing;
        return previousState;
    }

    /// <summary>
    /// Sets the job state to Active.
    /// </summary>
    /// <param name="job">The job to update.</param>
    /// <exception cref="ArgumentNullException">Thrown when job is null.</exception>
    public static void SetToActive(this IJob job)
    {
        ArgumentNullException.ThrowIfNull(job);
        job.State = JobState.Active;
    }

    /// <summary>
    /// Sets the job state to Inactive.
    /// </summary>
    /// <param name="job">The job to update.</param>
    /// <exception cref="ArgumentNullException">Thrown when job is null.</exception>
    public static void SetToInactive(this IJob job)
    {
        ArgumentNullException.ThrowIfNull(job);
        job.State = JobState.Inactive;
    }

    /// <summary>
    /// Gets a human-readable description of the current job state.
    /// </summary>
    /// <param name="job">The job to get the state description for.</param>
    /// <returns>A descriptive string for the current job state.</returns>
    /// <exception cref="ArgumentNullException">Thrown when job is null.</exception>
    public static string GetStateDescription(this IJob job)
    {
        ArgumentNullException.ThrowIfNull(job);
        return job.State switch
        {
            JobState.Inactive => "The job is not currently active",
            JobState.Active => "The job is ready for execution",
            JobState.Processing => "The job is currently being executed",
            JobState.Unknown => "The job state cannot be determined",
            _ => "Invalid job state"
        };
    }
} 