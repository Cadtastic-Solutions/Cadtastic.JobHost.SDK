using Cadtastic.JobHost.SDK.Interfaces;

namespace Cadtastic.JobHost.SDK.Models;

/// <summary>
/// Provides context and state information for a job during its execution.
/// This class contains all the necessary information and utilities that a job needs to perform its work.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="JobExecutionContext"/> class.
/// </remarks>
/// <param name="job">The job being executed.</param>
/// <param name="history">The execution history of the job.</param>
/// <param name="logger">The logger for the job execution.</param>
/// <param name="serviceProvider">The service provider for dependency injection.</param>
/// <param name="progress">Optional progress reporter for execution updates.</param>
/// <param name="data">The data associated with the job execution.</param>
/// <param name="cancellationToken">The cancellation token for the job execution.</param>
/// <exception cref="ArgumentNullException">Thrown when job, history, logger, or serviceProvider is null.</exception>
public class JobExecutionContext(
    IJob job,
    IJobExecutionHistory history,
    ILogger logger,
    IServiceProvider serviceProvider,
    IProgress<string>? progress = null,
    IDictionary<string, object>? data = null,
    CancellationToken cancellationToken = default) : IJobExecutionContext
{
    /// <summary>
    /// Gets the job being executed.
    /// </summary>
    public IJob Job { get; } = job ?? throw new ArgumentNullException(nameof(job));

    /// <summary>
    /// Gets the execution history of the job.
    /// </summary>
    public IJobExecutionHistory History { get; } = history ?? throw new ArgumentNullException(nameof(history));

    /// <summary>
    /// Gets the data associated with the job execution.
    /// </summary>
    public IDictionary<string, object> Data { get; } = data ?? new Dictionary<string, object>();

    /// <summary>
    /// Gets the cancellation token for the job execution.
    /// </summary>
    public CancellationToken CancellationToken { get; } = cancellationToken;

    /// <summary>
    /// Gets the logger for the job execution.
    /// </summary>
    public ILogger Logger { get; } = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Gets the service provider for dependency injection.
    /// </summary>
    public IServiceProvider ServiceProvider { get; } = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    /// <summary>
    /// Gets the progress reporter for the job execution.
    /// </summary>
    public IProgress<string>? Progress { get; } = progress;

    /// <summary>
    /// Gets the current status of the job execution.
    /// </summary>
    public JobStatus Status { get; private set; } = JobStatus.Pending;

    /// <summary>
    /// Gets the time when the job execution started.
    /// </summary>
    public DateTime StartTime { get; private set; } = DateTime.Now;

    /// <summary>
    /// Gets the time when the job execution ended, or null if still running.
    /// </summary>
    public DateTime? EndTime { get; private set; }

    /// <summary>
    /// Gets the results of completed tasks in the job.
    /// </summary>
    public IReadOnlyDictionary<string, ITaskResult> TaskResults { get; private set; } = new Dictionary<string, ITaskResult>();

    /// <summary>
    /// Adds data to the job execution context.
    /// </summary>
    /// <param name="key">The key for the data.</param>
    /// <param name="value">The value for the data.</param>
    /// <exception cref="ArgumentException">Thrown when key is null or empty.</exception>
    public void AddData(string key, object value)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("Key cannot be null or empty.", nameof(key));

        Data[key] = value;
    }

    /// <summary>
    /// Gets data from the job execution context.
    /// </summary>
    /// <typeparam name="T">The type of the data.</typeparam>
    /// <param name="key">The key for the data.</param>
    /// <returns>The data associated with the key, or default if the key is not found.</returns>
    /// <exception cref="ArgumentException">Thrown when key is null or empty.</exception>
    public T? GetData<T>(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("Key cannot be null or empty.", nameof(key));

        if (Data.TryGetValue(key, out var value) && value is T typedValue)
            return typedValue;

        return default;
    }

    /// <summary>
    /// Updates the state of the job.
    /// </summary>
    /// <param name="newState">The new state for the job.</param>
    /// <param name="message">Optional message for the job state.</param>
    public void UpdateJobState(JobState newState, string? message = null)
    {
        if (Job != null)
        {
            Job.State = newState;
        }
    }
}
