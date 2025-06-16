using Microsoft.Extensions.Logging;

namespace Cadtastic.JobHost.SDK.Interfaces;

/// <summary>
/// Represents the context in which a job is executed.
/// </summary>
public interface IJobExecutionContext
{
    /// <summary>
    /// Gets the job being executed.
    /// </summary>
    IJob Job { get; }

    /// <summary>
    /// Gets the execution history of the job.
    /// </summary>
    IJobExecutionHistory History { get; }

    /// <summary>
    /// Gets the data associated with the job execution.
    /// </summary>
    IDictionary<string, object> Data { get; }

    /// <summary>
    /// Gets the cancellation token for the job execution.
    /// </summary>
    CancellationToken CancellationToken { get; }

    /// <summary>
    /// Gets the logger for the job execution context.
    /// </summary>
    ILogger Logger { get; }

    /// <summary>
    /// Gets the service provider for dependency injection.
    /// </summary>
    IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Gets the progress reporter for the job execution.
    /// </summary>
    IProgress<string>? Progress { get; }

    /// <summary>
    /// Adds data to the job execution context.
    /// </summary>
    /// <param name="key">The key for the data.</param>
    /// <param name="value">The value for the data.</param>
    void AddData(string key, object value);

    /// <summary>
    /// Gets data from the job execution context.
    /// </summary>
    /// <typeparam name="T">The type of data to retrieve.</typeparam>
    /// <param name="key">The key for the data.</param>
    /// <returns>The retrieved data, or null if the data is not found.</returns>
    T? GetData<T>(string key);

    /// <summary>
    /// Updates the state of the job.
    /// </summary>
    /// <param name="newState">The new state for the job.</param>
    /// <param name="message">Optional message to accompany the state update.</param>
    void UpdateJobState(JobState newState, string? message = null);
}
