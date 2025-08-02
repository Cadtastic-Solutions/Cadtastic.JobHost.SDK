namespace Cadtastic.JobHost.SDK.Interfaces;

/// <summary>
/// Represents the entry point for executing a job.
/// A job executor orchestrates the execution of multiple task executors.
/// </summary>
public interface IJobExecutor
{
    /// <summary>
    /// Gets the collection of task executors that make up this job.
    /// These tasks will be executed according to their order and concurrency settings.
    /// </summary>
    /// <returns>The collection of task executors.</returns>
    IEnumerable<ITaskExecutor> GetTasks();

    /// <summary>
    /// Validates that the job configuration is valid for execution.
    /// </summary>
    /// <param name="configuration">The job configuration to validate.</param>
    /// <returns>True if the configuration is valid, false otherwise.</returns>
    bool ValidateConfiguration(object configuration);

    /// <summary>
    /// Executes the job asynchronously by orchestrating its task executors.
    /// </summary>
    /// <param name="context">The job execution context containing configuration and shared data.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the job execution.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the job execution result.</returns>
    Task<IJobExecutionResult> ExecuteAsync(IJobContext context, CancellationToken cancellationToken);
}