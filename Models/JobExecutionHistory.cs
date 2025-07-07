using Cadtastic.JobHost.SDK.Interfaces;

namespace Cadtastic.JobHost.SDK.Models;

/// <summary>
/// Represents the execution history of a job, including its tasks, results, and timing information.
/// This class provides a detailed record of how a job was executed and its outcomes.
/// </summary>
public class JobExecutionHistory : IJobExecutionHistory
{
    private readonly List<IJobExecutionResult> _results = new();

    /// <summary>
    /// Gets the unique identifier of the job.
    /// </summary>
    public string JobId { get; }

    /// <summary>
    /// Gets the type of the job.
    /// </summary>
    public string JobType { get; }

    /// <summary>
    /// Gets the name of the job.
    /// </summary>
    public string JobName { get; }

    /// <summary>
    /// Gets the status of the job execution.
    /// </summary>
    public JobStatus Status { get; }

    /// <summary>
    /// Gets the timestamp when the job execution started.
    /// </summary>
    public DateTimeOffset StartedAt { get; }

    /// <summary>
    /// Gets the timestamp when the job execution completed.
    /// </summary>
    public DateTimeOffset? CompletedAt { get; }

    /// <summary>
    /// Gets the duration of the job execution.
    /// </summary>
    public TimeSpan? Duration => CompletedAt.HasValue ? CompletedAt.Value - StartedAt : null;

    /// <summary>
    /// Gets the error message if the job execution failed, or null if it succeeded.
    /// </summary>
    public string? ErrorMessage { get; }

    /// <summary>
    /// Gets the exception that caused the job execution to fail, if any.
    /// </summary>
    public Exception? Exception { get; }

    /// <summary>
    /// Gets a collection of task execution histories for all tasks in the job.
    /// </summary>
    public IReadOnlyCollection<TaskExecutionHistory> TaskHistories { get; }

    /// <summary>
    /// Gets or sets the list of execution results for the job.
    /// </summary>
    public IList<IJobExecutionResult> Results
    {
        get => _results;
        set
        {
            _results.Clear();
            if (value != null)
                _results.AddRange(value);
        }
    }

    /// <summary>
    /// Gets or sets the total number of executions.
    /// </summary>
    public int TotalExecutions { get; set; }

    /// <summary>
    /// Gets or sets the total number of successful executions.
    /// </summary>
    public int TotalSucceeded { get; set; }

    /// <summary>
    /// Gets or sets the total number of failed executions.
    /// </summary>
    public int TotalFailed { get; set; }

    /// <summary>
    /// Gets the total number of successful executions.
    /// </summary>
    public int SuccessCount => _results.Count(r => r.State == ResultState.Successful);

    /// <summary>
    /// Gets the total number of failed executions.
    /// </summary>
    public int FailureCount => _results.Count(r => r.State == ResultState.Failed);

    /// <summary>
    /// Gets the total number of cancelled executions.
    /// </summary>
    public int CancelledCount => _results.Count(r => r.State == ResultState.Cancelled);

    /// <summary>
    /// Gets the average duration of successful executions.
    /// </summary>
    public TimeSpan AverageDuration
    {
        get
        {
            var successfulResults = _results.Where(r => r.State == ResultState.Successful).ToList();
            if (!successfulResults.Any())
                return TimeSpan.Zero;

            return TimeSpan.FromTicks((long)successfulResults.Average(r => r.Duration?.Ticks ?? 0));
        }
    }

    /// <summary>
    /// Gets the last execution result.
    /// </summary>
    public IJobExecutionResult? LastResult => _results.LastOrDefault();

    /// <summary>
    /// Gets the last successful execution result.
    /// </summary>
    public IJobExecutionResult? LastSuccessfulResult => _results.LastOrDefault(r => r.State == ResultState.Successful);

    /// <summary>
    /// Gets the last failed execution result.
    /// </summary>
    public IJobExecutionResult? LastFailedResult => _results.LastOrDefault(r => r.State == ResultState.Failed);

    /// <summary>
    /// Initializes a new instance of the <see cref="JobExecutionHistory"/> class.
    /// </summary>
    /// <param name="jobId">The unique identifier of the job.</param>
    /// <param name="jobType">The type of the job.</param>
    /// <param name="jobName">The name of the job.</param>
    /// <param name="status">The status of the job execution.</param>
    /// <param name="startedAt">The timestamp when the job execution started.</param>
    /// <param name="completedAt">The timestamp when the job execution completed, if it has completed.</param>
    /// <param name="errorMessage">The error message if the job execution failed.</param>
    /// <param name="exception">The exception that caused the job execution to fail.</param>
    /// <param name="taskHistories">A collection of task execution histories.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when any of the required parameters (jobId, jobType, jobName, taskHistories) is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when jobId or jobType is empty or consists only of white-space characters.
    /// </exception>
    public JobExecutionHistory(
        string jobId,
        string jobType,
        string jobName,
        JobStatus status,
        DateTimeOffset startedAt,
        DateTimeOffset? completedAt,
        string? errorMessage,
        Exception? exception,
        IReadOnlyCollection<TaskExecutionHistory> taskHistories)
    {
        if (string.IsNullOrWhiteSpace(jobId))
            throw new ArgumentException("Job ID cannot be null or empty.", nameof(jobId));
        if (string.IsNullOrWhiteSpace(jobType))
            throw new ArgumentException("Job type cannot be null or empty.", nameof(jobType));
        if (string.IsNullOrWhiteSpace(jobName))
            throw new ArgumentException("Job name cannot be null or empty.", nameof(jobName));
        if (taskHistories == null)
            throw new ArgumentNullException(nameof(taskHistories));

        JobId = jobId;
        JobType = jobType;
        JobName = jobName;
        Status = status;
        StartedAt = startedAt;
        CompletedAt = completedAt;
        ErrorMessage = errorMessage;
        Exception = exception;
        TaskHistories = taskHistories;
    }

    /// <summary>
    /// Adds an execution result to the history.
    /// </summary>
    /// <param name="result">The execution result to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when result is null.</exception>
    public void AddResult(IJobExecutionResult result)
    {
        if (result == null)
            throw new ArgumentNullException(nameof(result));

        _results.Add(result);
        TotalExecutions++;
        
        switch (result.State)
        {
            case ResultState.Successful:
                TotalSucceeded++;
                break;
            case ResultState.Failed:
            case ResultState.Cancelled:
            case ResultState.Unknown:
                TotalFailed++;
                break;
        }
    }
}

/// <summary>
/// Represents the execution history of a single task within a job.
/// </summary>
public class TaskExecutionHistory
{
    /// <summary>
    /// Gets the unique identifier of the task.
    /// </summary>
    public string TaskId { get; }

    /// <summary>
    /// Gets the type of the task.
    /// </summary>
    public Type TaskType { get; }

    /// <summary>
    /// Gets the status of the task execution.
    /// </summary>
    public TaskStatus Status { get; }

    /// <summary>
    /// Gets the timestamp when the task execution started.
    /// </summary>
    public DateTimeOffset StartedAt { get; }

    /// <summary>
    /// Gets the timestamp when the task execution completed.
    /// </summary>
    public DateTimeOffset? CompletedAt { get; }

    /// <summary>
    /// Gets the duration of the task execution.
    /// </summary>
    public TimeSpan? Duration => CompletedAt.HasValue ? CompletedAt.Value - StartedAt : null;

    /// <summary>
    /// Gets the error message if the task execution failed, or null if it succeeded.
    /// </summary>
    public string? ErrorMessage { get; }

    /// <summary>
    /// Gets the exception that caused the task execution to fail, if any.
    /// </summary>
    public Exception? Exception { get; }

    /// <summary>
    /// Gets the output data produced by the task execution, if any.
    /// </summary>
    public object? Output { get; }

    /// <summary>
    /// Gets a collection of validation errors that occurred during task execution, if any.
    /// </summary>
    public IReadOnlyCollection<string> ValidationErrors { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskExecutionHistory"/> class.
    /// </summary>
    /// <param name="taskId">The unique identifier of the task.</param>
    /// <param name="taskType">The type of the task.</param>
    /// <param name="status">The status of the task execution.</param>
    /// <param name="startedAt">The timestamp when the task execution started.</param>
    /// <param name="completedAt">The timestamp when the task execution completed, if it has completed.</param>
    /// <param name="errorMessage">The error message if the task execution failed.</param>
    /// <param name="exception">The exception that caused the task execution to fail.</param>
    /// <param name="output">The output data produced by the task execution.</param>
    /// <param name="validationErrors">A collection of validation errors that occurred during task execution.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when any of the required parameters (taskId, taskType, validationErrors) is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when taskId is empty or consists only of white-space characters.
    /// </exception>
    public TaskExecutionHistory(
        string taskId,
        Type taskType,
        TaskStatus status,
        DateTimeOffset startedAt,
        DateTimeOffset? completedAt,
        string? errorMessage,
        Exception? exception,
        object? output,
        IReadOnlyCollection<string> validationErrors)
    {
        if (string.IsNullOrWhiteSpace(taskId))
            throw new ArgumentException("Task ID cannot be null or empty.", nameof(taskId));
        if (taskType == null)
            throw new ArgumentNullException(nameof(taskType));
        if (validationErrors == null)
            throw new ArgumentNullException(nameof(validationErrors));

        TaskId = taskId;
        TaskType = taskType;
        Status = status;
        StartedAt = startedAt;
        CompletedAt = completedAt;
        ErrorMessage = errorMessage;
        Exception = exception;
        Output = output;
        ValidationErrors = validationErrors;
    }
} 