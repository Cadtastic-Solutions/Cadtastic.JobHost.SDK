using Cadtastic.JobHost.SDK.Exceptions;

namespace Cadtastic.JobHost.SDK.Interfaces;

/// <summary>
/// Attribute to declare what job types a task executor supports.
/// This replaces manual maintenance of job type arrays.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class SupportedJobTypeAttribute : Attribute
{
    /// <summary>
    /// Gets the job type this executor supports.
    /// </summary>
    public string JobType { get; }

    /// <summary>
    /// Gets a value indicating whether this is the primary job type for this executor.
    /// </summary>
    public bool IsPrimary { get; set; } = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="SupportedJobTypeAttribute"/> class.
    /// </summary>
    /// <param name="jobType">The job type this executor supports.</param>
    public SupportedJobTypeAttribute(string jobType)
    {
        JobType = jobType ?? throw new ArgumentNullException(nameof(jobType));
    }
}

/// <summary>
/// Represents a task executor that can be executed as part of a job.
/// Tasks can be executed in a specific order or concurrently based on their configuration.
/// </summary>
public interface ITaskExecutor
{
    /// <summary>
    /// Gets the unique identifier for this task.
    /// This ID is used to identify the task in logs, results, and dependencies.
    /// </summary>
    string TaskId { get; }

    /// <summary>
    /// Gets the execution order/step for this task.
    /// Tasks with the same step number can run concurrently.
    /// Tasks are executed in ascending order of step numbers.
    /// Null indicates the task has no specific order requirement.
    /// </summary>
    int? Step { get; }

    /// <summary>
    /// Gets a value indicating whether this task can run concurrently with other tasks at the same step.
    /// Only applies to tasks with the same Step number.
    /// </summary>
    bool CanRunConcurrently { get; }

    /// <summary>
    /// Gets the collection of task IDs that this task depends on.
    /// The task will not execute until all dependencies have completed successfully.
    /// </summary>
    IReadOnlyCollection<string> Dependencies { get; }

    /// <summary>
    /// Gets a value indicating whether this task is critical.
    /// If a critical task fails, the entire job will fail.
    /// </summary>
    bool IsCritical { get; }

    /// <summary>
    /// Executes the task asynchronously.
    /// </summary>
    /// <param name="context">The context containing job information, previous task results, and cancellation token.</param>
    /// <param name="serviceProvider">Optional service provider for dependency injection.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the execution result.</returns>
    /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
    /// <exception cref="JobExecutionException">Thrown when an error occurs during task execution.</exception>
    Task<T> ExecuteAsync<T>(ITaskContext context, IServiceProvider? serviceProvider = null) where T : class, ISettableTaskResult, new();
}