using Cadtastic.JobHost.SDK.Exceptions;
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
    /// Gets the execution order/step for this task.
    /// Tasks with the same step number can run concurrently.
    /// Tasks are executed in ascending order of step numbers.
    /// Null indicates the task has no specific order requirement.
    /// </summary>
    public abstract int? Step { get; }

    /// <summary>
    /// Gets a value indicating whether this task can run concurrently with other tasks at the same step.
    /// Only applies to tasks with the same Step number.
    /// </summary>
    public virtual bool CanRunConcurrently => true;

    /// <summary>
    /// Gets the collection of task IDs that this task depends on.
    /// The task will not execute until all dependencies have completed successfully.
    /// </summary>
    public virtual IReadOnlyCollection<string> Dependencies => [];

    /// <summary>
    /// Gets a value indicating whether this task is critical.
    /// If a critical task fails, the entire job will fail.
    /// </summary>
    public virtual bool IsCritical => false;

    /// <summary>
    /// Executes the task asynchronously.
    /// </summary>
    /// <param name="context">The context containing job information, previous task results, and cancellation token.</param>
    /// <param name="serviceProvider">Optional service provider for dependency injection.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the execution result.</returns>
    /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
    /// <exception cref="JobExecutionException">Thrown when an error occurs during task execution.</exception>
    public abstract Task<T> ExecuteAsync<T>(ITaskContext context, IServiceProvider? serviceProvider = null) where T : class, ISettableTaskResult, new();

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
        foreach (var dependencyId in Dependencies)
        {
            var result = context.GetPreviousTaskResult(dependencyId);
            if (result == null || !result.IsSuccess)
            {
                return false;
            }
        }

        return true;
    }
}