namespace Cadtastic.JobHost.SDK.Interfaces;

/// <summary>
/// Extends ITaskResult with settable properties for use in task executors.
/// This interface allows task executors to create and populate task results.
/// </summary>
public interface ISettableTaskResult : ITaskResult
{
    /// <summary>
    /// Sets the unique identifier of the task that produced this result.
    /// </summary>
    new string TaskId { get; set; }

    /// <summary>
    /// Sets a value indicating whether the task execution was successful.
    /// </summary>
    new bool IsSuccess { get; set; }

    /// <summary>
    /// Sets the error message if the task failed.
    /// </summary>
    new string? ErrorMessage { get; set; }

    /// <summary>
    /// Sets the exception if the task failed with an exception.
    /// </summary>
    new Exception? Exception { get; set; }

    /// <summary>
    /// Sets the start time of the task execution.
    /// </summary>
    new DateTime StartTime { get; set; }

    /// <summary>
    /// Sets the end time of the task execution.
    /// </summary>
    new DateTime EndTime { get; set; }
}