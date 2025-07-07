namespace Cadtastic.JobHost.SDK.Interfaces;

/// <summary>
/// Represents a task executor that can be executed as part of a job.
/// This interface defines the contract for all task executors in the system.
/// </summary>
public interface ITaskExecutor
{
    /// <summary>
    /// Gets the unique identifier for this task.
    /// This ID is used to identify the task in logs, results, and dependencies.
    /// </summary>
    string TaskId { get; }

    /// <summary>
    /// Gets the collection of task types that this task depends on.
    /// The task will not execute until all dependencies have completed successfully.
    /// </summary>
    IReadOnlyCollection<Type> Dependencies { get; }

    /// <summary>
    /// Gets a value indicating whether this task is critical.
    /// If a critical task fails, the entire job will fail.
    /// </summary>
    bool IsCritical { get; }

    /// <summary>
    /// Gets a value indicating whether this task can run concurrently with other tasks.
    /// Tasks that can run concurrently may execute in parallel if their dependencies are satisfied.
    /// </summary>
    bool CanRunConcurrently { get; }

    /// <summary>
    /// Executes the task asynchronously.
    /// </summary>
    /// <param name="context">The context containing job information and task results.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the execution result.</returns>
    /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
    /// <exception cref="TaskExecutionException">Thrown when an error occurs during task execution.</exception>
    Task<ITaskResult> ExecuteAsync(ITaskContext context);
} 