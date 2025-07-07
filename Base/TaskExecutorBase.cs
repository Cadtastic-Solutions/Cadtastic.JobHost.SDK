using Cadtastic.JobHost.SDK.Interfaces;

namespace Cadtastic.JobHost.SDK.Base;

/// <summary>
/// Base class for task executors that provides common functionality and default implementations.
/// This class implements the core task execution logic and dependency management.
/// </summary>
public abstract class TaskExecutorBase : ITaskExecutor
{
    /// <summary>
    /// Gets the unique identifier for this task.
    /// This ID is used to identify the task in logs, results, and dependencies.
    /// </summary>
    public abstract string TaskId { get; }

    /// <summary>
    /// Gets the collection of task types that this task depends on.
    /// The task will not execute until all dependencies have completed successfully.
    /// </summary>
    public abstract IReadOnlyCollection<Type> Dependencies { get; }

    /// <summary>
    /// Gets a value indicating whether this task is critical.
    /// If a critical task fails, the entire job will fail.
    /// </summary>
    public virtual bool IsCritical => false;

    /// <summary>
    /// Gets a value indicating whether this task can run concurrently with other tasks.
    /// Tasks that can run concurrently may execute in parallel if their dependencies are satisfied.
    /// </summary>
    public virtual bool CanRunConcurrently => false;

    /// <summary>
    /// Executes the task asynchronously.
    /// </summary>
    /// <param name="context">The context containing job information and task results.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the execution result.</returns>
    /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
    /// <exception cref="TaskExecutionException">Thrown when an error occurs during task execution.</exception>
    public abstract Task<ITaskResult> ExecuteAsync(ITaskContext context);

    /// <summary>
    /// Determines if this task can be executed in the current context.
    /// A task can be executed if all its dependencies have completed successfully.
    /// </summary>
    /// <param name="context">The context containing job information and task results.</param>
    /// <returns>True if the task can be executed, false otherwise.</returns>
    /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
    protected virtual bool CanExecute(ITaskContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        // Check if all dependencies have completed successfully
        foreach (var dependencyType in Dependencies)
        {
            var dependencyId = dependencyType.Name;
            if (!context.Results.TryGetValue(dependencyId, out var result) || !result.IsSuccess)
            {
                return false;
            }
        }

        return true;
    }
} 