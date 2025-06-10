using Cadtastic.JobHost.SDK.Interfaces;

namespace Cadtastic.JobHost.SDK.Models;

/// <summary>
/// Result of job execution with factory methods for easy creation.
/// Implements <see cref="IJobExecutionResult"/> to provide consistent result handling.
/// </summary>
public class JobExecutionResult : IJobExecutionResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the job execution was successful.
    /// </summary>
    public bool IsSuccess { get; set; }
    
    /// <summary>
    /// Gets or sets the specific result state of the job execution.
    /// </summary>
    public ResultState State { get; set; }
    
    /// <summary>
    /// Gets or sets the error message if execution failed. Should be null for successful executions.
    /// </summary>
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// Gets or sets additional details about the execution, such as processing statistics or diagnostic information.
    /// </summary>
    public string? Details { get; set; }
    
    /// <summary>
    /// Gets or sets when the execution started.
    /// </summary>
    public DateTime StartTime { get; set; }
    
    /// <summary>
    /// Gets or sets when the execution ended.
    /// </summary>
    public DateTime EndTime { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="JobExecutionResult"/> class.
    /// </summary>
    public JobExecutionResult()
    {
        State = ResultState.Unknown;
        IsSuccess = false;
    }

    /// <summary>
    /// Creates a successful execution result with optional details and timing information.
    /// </summary>
    /// <param name="details">Optional details about the execution such as processing statistics.</param>
    /// <param name="startTime">When execution started. Uses current time if not specified.</param>
    /// <param name="endTime">When execution ended. Uses current time if not specified.</param>
    /// <returns>A successful <see cref="JobExecutionResult"/> instance.</returns>
    public static JobExecutionResult Successful(string? details = null, DateTime? startTime = null, DateTime? endTime = null)
    {
        var now = DateTime.Now;
        return new JobExecutionResult
        {
            IsSuccess = true,
            State = ResultState.Successful,
            Details = details,
            StartTime = startTime ?? now,
            EndTime = endTime ?? now
        };
    }

    /// <summary>
    /// Creates a failed execution result with error information and timing details.
    /// </summary>
    /// <param name="errorMessage">The error message describing what went wrong.</param>
    /// <param name="details">Optional additional details such as stack trace or diagnostic information.</param>
    /// <param name="startTime">When execution started. Uses current time if not specified.</param>
    /// <param name="endTime">When execution ended. Uses current time if not specified.</param>
    /// <returns>A failed <see cref="JobExecutionResult"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when errorMessage is null or empty.</exception>
    public static JobExecutionResult Failed(string errorMessage, string? details = null, DateTime? startTime = null, DateTime? endTime = null)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
            throw new ArgumentException("Error message cannot be null or empty for failed results.", nameof(errorMessage));
            
        var now = DateTime.Now;
        return new JobExecutionResult
        {
            IsSuccess = false,
            State = ResultState.Failed,
            ErrorMessage = errorMessage,
            Details = details,
            StartTime = startTime ?? now,
            EndTime = endTime ?? now
        };
    }

    /// <summary>
    /// Creates a cancelled execution result with optional details and timing information.
    /// </summary>
    /// <param name="details">Optional details about the cancellation.</param>
    /// <param name="startTime">When execution started. Uses current time if not specified.</param>
    /// <param name="endTime">When execution ended. Uses current time if not specified.</param>
    /// <returns>A cancelled <see cref="JobExecutionResult"/> instance.</returns>
    public static JobExecutionResult Cancelled(string? details = null, DateTime? startTime = null, DateTime? endTime = null)
    {
        var now = DateTime.Now;
        return new JobExecutionResult
        {
            IsSuccess = false,
            State = ResultState.Cancelled,
            ErrorMessage = "Job execution was cancelled",
            Details = details,
            StartTime = startTime ?? now,
            EndTime = endTime ?? now
        };
    }

    /// <summary>
    /// Creates an unknown state execution result with optional details and timing information.
    /// </summary>
    /// <param name="details">Optional details about why the state is unknown.</param>
    /// <param name="startTime">When execution started. Uses current time if not specified.</param>
    /// <param name="endTime">When execution ended. Uses current time if not specified.</param>
    /// <returns>An unknown state <see cref="JobExecutionResult"/> instance.</returns>
    public static JobExecutionResult Unknown(string? details = null, DateTime? startTime = null, DateTime? endTime = null)
    {
        var now = DateTime.Now;
        return new JobExecutionResult
        {
            IsSuccess = false,
            State = ResultState.Unknown,
            ErrorMessage = "Job execution result state is unknown",
            Details = details,
            StartTime = startTime ?? now,
            EndTime = endTime ?? now
        };
    }

    /// <summary>
    /// Creates a job execution result from a specific result state.
    /// </summary>
    /// <param name="state">The result state to create a result for.</param>
    /// <param name="details">Optional details about the execution.</param>
    /// <param name="errorMessage">Optional error message (used for failed/cancelled/unknown states).</param>
    /// <param name="startTime">When execution started. Uses current time if not specified.</param>
    /// <param name="endTime">When execution ended. Uses current time if not specified.</param>
    /// <returns>A <see cref="JobExecutionResult"/> instance with the specified state.</returns>
    public static JobExecutionResult FromState(ResultState state, string? details = null, string? errorMessage = null, DateTime? startTime = null, DateTime? endTime = null)
    {
        return state switch
        {
            ResultState.Successful => Successful(details, startTime, endTime),
            ResultState.Failed => Failed(errorMessage ?? "Job execution failed", details, startTime, endTime),
            ResultState.Cancelled => Cancelled(details, startTime, endTime),
            ResultState.Unknown => Unknown(details, startTime, endTime),
            _ => Unknown($"Invalid result state: {state}", startTime, endTime)
        };
    }
} 