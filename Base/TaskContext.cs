using Cadtastic.JobHost.SDK.Interfaces;

namespace Cadtastic.JobHost.SDK.Base;

/// <summary>
/// Base class for task contexts that provides common functionality.
/// </summary>
public class TaskContext : ITaskContext
{
    /// <summary>
    /// Gets the job being executed.
    /// </summary>
    public IJob Job { get; }

    /// <summary>
    /// Gets the results of previously executed tasks.
    /// </summary>
    public IReadOnlyDictionary<string, ITaskResult> PreviousResults { get; }

    /// <summary>
    /// Gets all task results from the current job execution.
    /// </summary>
    public IReadOnlyDictionary<string, ITaskResult> Results { get; }

    /// <summary>
    /// Gets the current task being executed.
    /// </summary>
    public ITaskExecutor CurrentTask { get; }

    /// <summary>
    /// Gets or sets custom data for the task execution.
    /// </summary>
    public IDictionary<string, object> Data { get; }

    /// <summary>
    /// Creates a new instance of the TaskContext class.
    /// </summary>
    /// <param name="job">The job being executed.</param>
    /// <param name="currentTask">The current task being executed.</param>
    /// <param name="previousResults">The results of previously executed tasks.</param>
    /// <param name="allResults">All task results from the current job execution.</param>
    public TaskContext(IJob job, ITaskExecutor currentTask, IReadOnlyDictionary<string, ITaskResult>? previousResults = null, IReadOnlyDictionary<string, ITaskResult>? allResults = null)
    {
        Job = job;
        CurrentTask = currentTask;
        PreviousResults = previousResults ?? new Dictionary<string, ITaskResult>();
        Results = allResults ?? new Dictionary<string, ITaskResult>();
        Data = new Dictionary<string, object>();
    }
} 