using Cadtastic.JobHost.SDK.Models;
using Microsoft.Extensions.Logging;

namespace Cadtastic.JobHost.SDK.Interfaces;

/// <summary>
/// Interface for job executors that can handle specific job types.
/// This is the main interface that job DLLs must implement.
/// </summary>
public interface IJobExecutor
{
    /// <summary>
    /// Array of job types that this executor can handle
    /// </summary>
    string[] SupportedJobTypes { get; }

    /// <summary>
    /// Description of what this executor does
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Determines if this executor can execute the specified job
    /// </summary>
    /// <param name="job">The job to check</param>
    /// <returns>True if this executor can handle the job, false otherwise</returns>
    bool CanExecute(IJob job);

    /// <summary>
    /// Executes the specified job with the given context
    /// </summary>
    /// <param name="context">The execution context containing job and services</param>
    /// <returns>The result of job execution</returns>
    Task<IJobExecutionResult> ExecuteAsync(IJobExecutionContext context);
}

/// <summary>
/// Extension methods for <see cref="IJobExecutor"/> to provide common functionality
/// and reduce code duplication across job implementations.
/// </summary>
public static class JobExecutorExtensions
{
    /// <summary>
    /// Executes a job with automatic timing and error handling, returning a user-specified result type.
    /// </summary>
    /// <typeparam name="TResult">The type of result to return from the job action.</typeparam>
    /// <param name="executor">The job executor instance.</param>
    /// <param name="context">The execution context.</param>
    /// <param name="jobAction">The action to perform for the job that returns the specified result type.</param>
    /// <returns>A <see cref="IJobExecutionResult"/> with the job action result serialized in Details.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    public static async Task<IJobExecutionResult> ExecuteWithTimingAsync<TResult>(
        this IJobExecutor executor,
        IJobExecutionContext context,
        Func<IJobExecutionContext, Task<TResult>> jobAction)
    {
        ArgumentNullException.ThrowIfNull(executor);
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(jobAction);

        var startTime = DateTime.Now;
        var logger = context.ServiceProvider.GetLogger(executor.GetType());
        
        try
        {
            var jobName = context.Job.Name;
            var jobType = context.Job.JobType;
            
            context.Progress?.Report($"Starting {jobType} job: {jobName}");
            logger?.LogInformation("Starting {JobType} job {JobName} (ID: {JobId})", jobType, jobName, context.Job.Id);
            
            var result = await jobAction(context);
            
            context.Progress?.Report($"Successfully completed {jobType} job: {jobName}");
            logger?.LogInformation("Completed {JobType} job {JobName} (ID: {JobId})", jobType, jobName, context.Job.Id);
            
            var resultDetails = result?.ToString() ?? "Operation completed successfully";
            return JobExecutionResult.Successful(resultDetails, startTime, DateTime.Now);
        }
        catch (OperationCanceledException)
        {
            var message = $"Job {context.Job.Name} was cancelled";
            context.Progress?.Report(message);
            logger?.LogWarning("Job {JobName} (ID: {JobId}) was cancelled", context.Job.Name, context.Job.Id);
            return JobExecutionResult.Cancelled(null, startTime, DateTime.Now);
        }
        catch (Exception ex)
        {
            var message = $"Job {context.Job.Name} failed: {ex.Message}";
            context.Progress?.Report(message);
            logger?.LogError(ex, "Job {JobName} (ID: {JobId}) failed", context.Job.Name, context.Job.Id);
            return JobExecutionResult.Failed(ex.Message, ex.StackTrace, startTime, DateTime.Now);
        }
    }

    /// <summary>
    /// Executes a job with automatic timing and error handling for void operations.
    /// </summary>
    /// <param name="executor">The job executor instance.</param>
    /// <param name="context">The execution context.</param>
    /// <param name="jobAction">The action to perform for the job.</param>
    /// <returns>A <see cref="IJobExecutionResult"/> indicating success or failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    public static async Task<IJobExecutionResult> ExecuteWithTimingAsync(
        this IJobExecutor executor,
        IJobExecutionContext context,
        Func<IJobExecutionContext, Task> jobAction)
    {
        return await executor.ExecuteWithTimingAsync(context, async ctx =>
        {
            await jobAction(ctx);
            return "Operation completed successfully";
        });
    }

    /// <summary>
    /// Validates that the executor can handle the specified job type.
    /// </summary>
    /// <param name="executor">The job executor instance.</param>
    /// <param name="jobType">The job type to validate.</param>
    /// <returns>True if the executor supports the job type, false otherwise.</returns>
    /// <exception cref="ArgumentNullException">Thrown when executor is null.</exception>
    public static bool SupportsJobType(this IJobExecutor executor, string jobType)
    {
        ArgumentNullException.ThrowIfNull(executor);
        
        if (string.IsNullOrWhiteSpace(jobType)) 
            return false;

        return executor.SupportedJobTypes.Contains(jobType, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets a typed logger for the executor from the execution context.
    /// </summary>
    /// <typeparam name="TExecutor">The type of the executor to create a logger for.</typeparam>
    /// <param name="executor">The job executor instance.</param>
    /// <param name="context">The execution context containing the service provider.</param>
    /// <returns>A logger instance for the executor type, or null if logging is not configured.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    public static ILogger<TExecutor>? GetLogger<TExecutor>(this TExecutor executor, IJobExecutionContext context) 
        where TExecutor : class, IJobExecutor
    {
        ArgumentNullException.ThrowIfNull(executor);
        ArgumentNullException.ThrowIfNull(context);
        
        return context.ServiceProvider.GetLogger<TExecutor>();
    }
}