using System.Reflection;

using Cadtastic.JobHost.SDK.Attributes;
using Cadtastic.JobHost.SDK.Interfaces;

namespace Cadtastic.JobHost.SDK.Defaults;

/// <summary>
/// Default job executor that discovers and executes task methods marked with [Task] attribute.
/// </summary>
/// <typeparam name="TTaskResult">The type of task result this job executor produces.</typeparam>
public class DefaultJobExecutor<TTaskResult> : IJobExecutor
    where TTaskResult : class, ISettableTaskResult, new()
{
    private readonly object _jobInstance;
    private readonly Type _jobType;
    private readonly List<ITaskExecutor> _taskExecutors;
    private readonly IServiceProvider? _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultJobExecutor{TTaskResult}"/> class.
    /// </summary>
    /// <param name="jobInstance">The job instance to execute tasks on.</param>
    /// <param name="serviceProvider">Optional service provider for task dependency injection.</param>
    public DefaultJobExecutor(object jobInstance, IServiceProvider? serviceProvider = null)
    {
        _jobInstance = jobInstance ?? throw new ArgumentNullException(nameof(jobInstance));
        _jobType = jobInstance.GetType();
        _serviceProvider = serviceProvider;
        _taskExecutors = DiscoverTasks();
    }

    /// <summary>
    /// Gets the collection of task executors that make up this job.
    /// </summary>
    /// <returns>The collection of task executors.</returns>
    public IEnumerable<ITaskExecutor> GetTasks()
    {
        return _taskExecutors;
    }

    /// <summary>
    /// Validates that the job configuration is valid for execution.
    /// </summary>
    /// <param name="configuration">The job configuration to validate.</param>
    /// <returns>True if the configuration is valid, false otherwise.</returns>
    public bool ValidateConfiguration(object configuration)
    {
        // Default validation - can be overridden
        return configuration != null;
    }

    /// <summary>
    /// Executes the job asynchronously by orchestrating its task executors.
    /// </summary>
    /// <param name="context">The job execution context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The job execution result.</returns>
    public async Task<IJobExecutionResult> ExecuteAsync(IJobContext context, CancellationToken cancellationToken)
    {
        var executionId = Guid.NewGuid().ToString();
        var startTime = DateTime.UtcNow;
        var taskResults = new Dictionary<string, ITaskResult>();
        var taskContexts = new Dictionary<string, DefaultTaskContext>();

        try
        {
            // Group tasks by step
            var tasksByStep = _taskExecutors
                .GroupBy(t => t.Step ?? int.MaxValue)
                .OrderBy(g => g.Key);

            if (!tasksByStep.Any())
                throw new InvalidOperationException($"No tasks found in {context.JobName} job to execute.");

            foreach (var stepGroup in tasksByStep)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    // If cancellation is requested, return a cancelled result
                    return CreateCancelledResult(executionId, context, startTime, taskResults);
                }

                // Execute tasks in this step
                var stepTasks = stepGroup.ToList();

                if (stepTasks.Count == 1 || !stepTasks.All(t => t.CanRunConcurrently))
                {
                    // Execute sequentially
                    foreach (var task in stepTasks)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return CreateCancelledResult(executionId, context, startTime, taskResults);
                        }

                        // Capture task execution exceptions independently so other tasks can still run if current task fails and is not critical
                        var result = new TTaskResult
                        {
                            TaskId = task.TaskId,
                            StartTime = DateTime.UtcNow,
                            EndTime = DateTime.UtcNow
                        };

                        DefaultTaskContext taskContext = null!;

                        try
                        {
                            taskContext = new DefaultTaskContext(context, task.TaskId, task.IsCritical, taskResults);
                            result = await task.ExecuteAsync<TTaskResult>(taskContext, _serviceProvider);
                            
                            // Update context properties based on task result
                            taskContext.IsSuccess = result.IsSuccess;
                            taskContext.BlockSubsequentTasks = result.BlockSubsequentTasks;
                        }
                        catch (Exception ex)
                        {
                            result.IsSuccess = false;
                            result.ErrorMessage = $"\n\t\t{ex.Message}";
                            result.Exception = ex;
                            result.EndTime = DateTime.UtcNow;
                        }
                        finally
                        {
                            taskResults[task.TaskId] = result;
                            if (taskContext != null)
                            {
                                taskContexts[task.TaskId] = taskContext;
                            }
                        }

                        if (!result.IsSuccess && taskContext?.IsCritical == true)
                        {
                            return CreateFailedResult(executionId, context, startTime, taskResults, $"Critical task '{task.TaskId}' failed: \t{result.ErrorMessage}");
                        }
                        
                        // Check if this task result indicates that subsequent tasks should be blocked
                        if (result.IsSuccess && taskContext?.BlockSubsequentTasks == true)
                        {
                            // Log that processing is being terminated early due to no work needed
                            context.Log(Interfaces.JobLogLevel.Information, $"Task '{task.TaskId}' completed successfully but indicated no further processing is needed. Terminating remaining tasks.");
                            
                            // Return success result with current task results
                            return CreateSuccessResult(executionId, context, startTime, taskResults);
                        }
                    }
                }
                else
                {
                    // Execute concurrently
                    var concurrentTasks = stepTasks.Select(async task =>
                    {
                        var taskContext = new DefaultTaskContext(context, task.TaskId, task.IsCritical, taskResults);
                        var result = await task.ExecuteAsync<TTaskResult>(taskContext, _serviceProvider);
                        
                        // Update context properties based on task result
                        taskContext.IsSuccess = result.IsSuccess;
                        taskContext.BlockSubsequentTasks = result.BlockSubsequentTasks;
                        
                        return (task, result, taskContext);
                    });

                    var results = await Task.WhenAll(concurrentTasks);

                    foreach (var (task, result, taskContext) in results)
                    {
                        taskResults[task.TaskId] = result;
                        if (taskContext != null)
                        {
                            taskContexts[task.TaskId] = taskContext;
                        }

                        if (!result.IsSuccess && taskContext.IsCritical)
                        {
                            return CreateFailedResult(executionId, context, startTime, taskResults,
                                $"Critical task '{task.TaskId}' failed: {result.ErrorMessage}");
                        }
                        
                        // Check if this task result indicates that subsequent tasks should be blocked
                        if (result.IsSuccess && taskContext.BlockSubsequentTasks)
                        {
                            // Log that processing is being terminated early due to no work needed
                            context.Log(Interfaces.JobLogLevel.Information, $"Task '{task.TaskId}' completed successfully but indicated no further processing is needed. Terminating remaining tasks.");
                            
                            // Return success result with current task results
                            return CreateSuccessResult(executionId, context, startTime, taskResults);
                        }
                    }
                }
            }

            if (taskResults.Any(taskResults => !taskResults.Value.IsSuccess))
            {
                // Check if any critical task failed
                var failedTasks = taskResults
                    .Where(tr => !tr.Value.IsSuccess)
                    .ToList();

                // Find which of the failed tasks were critical using stored task contexts
                var criticalFailedTasks = failedTasks
                    .Where(tr => taskContexts.TryGetValue(tr.Key, out var ctx) && ctx.IsCritical)
                    .ToList();

                // Only fail the job if at least one critical task failed
                if (criticalFailedTasks.Any())
                {
                    string errorMsgs;
                    if (criticalFailedTasks.Count == 1)
                    {
                        // Single critical error: tab space then error message
                        errorMsgs = $"\t{criticalFailedTasks[0].Value.ErrorMessage}";
                    }
                    else
                    {
                        // Multiple critical errors: each error on new line with tab, no newline after last error
                        errorMsgs = string.Join(Environment.NewLine,
                            criticalFailedTasks.Select(tr => $"\t{tr.Key}: {tr.Value.ErrorMessage}"));
                    }

                    return CreateFailedResult(executionId, context, startTime, taskResults,
                        $"{context.JobName} job failed with the following critical task error(s):{Environment.NewLine}{errorMsgs}");
                }
                else
                {
                    // Only non-critical tasks failed - log but don't fail the job
                    var nonCriticalFailedTasks = failedTasks;
                    foreach (var failedTask in nonCriticalFailedTasks)
                    {
                        context.Log(Interfaces.JobLogLevel.Warning, 
                            $"Non-critical task '{failedTask.Key}' failed: {failedTask.Value.ErrorMessage}");
                    }
                    
                    context.Log(Interfaces.JobLogLevel.Information, 
                        $"Job completed successfully despite {nonCriticalFailedTasks.Count} non-critical task failure(s)");
                }
            }


         return CreateSuccessResult(executionId, context, startTime, taskResults);
        }
        catch (Exception ex)
        {
            return CreateFailedResult(executionId, context, startTime, taskResults, $"{ex.Message}", ex);
        }
    }

    /// <summary>
    /// Discovers task methods marked with [Task] attribute.
    /// </summary>
    /// <returns>A list of task executors.</returns>
    private List<ITaskExecutor> DiscoverTasks()
    {
        var taskExecutors = new List<ITaskExecutor>();

        // Look for both instance and static methods
        var methods = _jobType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
            .Where(m => m.GetCustomAttribute<TaskAttribute>() != null);

        foreach (var method in methods)
        {
            var taskAttribute = method.GetCustomAttribute<TaskAttribute>()!;
            var taskId = taskAttribute.TaskId ?? method.Name;

            var executor = new MethodTaskExecutor(
                _jobInstance,
                method,
                taskId,
                taskAttribute.Step,
                taskAttribute.CanRunConcurrently,
                taskAttribute.IsCritical,
                taskAttribute.DependsOn ?? []
            );

            taskExecutors.Add(executor);
        }

        return taskExecutors;
    }

    private static IJobExecutionResult CreateSuccessResult(string executionId, IJobContext context,
                                                           DateTime startTime, Dictionary<string, ITaskResult> taskResults)
                                                           => new DefaultJobExecutionResult
                                                           {
                                                               ExecutionId = executionId,
                                                               JobId = context.JobId,
                                                               JobType = context.JobType,
                                                               IsSuccess = true,
                                                               Status = JobExecutionStatus.Success,
                                                               StartTime = startTime,
                                                               EndTime = DateTime.UtcNow,
                                                               TaskResults = taskResults
                                                           };

    private static IJobExecutionResult CreateFailedResult(string executionId, IJobContext context,
                                                          DateTime startTime, Dictionary<string, ITaskResult> taskResults,
                                                          string errorMessage, Exception? exception = null)
                                                          => new DefaultJobExecutionResult
                                                          {
                                                              ExecutionId = executionId,
                                                              JobId = context.JobId,
                                                              JobType = context.JobType,
                                                              IsSuccess = false,
                                                              Status = JobExecutionStatus.Failed,
                                                              ErrorMessage = errorMessage,
                                                              Exception = exception,
                                                              StartTime = startTime,
                                                              EndTime = DateTime.UtcNow,
                                                              TaskResults = taskResults
                                                          };

    private static IJobExecutionResult CreateCancelledResult(string executionId, IJobContext context,
                                                             DateTime startTime, Dictionary<string, ITaskResult> taskResults)
                                                             => new DefaultJobExecutionResult
                                                             {
                                                                 ExecutionId = executionId,
                                                                 JobId = context.JobId,
                                                                 JobType = context.JobType,
                                                                 IsSuccess = false,
                                                                 Status = JobExecutionStatus.Cancelled,
                                                                 ErrorMessage = "Job execution was cancelled",
                                                                 StartTime = startTime,
                                                                 EndTime = DateTime.UtcNow,
                                                                 TaskResults = taskResults
                                                             };
}

/// <summary>
/// Non-generic default job executor that uses DefaultTaskResult.
/// Provided for backward compatibility and simple scenarios.
/// </summary>
public class DefaultJobExecutor : DefaultJobExecutor<DefaultTaskResult>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultJobExecutor"/> class.
    /// </summary>
    /// <param name="jobInstance">The job instance to execute tasks on.</param>
    /// <param name="serviceProvider">Optional service provider for task dependency injection.</param>
    public DefaultJobExecutor(object jobInstance, IServiceProvider? serviceProvider = null)
        : base(jobInstance, serviceProvider)
    {
    }
}