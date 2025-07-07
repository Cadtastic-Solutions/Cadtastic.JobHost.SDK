using Cadtastic.JobHost.SDK.Interfaces;

namespace Cadtastic.JobHost.SDK.Models;

/// <summary>
/// Represents the result of a job execution, including success status, task results, and any errors that occurred.
/// This class provides a comprehensive record of the job's execution outcome.
/// </summary>
public class JobExecutionResult : IJobExecutionResult
{
    /// <summary>
    /// Gets the unique identifier of the job.
    /// </summary>
    public string JobId { get; }

    /// <summary>
    /// Gets the type of the job.
    /// </summary>
    public string JobType { get; }

    /// <summary>
    /// Gets a value indicating whether the job execution was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets the status of the job execution.
    /// </summary>
    public JobStatus Status { get; }

    /// <summary>
    /// Gets the error message if the job execution failed, or null if it succeeded.
    /// </summary>
    public string? ErrorMessage { get; }

    /// <summary>
    /// Gets the exception that caused the job execution to fail, if any.
    /// </summary>
    public Exception? Exception { get; }

    /// <summary>
    /// Gets the timestamp when the job execution started.
    /// </summary>
    public DateTimeOffset StartedAt { get; }

    /// <summary>
    /// Gets the timestamp when the job execution completed.
    /// </summary>
    public DateTimeOffset CompletedAt { get; }

    /// <summary>
    /// Gets the duration of the job execution.
    /// </summary>
    public TimeSpan Duration => CompletedAt - StartedAt;

    /// <summary>
    /// Gets the results of all tasks in the job.
    /// </summary>
    public IReadOnlyDictionary<string, ITaskResult> TaskResults { get; }

    /// <summary>
    /// Gets a collection of validation errors that occurred during job execution, if any.
    /// </summary>
    public IReadOnlyCollection<string> ValidationErrors { get; }

    /// <summary>
    /// Gets the specific result state of the job execution.
    /// </summary>
    public ResultState State
    {
        get
        {
            return Status switch
            {
                JobStatus.Completed => ResultState.Successful,
                JobStatus.Failed => ResultState.Failed,
                JobStatus.ValidationFailed => ResultState.Failed,
                JobStatus.Cancelled => ResultState.Cancelled,
                _ => ResultState.Unknown
            };
        }
    }

    /// <summary>
    /// Gets additional details about the execution.
    /// </summary>
    public string? Details => ErrorMessage;

    /// <summary>
    /// Gets when the execution started.
    /// </summary>
    public DateTime StartTime => StartedAt.DateTime;

    /// <summary>
    /// Gets when the execution ended.
    /// </summary>
    public DateTime? EndTime => CompletedAt.DateTime;

    /// <summary>
    /// Initializes a new instance of the <see cref="JobExecutionResult"/> class.
    /// </summary>
    /// <param name="jobId">The unique identifier of the job.</param>
    /// <param name="jobType">The type of the job.</param>
    /// <param name="isSuccess">Whether the job execution was successful.</param>
    /// <param name="status">The status of the job execution.</param>
    /// <param name="errorMessage">The error message if the job execution failed.</param>
    /// <param name="exception">The exception that caused the job execution to fail.</param>
    /// <param name="startedAt">The timestamp when the job execution started.</param>
    /// <param name="completedAt">The timestamp when the job execution completed.</param>
    /// <param name="taskResults">A dictionary of task results from all tasks in the job.</param>
    /// <param name="validationErrors">A collection of validation errors that occurred during job execution.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when any of the required parameters (jobId, jobType, taskResults, validationErrors) is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when jobId or jobType is empty or consists only of white-space characters.
    /// </exception>
    public JobExecutionResult(
        string jobId,
        string jobType,
        bool isSuccess,
        JobStatus status,
        string? errorMessage,
        Exception? exception,
        DateTimeOffset startedAt,
        DateTimeOffset completedAt,
        IReadOnlyDictionary<string, ITaskResult> taskResults,
        IReadOnlyCollection<string> validationErrors)
    {
        if (string.IsNullOrWhiteSpace(jobId))
            throw new ArgumentException("Job ID cannot be null or empty.", nameof(jobId));
        if (string.IsNullOrWhiteSpace(jobType))
            throw new ArgumentException("Job type cannot be null or empty.", nameof(jobType));
        if (taskResults == null)
            throw new ArgumentNullException(nameof(taskResults));
        if (validationErrors == null)
            throw new ArgumentNullException(nameof(validationErrors));

        JobId = jobId;
        JobType = jobType;
        IsSuccess = isSuccess;
        Status = status;
        ErrorMessage = errorMessage;
        Exception = exception;
        StartedAt = startedAt;
        CompletedAt = completedAt;
        TaskResults = taskResults;
        ValidationErrors = validationErrors;
    }

    /// <summary>
    /// Creates a successful job execution result.
    /// </summary>
    /// <param name="jobId">The unique identifier of the job.</param>
    /// <param name="jobType">The type of the job.</param>
    /// <param name="taskResults">A dictionary of task results from all tasks in the job.</param>
    /// <param name="startedAt">The timestamp when the job execution started.</param>
    /// <param name="completedAt">The timestamp when the job execution completed.</param>
    /// <returns>A new <see cref="JobExecutionResult"/> instance representing a successful execution.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when any of the required parameters (jobId, jobType, taskResults) is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when jobId or jobType is empty or consists only of white-space characters.
    /// </exception>
    public static JobExecutionResult Success(
        string jobId,
        string jobType,
        IReadOnlyDictionary<string, ITaskResult> taskResults,
        DateTimeOffset startedAt,
        DateTimeOffset completedAt)
    {
        return new JobExecutionResult(
            jobId,
            jobType,
            isSuccess: true,
            status: JobStatus.Completed,
            errorMessage: null,
            exception: null,
            startedAt,
            completedAt,
            taskResults,
            Array.Empty<string>());
    }

    /// <summary>
    /// Creates a failed job execution result.
    /// </summary>
    /// <param name="jobId">The unique identifier of the job.</param>
    /// <param name="jobType">The type of the job.</param>
    /// <param name="errorMessage">The error message describing why the job failed.</param>
    /// <param name="exception">The exception that caused the job to fail.</param>
    /// <param name="taskResults">A dictionary of task results from all tasks in the job.</param>
    /// <param name="startedAt">The timestamp when the job execution started.</param>
    /// <param name="completedAt">The timestamp when the job execution completed.</param>
    /// <returns>A new <see cref="JobExecutionResult"/> instance representing a failed execution.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when any of the required parameters (jobId, jobType, errorMessage, taskResults) is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when jobId or jobType is empty or consists only of white-space characters.
    /// </exception>
    public static JobExecutionResult Failure(
        string jobId,
        string jobType,
        string errorMessage,
        Exception? exception,
        IReadOnlyDictionary<string, ITaskResult> taskResults,
        DateTimeOffset startedAt,
        DateTimeOffset completedAt)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
            throw new ArgumentException("Error message cannot be null or empty.", nameof(errorMessage));

        return new JobExecutionResult(
            jobId,
            jobType,
            isSuccess: false,
            status: JobStatus.Failed,
            errorMessage,
            exception,
            startedAt,
            completedAt,
            taskResults,
            Array.Empty<string>());
    }

    /// <summary>
    /// Creates a failed job execution result with validation errors.
    /// </summary>
    /// <param name="jobId">The unique identifier of the job.</param>
    /// <param name="jobType">The type of the job.</param>
    /// <param name="validationErrors">A collection of validation errors that occurred during job execution.</param>
    /// <param name="taskResults">A dictionary of task results from all tasks in the job.</param>
    /// <param name="startedAt">The timestamp when the job execution started.</param>
    /// <param name="completedAt">The timestamp when the job execution completed.</param>
    /// <returns>A new <see cref="JobExecutionResult"/> instance representing a failed execution.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when any of the required parameters (jobId, jobType, validationErrors, taskResults) is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when jobId or jobType is empty or consists only of white-space characters.
    /// </exception>
    public static JobExecutionResult ValidationFailure(
        string jobId,
        string jobType,
        IReadOnlyCollection<string> validationErrors,
        IReadOnlyDictionary<string, ITaskResult> taskResults,
        DateTimeOffset startedAt,
        DateTimeOffset completedAt)
    {
        if (validationErrors == null)
            throw new ArgumentNullException(nameof(validationErrors));
        if (validationErrors.Count == 0)
            throw new ArgumentException("At least one validation error must be provided.", nameof(validationErrors));

        return new JobExecutionResult(
            jobId,
            jobType,
            isSuccess: false,
            status: JobStatus.ValidationFailed,
            errorMessage: "Validation failed",
            exception: null,
            startedAt,
            completedAt,
            taskResults,
            validationErrors);
    }

    /// <summary>
    /// Creates a successful job execution result with minimal parameters.
    /// </summary>
    /// <param name="details">Optional details about the successful execution.</param>
    /// <param name="startTime">The start time of the execution.</param>
    /// <param name="endTime">The end time of the execution.</param>
    /// <returns>A new <see cref="JobExecutionResult"/> instance representing a successful execution.</returns>
    public static JobExecutionResult Successful(string? details = null, DateTime? startTime = null, DateTime? endTime = null)
    {
        var now = DateTimeOffset.Now;
        var start = startTime.HasValue ? new DateTimeOffset(startTime.Value) : now;
        var end = endTime.HasValue ? new DateTimeOffset(endTime.Value) : now;

        return new JobExecutionResult(
            jobId: Guid.NewGuid().ToString(),
            jobType: "Unknown",
            isSuccess: true,
            status: JobStatus.Completed,
            errorMessage: null,
            exception: null,
            startedAt: start,
            completedAt: end,
            taskResults: new Dictionary<string, ITaskResult>(),
            validationErrors: Array.Empty<string>());
    }

    /// <summary>
    /// Creates a failed job execution result with minimal parameters.
    /// </summary>
    /// <param name="errorMessage">The error message describing the failure.</param>
    /// <param name="details">Optional additional details about the failure.</param>
    /// <param name="startTime">The start time of the execution.</param>
    /// <param name="endTime">The end time of the execution.</param>
    /// <returns>A new <see cref="JobExecutionResult"/> instance representing a failed execution.</returns>
    public static JobExecutionResult Failed(string errorMessage, string? details = null, DateTime? startTime = null, DateTime? endTime = null)
    {
        var now = DateTimeOffset.Now;
        var start = startTime.HasValue ? new DateTimeOffset(startTime.Value) : now;
        var end = endTime.HasValue ? new DateTimeOffset(endTime.Value) : now;

        return new JobExecutionResult(
            jobId: Guid.NewGuid().ToString(),
            jobType: "Unknown",
            isSuccess: false,
            status: JobStatus.Failed,
            errorMessage: errorMessage,
            exception: null,
            startedAt: start,
            completedAt: end,
            taskResults: new Dictionary<string, ITaskResult>(),
            validationErrors: Array.Empty<string>());
    }

    /// <summary>
    /// Creates a cancelled job execution result with minimal parameters.
    /// </summary>
    /// <param name="details">Optional details about the cancellation.</param>
    /// <param name="startTime">The start time of the execution.</param>
    /// <param name="endTime">The end time of the execution.</param>
    /// <returns>A new <see cref="JobExecutionResult"/> instance representing a cancelled execution.</returns>
    public static JobExecutionResult Cancelled(string? details = null, DateTime? startTime = null, DateTime? endTime = null)
    {
        var now = DateTimeOffset.Now;
        var start = startTime.HasValue ? new DateTimeOffset(startTime.Value) : now;
        var end = endTime.HasValue ? new DateTimeOffset(endTime.Value) : now;

        return new JobExecutionResult(
            jobId: Guid.NewGuid().ToString(),
            jobType: "Unknown",
            isSuccess: false,
            status: JobStatus.Cancelled,
            errorMessage: details ?? "Job execution was cancelled",
            exception: null,
            startedAt: start,
            completedAt: end,
            taskResults: new Dictionary<string, ITaskResult>(),
            validationErrors: Array.Empty<string>());
    }
} 