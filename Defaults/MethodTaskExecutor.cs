using System.Reflection;

using Cadtastic.JobHost.SDK.Exceptions;
using Cadtastic.JobHost.SDK.Interfaces;

namespace Cadtastic.JobHost.SDK.Defaults;

/// <summary>
/// Task executor that wraps a method marked with [Task] attribute.
/// </summary>
public class MethodTaskExecutor : ITaskExecutor
{
    private readonly object _instance;
    private readonly MethodInfo _method;

    /// <summary>
    /// Gets the unique identifier for this task.
    /// </summary>
    public string TaskId { get; }

    /// <summary>
    /// Gets the execution order/step for this task.
    /// </summary>
    public int? Step { get; }

    /// <summary>
    /// Gets a value indicating whether this task can run concurrently with other tasks at the same step.
    /// </summary>
    public bool CanRunConcurrently { get; }

    /// <summary>
    /// Gets the collection of task IDs that this task depends on.
    /// </summary>
    public IReadOnlyCollection<string> Dependencies { get; }

    /// <summary>
    /// Gets a value indicating whether this task is critical.
    /// </summary>
    public bool IsCritical { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MethodTaskExecutor"/> class.
    /// </summary>
    public MethodTaskExecutor(
        object instance,
        MethodInfo method,
        string taskId,
        int step,
        bool canRunConcurrently,
        bool isCritical,
        string[] dependencies)
    {
        _instance = instance ?? throw new ArgumentNullException(nameof(instance));
        _method = method ?? throw new ArgumentNullException(nameof(method));
        TaskId = taskId ?? throw new ArgumentNullException(nameof(taskId));
        Step = step;
        CanRunConcurrently = canRunConcurrently;
        IsCritical = isCritical;
        Dependencies = dependencies ?? [];
    }

    /// <summary>
    /// Executes the task asynchronously by invoking the wrapped method.
    /// </summary>
    /// <param name="context">The task execution context.</param>
    /// <param name="serviceProvider">Optional service provider for dependency injection.</param>
    /// <returns>The task result.</returns>
    public async Task<T> ExecuteAsync<T>(ITaskContext context, IServiceProvider? serviceProvider)
        where T : class, ISettableTaskResult, new()
    {
        // Check if any previous tasks failed before starting execution
        foreach (var previousResult in context.PreviousTaskResults.Values)
        {
            if (!previousResult.IsSuccess)
            {
                var errorMessage = $"Cannot execute task because previous task '{previousResult.TaskId}' failed.";

                var result = new T
                {
                    TaskId = TaskId,
                    IsSuccess = false,
                    ErrorMessage = errorMessage,
                    Exception = previousResult.Exception,
                    StartTime = DateTime.UtcNow,
                    EndTime = DateTime.UtcNow
                };

                throw new JobExecutionException(result.ErrorMessage, context.JobContext.JobId, JobExecutionFailureReason.TaskFailure);

            }
        }

        var startTime = DateTime.UtcNow;

        try
        {
            // Check if dependencies are satisfied
            foreach (var dependency in Dependencies)
            {
                var depResult = context.GetPreviousTaskResult(dependency);
                if (depResult == null || !depResult.IsSuccess)
                {
                    return new T
                    {
                        TaskId = TaskId,
                        IsSuccess = false,
                        ErrorMessage = $"Dependency '{dependency}' not satisfied",
                        StartTime = startTime,
                        EndTime = DateTime.UtcNow
                    };
                }
            }

            // Determine method parameters
            var parameters = _method.GetParameters();
            object? result = null;

            if (parameters.Length == 0)
            {
                // No parameters
                result = _method.Invoke(_instance, null);
            }
            else if (parameters.Length == 1 && parameters[0].ParameterType == typeof(ITaskContext))
            {
                // Single ITaskContext parameter
                result = _method.Invoke(_instance, [context]);
            }
            else if (parameters.Length == 2 &&
                     parameters[0].ParameterType == typeof(ITaskContext) &&
                     parameters[1].ParameterType == typeof(IServiceProvider))
            {
                // ITaskContext and IServiceProvider parameters
                if (serviceProvider == null)
                {
                    throw new InvalidOperationException("Service provider is required for this task but was not provided.");
                }
                
                result = _method.Invoke(_instance, [context, serviceProvider]);
            }
            else if (parameters.Length == 1 && parameters[0].ParameterType == typeof(IServiceProvider))
            {
                // Single IServiceProvider parameter
                if (serviceProvider == null)
                {
                    throw new InvalidOperationException("Service provider is required for this task but was not provided.");
                }
                result = _method.Invoke(_instance, [serviceProvider]);
            }
            else
            {
                throw new InvalidOperationException(
                    $"Task method '{_method.Name}' must have either no parameters, a single ITaskContext parameter, a single IServiceProvider parameter, or both ITaskContext and IServiceProvider parameters.");
            }

            // Handle async methods
            if (result is Task task)
            {
                await task;

                // Check if it's a Task<T>
                var taskType = task.GetType();
                // TODO: Fix to Handle Task<T> properly
                //if (taskType.IsGenericType && taskType.GetGenericTypeDefinition() == typeof(Task<>))
                if (taskType.IsGenericType)
                {
                    var resultProperty = taskType.GetProperty("Result");
                    result = resultProperty?.GetValue(task);
                }
                else
                {
                    result = null;
                }
            }

            // If the result is already the correct type T (e.g., VaultArchivalTaskResult), return it directly
            if (result is T directResult)
            {
                return directResult;
            }

            // Create success result for other types
            var taskResult = new T
            {
                TaskId = TaskId,
                IsSuccess = true,
                StartTime = startTime,
                EndTime = DateTime.UtcNow
            };

            // Store result data if any
            if (result != null)
            {
                taskResult.SetData("Result", result);
            }

            return taskResult;
        }
        catch (Exception ex)
        {
            // Unwrap target invocation exception
            var actualException = ex is TargetInvocationException tie ? tie.InnerException ?? ex : ex;
            var result = new T
            {
                TaskId = TaskId,
                IsSuccess = false,
                ErrorMessage = actualException.Message,
                Exception = ex,
                StartTime = startTime,
                EndTime = DateTime.UtcNow
            };

            throw new JobExecutionException(result.ErrorMessage, context.JobContext.JobId, JobExecutionFailureReason.TaskFailure);
        }
    }

}