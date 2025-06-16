using Cadtastic.JobHost.SDK.Interfaces;

namespace Cadtastic.JobHost.SDK.Extensions
{

    /// <summary>
    /// Extension methods for <see cref="IJob"/> to provide convenient access
    /// to job state management and validation.
    /// </summary>
    public static class IJobExtensions
    {
        /// <summary>
        /// Checks if the job is currently in a state that allows execution.
        /// </summary>
        /// <param name="job">The job to check.</param>
        /// <returns>True if the job can be executed, false otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when job is null.</exception>
        public static bool CanBeExecuted(this IJob job)
        {
            ArgumentNullException.ThrowIfNull(job);
            return job.State == JobState.Active;
        }

        /// <summary>
        /// Checks if the job is currently being processed.
        /// </summary>
        /// <param name="job">The job to check.</param>
        /// <returns>True if the job is currently processing, false otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when job is null.</exception>
        public static bool IsProcessing(this IJob job)
        {
            ArgumentNullException.ThrowIfNull(job);
            return job.State == JobState.Processing;
        }

        /// <summary>
        /// Sets the job state to Processing and returns the previous state.
        /// </summary>
        /// <param name="job">The job to update.</param>
        /// <returns>The previous job state.</returns>
        /// <exception cref="ArgumentNullException">Thrown when job is null.</exception>
        public static JobState SetToProcessing(this IJob job)
        {
            ArgumentNullException.ThrowIfNull(job);
            var previousState = job.State;
            job.State = JobState.Processing;
            return previousState;
        }

        /// <summary>
        /// Sets the job state to Active.
        /// </summary>
        /// <param name="job">The job to update.</param>
        /// <exception cref="ArgumentNullException">Thrown when job is null.</exception>
        public static void SetToActive(this IJob job)
        {
            ArgumentNullException.ThrowIfNull(job);
            job.State = JobState.Active;
        }

        /// <summary>
        /// Sets the job state to Inactive.
        /// </summary>
        /// <param name="job">The job to update.</param>
        /// <exception cref="ArgumentNullException">Thrown when job is null.</exception>
        public static void SetToInactive(this IJob job)
        {
            ArgumentNullException.ThrowIfNull(job);
            job.State = JobState.Inactive;
        }

        /// <summary>
        /// Gets a human-readable description of the current job state.
        /// </summary>
        /// <param name="job">The job to get the state description for.</param>
        /// <returns>A descriptive string for the current job state.</returns>
        /// <exception cref="ArgumentNullException">Thrown when job is null.</exception>
        public static string GetStateDescription(this IJob job)
        {
            ArgumentNullException.ThrowIfNull(job);
            return job.State switch
            {
                JobState.Inactive => "The job is not currently active.",
                JobState.Active => "The job is ready for execution.",
                JobState.Processing => "The job is currently being executed.",
                JobState.Unknown => "The job state cannot be determined.",
                _ => "Invalid job state."
            };
        }
    }
}
