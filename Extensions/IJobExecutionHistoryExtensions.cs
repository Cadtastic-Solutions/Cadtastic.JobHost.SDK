using Cadtastic.JobHost.SDK.Interfaces;

namespace Cadtastic.JobHost.SDK.Extensions
{
    /// <summary>
    /// Provides extension methods for working with job execution history.
    /// These extensions add convenience methods for analyzing and querying job execution history.
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

        /// <summary>
        /// Gets a collection of job execution results that occurred within the specified time range.
        /// </summary>
        /// <param name="history">The job execution history to query.</param>
        /// <param name="startTime">The start time of the range (inclusive).</param>
        /// <param name="endTime">The end time of the range (inclusive).</param>
        /// <returns>A collection of job execution results within the specified time range.</returns>
        /// <exception cref="ArgumentNullException">Thrown when history is null.</exception>
        /// <exception cref="ArgumentException">Thrown when endTime is before startTime.</exception>
        public static IEnumerable<IJobExecutionResult> GetResultsInTimeRange(
            this IJobExecutionHistory history,
            DateTimeOffset startTime,
            DateTimeOffset endTime)
        {
            if (history == null)
                throw new ArgumentNullException(nameof(history));
            if (endTime < startTime)
                throw new ArgumentException("End time must be after start time.", nameof(endTime));

            return history.Results
                .Where(r => r.StartedAt >= startTime && r.CompletedAt <= endTime);
        }

        /// <summary>
        /// Gets a collection of job execution results for a specific job type.
        /// </summary>
        /// <param name="history">The job execution history to query.</param>
        /// <param name="jobType">The type of job to get results for.</param>
        /// <returns>A collection of job execution results for the specified job type.</returns>
        /// <exception cref="ArgumentNullException">Thrown when history or jobType is null.</exception>
        /// <exception cref="ArgumentException">Thrown when jobType is empty.</exception>
        public static IEnumerable<IJobExecutionResult> GetResultsByJobType(
            this IJobExecutionHistory history,
            string jobType)
        {
            if (history == null)
                throw new ArgumentNullException(nameof(history));
            if (string.IsNullOrEmpty(jobType))
                throw new ArgumentException("Job type cannot be null or empty.", nameof(jobType));

            return history.Results
                .Where(r => r.JobType == jobType);
        }

        /// <summary>
        /// Gets the most recent job execution result.
        /// </summary>
        /// <param name="history">The job execution history to query.</param>
        /// <returns>The most recent job execution result, or null if no results exist.</returns>
        /// <exception cref="ArgumentNullException">Thrown when history is null.</exception>
        public static IJobExecutionResult? GetMostRecentResult(this IJobExecutionHistory history)
        {
            if (history == null)
                throw new ArgumentNullException(nameof(history));

            return history.Results
                .OrderByDescending(r => r.CompletedAt)
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets the most recent job execution result for a specific job type.
        /// </summary>
        /// <param name="history">The job execution history to query.</param>
        /// <param name="jobType">The type of job to get the most recent result for.</param>
        /// <returns>The most recent job execution result for the specified job type, or null if no results exist.</returns>
        /// <exception cref="ArgumentNullException">Thrown when history or jobType is null.</exception>
        /// <exception cref="ArgumentException">Thrown when jobType is empty.</exception>
        public static IJobExecutionResult? GetMostRecentResultByJobType(
            this IJobExecutionHistory history,
            string jobType)
        {
            if (history == null)
                throw new ArgumentNullException(nameof(history));
            if (string.IsNullOrEmpty(jobType))
                throw new ArgumentException("Job type cannot be null or empty.", nameof(jobType));

            return history.Results
                .Where(r => r.JobType == jobType)
                .OrderByDescending(r => r.CompletedAt)
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets the success rate of job executions for a specific job type as a percentage.
        /// </summary>
        /// <param name="history">The job execution history to calculate success rate from.</param>
        /// <param name="jobType">The type of job to calculate success rate for.</param>
        /// <returns>The success rate as a percentage (0-100), or 0 if no results exist.</returns>
        /// <exception cref="ArgumentNullException">Thrown when history or jobType is null.</exception>
        /// <exception cref="ArgumentException">Thrown when jobType is empty.</exception>
        public static double GetSuccessRateByJobType(
            this IJobExecutionHistory history,
            string jobType)
        {
            if (history == null)
                throw new ArgumentNullException(nameof(history));
            if (string.IsNullOrEmpty(jobType))
                throw new ArgumentException("Job type cannot be null or empty.", nameof(jobType));

            var results = history.Results
                .Where(r => r.JobType == jobType)
                .ToList();

            if (!results.Any())
                return 0;

            var successfulResults = results.Count(r => r.State == ResultState.Successful);
            return (double)successfulResults / results.Count * 100;
        }

        /// <summary>
        /// Gets the average duration of job executions.
        /// </summary>
        /// <param name="history">The job execution history to calculate average duration from.</param>
        /// <returns>The average duration of job executions, or TimeSpan.Zero if no results exist.</returns>
        /// <exception cref="ArgumentNullException">Thrown when history is null.</exception>
        public static TimeSpan GetAverageDuration(this IJobExecutionHistory history)
        {
            if (history == null)
                throw new ArgumentNullException(nameof(history));

            var results = history.Results.ToList();
            if (!results.Any())
                return TimeSpan.Zero;

            var totalDuration = results.Sum(r => r.Duration?.Ticks ?? 0);
            return TimeSpan.FromTicks(totalDuration / results.Count);
        }

        /// <summary>
        /// Gets the average duration of job executions for a specific job type.
        /// </summary>
        /// <param name="history">The job execution history to calculate average duration from.</param>
        /// <param name="jobType">The type of job to calculate average duration for.</param>
        /// <returns>The average duration of job executions, or TimeSpan.Zero if no results exist.</returns>
        /// <exception cref="ArgumentNullException">Thrown when history or jobType is null.</exception>
        /// <exception cref="ArgumentException">Thrown when jobType is empty.</exception>
        public static TimeSpan GetAverageDurationByJobType(
            this IJobExecutionHistory history,
            string jobType)
        {
            if (history == null)
                throw new ArgumentNullException(nameof(history));
            if (string.IsNullOrEmpty(jobType))
                throw new ArgumentException("Job type cannot be null or empty.", nameof(jobType));

            var results = history.Results
                .Where(r => r.JobType == jobType)
                .ToList();

            if (!results.Any())
                return TimeSpan.Zero;

            var totalDuration = results.Sum(r => r.Duration?.Ticks ?? 0);
            return TimeSpan.FromTicks(totalDuration / results.Count);
        }

        /// <summary>
        /// Gets the longest duration of any job execution.
        /// </summary>
        /// <param name="history">The job execution history to find the longest duration from.</param>
        /// <returns>The longest duration of any job execution, or TimeSpan.Zero if no results exist.</returns>
        /// <exception cref="ArgumentNullException">Thrown when history is null.</exception>
        public static TimeSpan GetLongestDuration(this IJobExecutionHistory history)
        {
            if (history == null)
                throw new ArgumentNullException(nameof(history));

            return history.Results
                .Select(r => r.Duration ?? TimeSpan.Zero)
                .DefaultIfEmpty(TimeSpan.Zero)
                .Max();
        }

        /// <summary>
        /// Gets the shortest duration of any job execution.
        /// </summary>
        /// <param name="history">The job execution history to find the shortest duration from.</param>
        /// <returns>The shortest duration of any job execution, or TimeSpan.Zero if no results exist.</returns>
        /// <exception cref="ArgumentNullException">Thrown when history is null.</exception>
        public static TimeSpan GetShortestDuration(this IJobExecutionHistory history)
        {
            if (history == null)
                throw new ArgumentNullException(nameof(history));

            return history.Results
                .Select(r => r.Duration ?? TimeSpan.Zero)
                .DefaultIfEmpty(TimeSpan.Zero)
                .Min();
        }

        /// <summary>
        /// Gets the total number of job executions.
        /// </summary>
        /// <param name="history">The job execution history to count executions from.</param>
        /// <returns>The total number of job executions.</returns>
        /// <exception cref="ArgumentNullException">Thrown when history is null.</exception>
        public static int GetTotalExecutionCount(this IJobExecutionHistory history)
        {
            if (history == null)
                throw new ArgumentNullException(nameof(history));

            return history.Results.Count;
        }
    }
}
