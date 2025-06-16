using Cadtastic.JobHost.SDK.Exceptions;

namespace Cadtastic.JobHost.SDK.Interfaces;

/// <summary>
/// Represents an executor that can execute a specific type of job.
/// </summary>
public interface IJobExecutor
{
    /// <summary>
    /// Gets the name of the executor.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the description of the executor.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets the version of the executor.
    /// </summary>
    string Version { get; }

    /// <summary>
    /// Gets the set of job types that this executor supports.
    /// </summary>
    HashSet<string> SupportedJobTypes { get; }

    /// <summary>
    /// Determines whether this executor can execute the specified job.
    /// </summary>
    /// <param name="job">The job to check.</param>
    /// <returns>True if this executor can execute the job, false otherwise.</returns>
    bool CanExecute(IJob job);

    /// <summary>
    /// Executes the specified job within the execution context.
    /// </summary>
    /// <param name="context">The execution context.</param>
    /// <returns>The result of the job execution.</returns>
    /// <exception cref="ArgumentNullException">Thrown when job or context is null.</exception>
    /// <exception cref="JobExecutionException">Thrown when an error occurs during job execution.</exception>
    Task<IJobExecutionResult> ExecuteAsync(IJobExecutionContext context);
}
