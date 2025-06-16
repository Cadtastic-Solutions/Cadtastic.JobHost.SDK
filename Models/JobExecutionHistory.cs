using Cadtastic.JobHost.SDK.Interfaces;

namespace Cadtastic.JobHost.SDK.Models;

/// <summary>
/// Represents the execution history of a job.
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
    /// <exception cref="ArgumentNullException">Thrown when jobId, jobType, or jobName is null or empty.</exception>
    public JobExecutionHistory(string jobId, string jobType, string jobName)
    {
        if (string.IsNullOrEmpty(jobId))
            throw new ArgumentNullException(nameof(jobId));
        if (string.IsNullOrEmpty(jobType))
            throw new ArgumentNullException(nameof(jobType));
        if (string.IsNullOrEmpty(jobName))
            throw new ArgumentNullException(nameof(jobName));

        JobId = jobId;
        JobType = jobType;
        JobName = jobName;
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