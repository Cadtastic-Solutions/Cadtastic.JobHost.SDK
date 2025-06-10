using Cadtastic.JobHost.SDK.Interfaces;

namespace Cadtastic.JobHost.SDK.Models;

/// <summary>
/// Context provided to job executors during execution, containing all necessary information and services required for job processing.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="JobExecutionContext"/> class.
/// </remarks>
/// <param name="job">The job to be executed.</param>
/// <param name="serviceProvider">The service provider for dependency injection.</param>
/// <param name="progress">Optional progress reporter for execution updates.</param>
/// <param name="cancellationToken">Optional cancellation token for graceful shutdown.</param>
/// <exception cref="ArgumentNullException">Thrown when job or serviceProvider is null.</exception>
public class JobExecutionContext(
    IJob job,
    IServiceProvider serviceProvider,
    IProgress<string>? progress = null,
    CancellationToken cancellationToken = default) : IJobExecutionContext
{
    /// <summary>
    /// Gets the job being executed.
    /// </summary>
    public IJob Job { get; } = job ?? throw new ArgumentNullException(nameof(job));

    /// <summary>
    /// Gets the service provider for dependency injection and accessing registered services.
    /// </summary>
    public IServiceProvider ServiceProvider { get; } = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    /// <summary>
    /// Gets the progress reporter for job execution updates. May be null if progress reporting is not required.
    /// </summary>
    public IProgress<string>? Progress { get; } = progress;

    /// <summary>
    /// Gets the cancellation token to allow graceful job cancellation.
    /// </summary>
    public CancellationToken CancellationToken { get; } = cancellationToken;
}
