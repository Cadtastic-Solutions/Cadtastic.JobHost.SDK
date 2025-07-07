namespace Cadtastic.JobHost.SDK.Interfaces;

/// <summary>
/// Represents the context for a task execution, including access to previous task results.
/// </summary>
public interface ITaskContext
{
    /// <summary>
    /// Gets the job being executed.
    /// </summary>
    IJob Job { get; }

    /// <summary>
    /// Gets the results of previously executed tasks.
    /// </summary>
    IReadOnlyDictionary<string, ITaskResult> PreviousResults { get; }

    /// <summary>
    /// Gets all task results from the current job execution.
    /// </summary>
    IReadOnlyDictionary<string, ITaskResult> Results { get; }

    /// <summary>
    /// Gets the current task being executed.
    /// </summary>
    ITaskExecutor CurrentTask { get; }

    /// <summary>
    /// Gets or sets custom data for the task execution.
    /// </summary>
    IDictionary<string, object> Data { get; }
} 