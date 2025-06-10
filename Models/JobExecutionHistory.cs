using Cadtastic.JobHost.SDK.Interfaces;

namespace Cadtastic.JobHost.SDK.Models;

/// <summary>
/// Represents job execution history with statistics and execution results.
/// Implements <see cref="IJobExecutionHistory"/> to provide consistent history tracking.
/// </summary>
public class JobExecutionHistory : IJobExecutionHistory
{
    /// <summary>
    /// Gets or sets the collection of execution history records for jobs.
    /// Contains all execution results in chronological order.
    /// </summary>
    public IEnumerable<IJobExecutionResult> Results { get; set; } = new List<IJobExecutionResult>();

    /// <summary>
    /// Gets or sets the total number of job executions tracked in this history.
    /// </summary>
    public int TotalExecutions { get; set; }

    /// <summary>
    /// Gets or sets the total number of successful job executions.
    /// </summary>
    public int TotalSucceeded { get; set; }

    /// <summary>
    /// Gets or sets the total number of failed job executions.
    /// </summary>
    public int TotalFailed { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="JobExecutionHistory"/> class with empty history.
    /// </summary>
    public JobExecutionHistory()
    {
        Results = new List<IJobExecutionResult>();
        TotalExecutions = 0;
        TotalSucceeded = 0;
        TotalFailed = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JobExecutionHistory"/> class with existing results.
    /// </summary>
    /// <param name="existingResults">Existing execution results to initialize the history with.</param>
    /// <exception cref="ArgumentNullException">Thrown when existingResults is null.</exception>
    public JobExecutionHistory(IEnumerable<IJobExecutionResult> existingResults)
    {
        ArgumentNullException.ThrowIfNull(existingResults);
        
        Results = new List<IJobExecutionResult>(existingResults);
        TotalExecutions = Results.Count();
        TotalSucceeded = Results.Count(r => r.State == ResultState.Successful);
        TotalFailed = Results.Count(r => r.State != ResultState.Successful);
    }

    /// <summary>
    /// Creates a new empty job execution history instance.
    /// </summary>
    /// <returns>A new <see cref="JobExecutionHistory"/> instance with no execution records.</returns>
    public static JobExecutionHistory Empty()
    {
        return new JobExecutionHistory();
    }

    /// <summary>
    /// Creates a new job execution history instance from existing results.
    /// </summary>
    /// <param name="results">The execution results to create history from.</param>
    /// <returns>A new <see cref="JobExecutionHistory"/> instance populated with the provided results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when results is null.</exception>
    public static JobExecutionHistory FromResults(IEnumerable<IJobExecutionResult> results)
    {
        return new JobExecutionHistory(results);
    }
} 