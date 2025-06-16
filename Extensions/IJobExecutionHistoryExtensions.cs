using Cadtastic.JobHost.SDK.Interfaces;

namespace Cadtastic.JobHost.SDK.Extensions
{

    /// <summary>
    /// Extension methods for <see cref="IJobExecutionHistory"/> to provide convenient access
    /// to history management and statistical operations.
    /// </summary>
    public static class IJobExecutionHistoryExtensions
    {
        /// <summary>
        /// Adds a new execution result to the history and updates statistics.
        /// </summary>
        /// <param name="history">The job execution history instance.</param>
        /// <param name="result">The execution result to add to the history.</param>
        /// <exception cref="ArgumentNullException">Thrown when history or result is null.</exception>
        public static void AddExecutionResult(this IJobExecutionHistory history, IJobExecutionResult result)
        {
            ArgumentNullException.ThrowIfNull(history);
            ArgumentNullException.ThrowIfNull(result);

            history.Results ??= [];
            ((List<IJobExecutionResult>)history.Results).Add(result);
            history.TotalExecutions++;

            switch (result.State)
            {
                case ResultState.Successful:
                    history.TotalSucceeded++;
                    break;
                case ResultState.Failed:
                case ResultState.Cancelled:
                case ResultState.Unknown:
                    history.TotalFailed++;
                    break;
            }
        }

        /// <summary>
        /// Gets all successful execution results from the history.
        /// </summary>
        /// <param name="history">The job execution history instance.</param>
        /// <returns>An enumerable of successful execution results.</returns>
        /// <exception cref="ArgumentNullException">Thrown when history is null.</exception>
        public static IEnumerable<IJobExecutionResult> GetSuccessfulResults(this IJobExecutionHistory history)
        {
            ArgumentNullException.ThrowIfNull(history);
            return history.Results?.Where(r => r.State == ResultState.Successful) ?? Enumerable.Empty<IJobExecutionResult>();
        }

        /// <summary>
        /// Gets all failed execution results from the history.
        /// </summary>
        /// <param name="history">The job execution history instance.</param>
        /// <returns>An enumerable of failed execution results.</returns>
        /// <exception cref="ArgumentNullException">Thrown when history is null.</exception>
        public static IEnumerable<IJobExecutionResult> GetFailedResults(this IJobExecutionHistory history)
        {
            ArgumentNullException.ThrowIfNull(history);
            return history.Results?.Where(r => r.State == ResultState.Failed) ?? Enumerable.Empty<IJobExecutionResult>();
        }

        /// <summary>
        /// Gets all cancelled execution results from the history.
        /// </summary>
        /// <param name="history">The job execution history instance.</param>
        /// <returns>An enumerable of cancelled execution results.</returns>
        /// <exception cref="ArgumentNullException">Thrown when history is null.</exception>
        public static IEnumerable<IJobExecutionResult> GetCancelledResults(this IJobExecutionHistory history)
        {
            ArgumentNullException.ThrowIfNull(history);
            return history.Results?.Where(r => r.State == ResultState.Cancelled) ?? Enumerable.Empty<IJobExecutionResult>();
        }

        /// <summary>
        /// Gets execution results filtered by the specified result state.
        /// </summary>
        /// <param name="history">The job execution history instance.</param>
        /// <param name="state">The result state to filter by.</param>
        /// <returns>An enumerable of execution results with the specified state.</returns>
        /// <exception cref="ArgumentNullException">Thrown when history is null.</exception>
        public static IEnumerable<IJobExecutionResult> GetResultsByState(this IJobExecutionHistory history, ResultState state)
        {
            ArgumentNullException.ThrowIfNull(history);
            return history.Results?.Where(r => r.State == state) ?? Enumerable.Empty<IJobExecutionResult>();
        }

        /// <summary>
        /// Gets the most recent execution result from the history.
        /// </summary>
        /// <param name="history">The job execution history instance.</param>
        /// <returns>The last execution result.</returns>
        /// <exception cref="ArgumentNullException">Thrown when history is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no execution results are found.</exception>
        public static IJobExecutionResult GetLastExecutionResult(this IJobExecutionHistory history)
        {
            ArgumentNullException.ThrowIfNull(history);
            return history.Results?.LastOrDefault() ?? throw new InvalidOperationException("No execution results found.");
        }

        /// <summary>
        /// Gets the count of failed executions from the history results.
        /// </summary>
        /// <param name="history">The job execution history instance.</param>
        /// <returns>The number of failed executions.</returns>
        /// <exception cref="ArgumentNullException">Thrown when history is null.</exception>
        public static int GetFailedExecutionCount(this IJobExecutionHistory history)
        {
            ArgumentNullException.ThrowIfNull(history);
            return history.Results?.Count(r => r.State == ResultState.Failed) ?? 0;
        }

        /// <summary>
        /// Gets the count of successful executions from the history results.
        /// </summary>
        /// <param name="history">The job execution history instance.</param>
        /// <returns>The number of successful executions.</returns>
        /// <exception cref="ArgumentNullException">Thrown when history is null.</exception>
        public static int GetSuccessfulExecutionCount(this IJobExecutionHistory history)
        {
            ArgumentNullException.ThrowIfNull(history);
            return history.Results?.Count(r => r.State == ResultState.Successful) ?? 0;
        }

        /// <summary>
        /// Gets the count of cancelled executions from the history results.
        /// </summary>
        /// <param name="history">The job execution history instance.</param>
        /// <returns>The number of cancelled executions.</returns>
        /// <exception cref="ArgumentNullException">Thrown when history is null.</exception>
        public static int GetCancelledExecutionCount(this IJobExecutionHistory history)
        {
            ArgumentNullException.ThrowIfNull(history);
            return history.Results?.Count(r => r.State == ResultState.Cancelled) ?? 0;
        }

        /// <summary>
        /// Gets the count of executions with the specified result state.
        /// </summary>
        /// <param name="history">The job execution history instance.</param>
        /// <param name="state">The result state to count.</param>
        /// <returns>The number of executions with the specified state.</returns>
        /// <exception cref="ArgumentNullException">Thrown when history is null.</exception>
        public static int GetExecutionCountByState(this IJobExecutionHistory history, ResultState state)
        {
            ArgumentNullException.ThrowIfNull(history);
            return history.Results?.Count(r => r.State == state) ?? 0;
        }

        /// <summary>
        /// Gets the average execution duration for all completed jobs.
        /// </summary>
        /// <param name="history">The job execution history instance.</param>
        /// <returns>The average execution duration, or TimeSpan.Zero if no results exist.</returns>
        /// <exception cref="ArgumentNullException">Thrown when history is null.</exception>
        public static TimeSpan GetAverageExecutionDuration(this IJobExecutionHistory history)
        {
            ArgumentNullException.ThrowIfNull(history);

            if (history.Results?.Any() != true)
                return TimeSpan.Zero;

            var totalTicks = history.Results.Sum(r => r.Duration?.Ticks ?? 0);
            var averageTicks = (long)(totalTicks / history.Results.Count());
            return new TimeSpan(averageTicks);
        }

        /// <summary>
        /// Gets the success rate as a percentage (0-100).
        /// </summary>
        /// <param name="history">The job execution history instance.</param>
        /// <returns>The success rate as a percentage, or 0 if no executions exist.</returns>
        /// <exception cref="ArgumentNullException">Thrown when history is null.</exception>
        public static double GetSuccessRate(this IJobExecutionHistory history)
        {
            ArgumentNullException.ThrowIfNull(history);

            if (history.TotalExecutions == 0)
                return 0.0;

            return (double)history.TotalSucceeded / history.TotalExecutions * 100.0;
        }
    }
}
