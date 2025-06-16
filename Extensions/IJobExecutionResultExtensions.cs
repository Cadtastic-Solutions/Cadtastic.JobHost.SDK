using Cadtastic.JobHost.SDK.Interfaces;

namespace Cadtastic.JobHost.SDK.Extensions
{

    /// <summary>
    /// Extension methods for <see cref="IJobExecutionResult"/> to provide convenient access
    /// to result state analysis and validation.
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
    }
}
