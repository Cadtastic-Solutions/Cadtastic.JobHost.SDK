using Microsoft.Extensions.DependencyInjection;

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
/// Base interface for all job types in the system.
/// This is the main registerable unit that the JobHost discovers and manages.
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
    /// Gets the description of the job.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets the version of the job.
    /// </summary>
    string Version { get; }

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

    /// <summary>
    /// Gets the job executor that handles the execution of this job.
    /// </summary>
    /// <returns>The job executor instance, or null if it should be resolved from services.</returns>
    IJobExecutor? GetExecutor();

    /// <summary>
    /// Gets the job executor that handles the execution of this job with service provider support.
    /// </summary>
    /// <param name="serviceProvider">The service provider for dependency injection.</param>
    /// <returns>The job executor instance, or null if it should be resolved from services.</returns>
    IJobExecutor? GetExecutor(IServiceProvider serviceProvider);

    /// <summary>
    /// Validates the job configuration.
    /// </summary>
    /// <returns>True if the job configuration is valid, false otherwise.</returns>
    bool ValidateConfiguration();

    /// <summary>
    /// Gets or sets the service provider used to resolve job-related services.
    /// </summary>
    IServiceProvider JobServiceProvidor { get; set; }

    /// <summary>
    /// Registers job-specific services with the service collection.
    /// This method is called when the job is loaded to register its dependencies.
    /// </summary>
    /// <param name="services">The service collection to register services with.</param>
    void RegisterJobServices(IServiceCollection services);
}
