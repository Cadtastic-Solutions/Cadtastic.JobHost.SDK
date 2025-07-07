using Cadtastic.JobHost.SDK.Interfaces;

namespace Cadtastic.JobHost.SDK.Extensions
{
    /// <summary>
    /// Provides extension methods for working with job execution results.
    /// These extensions add convenience methods for common operations on job execution results.
    /// </summary>
    public static class IJobExecutionResultExtensions
    {
        /// <summary>
        /// Checks if the execution result indicates a failure.
        /// </summary>
        /// <param name="result">The execution result to check.</param>
        /// <returns>True if the execution failed, false otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when result is null.</exception>
        public static bool IsFailed(this IJobExecutionResult result)
        {
            ArgumentNullException.ThrowIfNull(result);
            return result.State == ResultState.Failed;
        }

        /// <summary>
        /// Checks if the execution result indicates success.
        /// </summary>
        /// <param name="result">The execution result to check.</param>
        /// <returns>True if the execution was successful, false otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when result is null.</exception>
        public static bool IsSuccessful(this IJobExecutionResult result)
        {
            ArgumentNullException.ThrowIfNull(result);
            return result.State == ResultState.Successful;
        }

        /// <summary>
        /// Checks if the execution result indicates cancellation.
        /// </summary>
        /// <param name="result">The execution result to check.</param>
        /// <returns>True if the execution was cancelled, false otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when result is null.</exception>
        public static bool IsCancelled(this IJobExecutionResult result)
        {
            ArgumentNullException.ThrowIfNull(result);
            return result.State == ResultState.Cancelled;
        }

        /// <summary>
        /// Checks if the execution result state is unknown.
        /// </summary>
        /// <param name="result">The execution result to check.</param>
        /// <returns>True if the execution state is unknown, false otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when result is null.</exception>
        public static bool IsUnknown(this IJobExecutionResult result)
        {
            ArgumentNullException.ThrowIfNull(result);
            return result.State == ResultState.Unknown;
        }

        /// <summary>
        /// Gets a human-readable description of the execution result state.
        /// </summary>
        /// <param name="result">The execution result to get the state description for.</param>
        /// <returns>A descriptive string for the current result state.</returns>
        /// <exception cref="ArgumentNullException">Thrown when result is null.</exception>
        public static string GetStateDescription(this IJobExecutionResult result)
        {
            ArgumentNullException.ThrowIfNull(result);
            return result.State switch
            {
                ResultState.Failed => "The job execution failed.",
                ResultState.Successful => "The job execution completed successfully.",
                ResultState.Cancelled => "The job execution was cancelled.",
                ResultState.Unknown => "The job execution result is unknown.",
                _ => "Invalid result state."
            };
        }

        /// <summary>
        /// Gets a summary string including duration and state information.
        /// </summary>
        /// <param name="result">The execution result to summarize.</param>
        /// <returns>A formatted summary string of the execution result.</returns>
        /// <exception cref="ArgumentNullException">Thrown when result is null.</exception>
        public static string GetSummary(this IJobExecutionResult result)
        {
            ArgumentNullException.ThrowIfNull(result);
            var stateDescription = result.GetStateDescription();
            var duration = result.Duration?.TotalSeconds ?? 0;
            return $"{stateDescription} in {duration:F2} seconds.";
        }

        /// <summary>
        /// Determines if the execution result should be retried based on its state.
        /// </summary>
        /// <param name="result">The execution result to check.</param>
        /// <returns>True if the execution should be retried, false otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when result is null.</exception>
        public static bool ShouldRetry(this IJobExecutionResult result)
        {
            ArgumentNullException.ThrowIfNull(result);
            return result.State == ResultState.Failed || result.State == ResultState.Unknown;
        }

        /// <summary>
        /// Gets the total duration of all tasks in the job execution result.
        /// </summary>
        /// <param name="result">The job execution result to calculate task durations from.</param>
        /// <returns>The total duration of all tasks, or TimeSpan.Zero if no tasks were executed.</returns>
        /// <exception cref="ArgumentNullException">Thrown when result is null.</exception>
        public static TimeSpan GetTotalTaskDuration(this IJobExecutionResult result)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            return result.TaskResults.Values
                .Select(t => t.Duration)
                .Aggregate(TimeSpan.Zero, (total, duration) => total + duration);
        }

        /// <summary>
        /// Gets the average duration of all tasks in the job execution result.
        /// </summary>
        /// <param name="result">The job execution result to calculate average task duration from.</param>
        /// <returns>The average duration of all tasks, or TimeSpan.Zero if no tasks were executed.</returns>
        /// <exception cref="ArgumentNullException">Thrown when result is null.</exception>
        public static TimeSpan GetAverageTaskDuration(this IJobExecutionResult result)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            var taskResults = result.TaskResults.Values.ToList();
            if (!taskResults.Any())
                return TimeSpan.Zero;

            var totalDuration = taskResults.Sum(t => t.Duration.Ticks);
            return TimeSpan.FromTicks(totalDuration / taskResults.Count);
        }

        /// <summary>
        /// Gets the duration of the longest task in the job execution result.
        /// </summary>
        /// <param name="result">The job execution result to find the longest task duration from.</param>
        /// <returns>The duration of the longest task, or TimeSpan.Zero if no tasks were executed.</returns>
        /// <exception cref="ArgumentNullException">Thrown when result is null.</exception>
        public static TimeSpan GetLongestTaskDuration(this IJobExecutionResult result)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            return result.TaskResults.Values
                .Select(t => t.Duration)
                .DefaultIfEmpty(TimeSpan.Zero)
                .Max();
        }

        /// <summary>
        /// Gets the duration of the shortest task in the job execution result.
        /// </summary>
        /// <param name="result">The job execution result to find the shortest task duration from.</param>
        /// <returns>The duration of the shortest task, or TimeSpan.Zero if no tasks were executed.</returns>
        /// <exception cref="ArgumentNullException">Thrown when result is null.</exception>
        public static TimeSpan GetShortestTaskDuration(this IJobExecutionResult result)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            return result.TaskResults.Values
                .Select(t => t.Duration)
                .DefaultIfEmpty(TimeSpan.Zero)
                .Min();
        }

        /// <summary>
        /// Gets a collection of task results that failed during execution.
        /// </summary>
        /// <param name="result">The job execution result to get failed tasks from.</param>
        /// <returns>A collection of task results that failed, or an empty collection if no tasks failed.</returns>
        /// <exception cref="ArgumentNullException">Thrown when result is null.</exception>
        public static IEnumerable<ITaskResult> GetFailedTasks(this IJobExecutionResult result)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            return result.TaskResults.Values
                .Where(t => !t.IsSuccess);
        }

        /// <summary>
        /// Gets a collection of task results that succeeded during execution.
        /// </summary>
        /// <param name="result">The job execution result to get successful tasks from.</param>
        /// <returns>A collection of task results that succeeded, or an empty collection if no tasks succeeded.</returns>
        /// <exception cref="ArgumentNullException">Thrown when result is null.</exception>
        public static IEnumerable<ITaskResult> GetSuccessfulTasks(this IJobExecutionResult result)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            return result.TaskResults.Values
                .Where(t => t.IsSuccess);
        }

        /// <summary>
        /// Gets the number of tasks that failed during execution.
        /// </summary>
        /// <param name="result">The job execution result to count failed tasks from.</param>
        /// <returns>The number of tasks that failed.</returns>
        /// <exception cref="ArgumentNullException">Thrown when result is null.</exception>
        public static int GetFailedTaskCount(this IJobExecutionResult result)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            return result.TaskResults.Values
                .Count(t => !t.IsSuccess);
        }

        /// <summary>
        /// Gets the number of tasks that succeeded during execution.
        /// </summary>
        /// <param name="result">The job execution result to count successful tasks from.</param>
        /// <returns>The number of tasks that succeeded.</returns>
        /// <exception cref="ArgumentNullException">Thrown when result is null.</exception>
        public static int GetSuccessfulTaskCount(this IJobExecutionResult result)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            return result.TaskResults.Values
                .Count(t => t.IsSuccess);
        }

        /// <summary>
        /// Gets the total number of tasks in the job execution result.
        /// </summary>
        /// <param name="result">The job execution result to count tasks from.</param>
        /// <returns>The total number of tasks.</returns>
        /// <exception cref="ArgumentNullException">Thrown when result is null.</exception>
        public static int GetTotalTaskCount(this IJobExecutionResult result)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            return result.TaskResults.Count;
        }

        /// <summary>
        /// Gets the success rate of tasks in the job execution result as a percentage.
        /// </summary>
        /// <param name="result">The job execution result to calculate success rate from.</param>
        /// <returns>The success rate as a percentage (0-100), or 0 if no tasks were executed.</returns>
        /// <exception cref="ArgumentNullException">Thrown when result is null.</exception>
        public static double GetTaskSuccessRate(this IJobExecutionResult result)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            var totalTasks = result.GetTotalTaskCount();
            if (totalTasks == 0)
                return 0;

            var successfulTasks = result.GetSuccessfulTaskCount();
            return (double)successfulTasks / totalTasks * 100;
        }
    }
}
