using Cadtastic.JobHost.SDK.Base;
using Cadtastic.JobHost.SDK.Interfaces;

namespace Cadtastic.JobHost.SDK.Models;

/// <summary>
/// Provides context and state information for a task during its execution.
/// This class contains all the necessary information and utilities that a task executor needs to perform its work.
/// </summary>
public class TaskContext : ITaskContext
{
    /// <summary>
    /// Gets the unique identifier of the job that this task belongs to.
    /// </summary>
    public string JobId { get; }

    /// <summary>
    /// Gets the unique identifier of the task being executed.
    /// </summary>
    public string TaskId { get; }

    /// <summary>
    /// Gets the type of the task being executed.
    /// </summary>
    public Type TaskType { get; }

    /// <summary>
    /// Gets the cancellation token that can be used to cancel the task execution.
    /// </summary>
    public CancellationToken CancellationToken { get; }

    /// <summary>
    /// Gets a dictionary of task results from previously executed tasks in the same job.
    /// The key is the task ID, and the value is the result of that task's execution.
    /// </summary>
    public IReadOnlyDictionary<string, TaskResult> TaskResults { get; }

    /// <summary>
    /// Gets a dictionary of shared data that can be used to pass information between tasks.
    /// This dictionary is shared across all tasks in the same job.
    /// </summary>
    public IDictionary<string, object> SharedData { get; }

    /// <summary>
    /// Gets the logger instance that can be used to log information during task execution.
    /// </summary>
    public ILogger Logger { get; }

    /// <summary>
    /// Gets the service provider that can be used to resolve dependencies during task execution.
    /// </summary>
    public IServiceProvider Services { get; }

    // ITaskContext interface implementation
    /// <summary>
    /// Gets the job being executed.
    /// </summary>
    public IJob Job { get; private set; } = null!;

    /// <summary>
    /// Gets the results of previously executed tasks.
    /// </summary>
    public IReadOnlyDictionary<string, ITaskResult> PreviousResults { get; private set; } = new Dictionary<string, ITaskResult>();

    /// <summary>
    /// Gets all task results from the current job execution.
    /// </summary>
    public IReadOnlyDictionary<string, ITaskResult> Results { get; private set; } = new Dictionary<string, ITaskResult>();

    /// <summary>
    /// Gets the current task being executed.
    /// </summary>
    public ITaskExecutor CurrentTask { get; private set; } = null!;

    /// <summary>
    /// Gets or sets custom data for the task execution.
    /// </summary>
    public IDictionary<string, object> Data => SharedData;

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskContext"/> class.
    /// </summary>
    /// <param name="jobId">The unique identifier of the job.</param>
    /// <param name="taskId">The unique identifier of the task.</param>
    /// <param name="taskType">The type of the task being executed.</param>
    /// <param name="cancellationToken">The cancellation token for the task.</param>
    /// <param name="taskResults">A dictionary of results from previously executed tasks.</param>
    /// <param name="sharedData">A dictionary for sharing data between tasks.</param>
    /// <param name="logger">The logger instance for the task.</param>
    /// <param name="services">The service provider for resolving dependencies.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when any of the required parameters (jobId, taskId, taskType, taskResults, sharedData, logger, services) is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when jobId or taskId is empty or consists only of white-space characters.
    /// </exception>
    public TaskContext(
        string jobId,
        string taskId,
        Type taskType,
        CancellationToken cancellationToken,
        IReadOnlyDictionary<string, TaskResult> taskResults,
        IDictionary<string, object> sharedData,
        ILogger logger,
        IServiceProvider services)
    {
        if (string.IsNullOrWhiteSpace(jobId))
            throw new ArgumentException("Job ID cannot be null or empty.", nameof(jobId));
        if (string.IsNullOrWhiteSpace(taskId))
            throw new ArgumentException("Task ID cannot be null or empty.", nameof(taskId));
        if (taskType == null)
            throw new ArgumentNullException(nameof(taskType));
        if (taskResults == null)
            throw new ArgumentNullException(nameof(taskResults));
        if (sharedData == null)
            throw new ArgumentNullException(nameof(sharedData));
        if (logger == null)
            throw new ArgumentNullException(nameof(logger));
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        JobId = jobId;
        TaskId = taskId;
        TaskType = taskType;
        CancellationToken = cancellationToken;
        TaskResults = taskResults;
        SharedData = sharedData;
        Logger = logger;
        Services = services;
    }

    /// <summary>
    /// Gets the result of a specific task that has already been executed.
    /// </summary>
    /// <param name="taskId">The ID of the task whose result to retrieve.</param>
    /// <returns>The result of the specified task.</returns>
    /// <exception cref="ArgumentNullException">Thrown when taskId is null.</exception>
    /// <exception cref="ArgumentException">Thrown when taskId is empty or consists only of white-space characters.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when no result exists for the specified task ID.</exception>
    public TaskResult GetTaskResult(string taskId)
    {
        if (string.IsNullOrWhiteSpace(taskId))
            throw new ArgumentException("Task ID cannot be null or empty.", nameof(taskId));

        if (!TaskResults.TryGetValue(taskId, out var result))
            throw new KeyNotFoundException($"No result found for task with ID: {taskId}");

        return result;
    }

    /// <summary>
    /// Attempts to get the result of a specific task that has already been executed.
    /// </summary>
    /// <param name="taskId">The ID of the task whose result to retrieve.</param>
    /// <param name="result">When this method returns, contains the result of the specified task if it exists; otherwise, null.</param>
    /// <returns>true if a result exists for the specified task ID; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when taskId is null.</exception>
    /// <exception cref="ArgumentException">Thrown when taskId is empty or consists only of white-space characters.</exception>
    public bool TryGetTaskResult(string taskId, out TaskResult? result)
    {
        if (string.IsNullOrWhiteSpace(taskId))
            throw new ArgumentException("Task ID cannot be null or empty.", nameof(taskId));

        return TaskResults.TryGetValue(taskId, out result);
    }

    /// <summary>
    /// Gets a value from the shared data dictionary.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve.</typeparam>
    /// <param name="key">The key of the value to retrieve.</param>
    /// <returns>The value associated with the specified key.</returns>
    /// <exception cref="ArgumentNullException">Thrown when key is null.</exception>
    /// <exception cref="ArgumentException">Thrown when key is empty or consists only of white-space characters.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when no value exists for the specified key.</exception>
    /// <exception cref="InvalidCastException">Thrown when the value cannot be cast to the specified type.</exception>
    public T GetSharedValue<T>(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or empty.", nameof(key));

        if (!SharedData.TryGetValue(key, out var value))
            throw new KeyNotFoundException($"No value found for key: {key}");

        if (value is not T typedValue)
            throw new InvalidCastException($"Value for key '{key}' cannot be cast to type {typeof(T).Name}");

        return typedValue;
    }

    /// <summary>
    /// Attempts to get a value from the shared data dictionary.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve.</typeparam>
    /// <param name="key">The key of the value to retrieve.</param>
    /// <param name="value">When this method returns, contains the value associated with the specified key if it exists and can be cast to the specified type; otherwise, the default value for the type.</param>
    /// <returns>true if a value exists for the specified key and can be cast to the specified type; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when key is null.</exception>
    /// <exception cref="ArgumentException">Thrown when key is empty or consists only of white-space characters.</exception>
    public bool TryGetSharedValue<T>(string key, out T? value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or empty.", nameof(key));

        if (!SharedData.TryGetValue(key, out var rawValue) || rawValue is not T typedValue)
        {
            value = default;
            return false;
        }

        value = typedValue;
        return true;
    }

    /// <summary>
    /// Sets a value in the shared data dictionary.
    /// </summary>
    /// <typeparam name="T">The type of the value to set.</typeparam>
    /// <param name="key">The key of the value to set.</param>
    /// <param name="value">The value to set.</param>
    /// <exception cref="ArgumentNullException">Thrown when key is null.</exception>
    /// <exception cref="ArgumentException">Thrown when key is empty or consists only of white-space characters.</exception>
    public void SetSharedValue<T>(string key, T value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or empty.", nameof(key));

        SharedData[key] = value!;
    }
} 