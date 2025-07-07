using Cadtastic.JobHost.SDK.Interfaces;

namespace Cadtastic.JobHost.SDK.Extensions
{
    /// <summary>
    /// Provides extension methods for working with job execution contexts.
    /// These extensions add convenience methods for common operations on job execution contexts.
    /// </summary>
    public static class IJobExecutionContextExtensions
    {
        /// <summary>
        /// Gets a value indicating whether the job execution has been cancelled.
        /// </summary>
        /// <param name="context">The job execution context to check.</param>
        /// <returns>True if the job execution has been cancelled; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
        public static bool IsCancelled(this IJobExecutionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return context.CancellationToken.IsCancellationRequested;
        }

        /// <summary>
        /// Gets a value indicating whether the job execution has completed.
        /// </summary>
        /// <param name="context">The job execution context to check.</param>
        /// <returns>True if the job execution has completed; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
        public static bool IsCompleted(this IJobExecutionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return context.Status == JobStatus.Completed || 
                   context.Status == JobStatus.Failed || 
                   context.Status == JobStatus.Cancelled;
        }

        /// <summary>
        /// Gets a value indicating whether the job execution is in progress.
        /// </summary>
        /// <param name="context">The job execution context to check.</param>
        /// <returns>True if the job execution is in progress; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
        public static bool IsInProgress(this IJobExecutionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return context.Status == JobStatus.Running;
        }

        /// <summary>
        /// Gets a value indicating whether the job execution is waiting to start.
        /// </summary>
        /// <param name="context">The job execution context to check.</param>
        /// <returns>True if the job execution is waiting to start; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
        public static bool IsWaiting(this IJobExecutionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return context.Status == JobStatus.Waiting;
        }

        /// <summary>
        /// Gets a value indicating whether the job execution has failed.
        /// </summary>
        /// <param name="context">The job execution context to check.</param>
        /// <returns>True if the job execution has failed; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
        public static bool HasFailed(this IJobExecutionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return context.Status == JobStatus.Failed;
        }

        /// <summary>
        /// Gets a value indicating whether the job execution has succeeded.
        /// </summary>
        /// <param name="context">The job execution context to check.</param>
        /// <returns>True if the job execution has succeeded; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
        public static bool HasSucceeded(this IJobExecutionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return context.Status == JobStatus.Completed;
        }

        /// <summary>
        /// Gets the duration of the job execution so far.
        /// </summary>
        /// <param name="context">The job execution context to get duration from.</param>
        /// <returns>The duration of the job execution, or TimeSpan.Zero if not started.</returns>
        /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
        public static TimeSpan GetDuration(this IJobExecutionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (context.StartTime == default(DateTime))
                return TimeSpan.Zero;

            var endTime = context.EndTime ?? DateTime.UtcNow;
            return endTime - context.StartTime;
        }

        /// <summary>
        /// Gets a collection of task results that have completed.
        /// </summary>
        /// <param name="context">The job execution context to get completed tasks from.</param>
        /// <returns>A collection of completed task results.</returns>
        /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
        public static IEnumerable<ITaskResult> GetCompletedTasks(this IJobExecutionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return context.TaskResults.Values
                .Where(t => t.IsCompleted);
        }

        /// <summary>
        /// Gets a collection of task results that are still pending.
        /// </summary>
        /// <param name="context">The job execution context to get pending tasks from.</param>
        /// <returns>A collection of pending task results.</returns>
        /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
        public static IEnumerable<ITaskResult> GetPendingTasks(this IJobExecutionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return context.TaskResults.Values
                .Where(t => !t.IsCompleted);
        }

        /// <summary>
        /// Gets the number of tasks that have completed.
        /// </summary>
        /// <param name="context">The job execution context to count completed tasks from.</param>
        /// <returns>The number of completed tasks.</returns>
        /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
        public static int GetCompletedTaskCount(this IJobExecutionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return context.TaskResults.Values
                .Count(t => t.IsCompleted);
        }

        /// <summary>
        /// Gets the number of tasks that are still pending.
        /// </summary>
        /// <param name="context">The job execution context to count pending tasks from.</param>
        /// <returns>The number of pending tasks.</returns>
        /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
        public static int GetPendingTaskCount(this IJobExecutionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return context.TaskResults.Values
                .Count(t => !t.IsCompleted);
        }

        /// <summary>
        /// Gets the total number of tasks in the job execution context.
        /// </summary>
        /// <param name="context">The job execution context to count tasks from.</param>
        /// <returns>The total number of tasks.</returns>
        /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
        public static int GetTotalTaskCount(this IJobExecutionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return context.TaskResults.Count;
        }

        /// <summary>
        /// Gets the completion rate of tasks in the job execution context as a percentage.
        /// </summary>
        /// <param name="context">The job execution context to calculate completion rate from.</param>
        /// <returns>The completion rate as a percentage (0-100), or 0 if no tasks exist.</returns>
        /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
        public static double GetTaskCompletionRate(this IJobExecutionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var totalTasks = context.GetTotalTaskCount();
            if (totalTasks == 0)
                return 0;

            var completedTasks = context.GetCompletedTaskCount();
            return (double)completedTasks / totalTasks * 100;
        }

        /// <summary>
        /// Gets a typed logger for the specified type from the execution context.
        /// </summary>
        /// <typeparam name="T">The type to create a logger for.</typeparam>
        /// <param name="context">The execution context containing the service provider.</param>
        /// <returns>A logger instance for the specified type, or null if logging is not configured.</returns>
        /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
        public static ILogger<T>? GetLogger<T>(this IJobExecutionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return context.ServiceProvider.GetLogger<T>();
        }

        /// <summary>
        /// Gets a logger instance for the specified category from the execution context.
        /// </summary>
        /// <param name="context">The execution context containing the service provider.</param>
        /// <param name="categoryName">The category name for the logger.</param>
        /// <returns>A logger instance for the specified category, or null if logging is not configured.</returns>
        /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
        public static ILogger? GetLogger(this IJobExecutionContext context, string categoryName)
        {
            ArgumentNullException.ThrowIfNull(context);
            return context.ServiceProvider.GetLogger(categoryName);
        }

        /// <summary>
        /// Gets a required service from the execution context, throwing an exception if not found.
        /// </summary>
        /// <typeparam name="T">The type of service to retrieve.</typeparam>
        /// <param name="context">The execution context containing the service provider.</param>
        /// <returns>The requested service instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the requested service is not registered.</exception>
        public static T GetRequiredService<T>(this IJobExecutionContext context) where T : notnull
        {
            ArgumentNullException.ThrowIfNull(context);
            return context.ServiceProvider.GetRequiredService<T>();
        }

        /// <summary>
        /// Safely gets a service from the execution context, returning null if not found.
        /// </summary>
        /// <typeparam name="T">The type of service to retrieve.</typeparam>
        /// <param name="context">The execution context containing the service provider.</param>
        /// <returns>The requested service instance, or null if not found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
        public static T? GetService<T>(this IJobExecutionContext context) where T : class
        {
            ArgumentNullException.ThrowIfNull(context);
            return context.ServiceProvider.GetService<T>();
        }

        /// <summary>
        /// Reports progress with structured logging if available.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="message">The progress message to report.</param>
        /// <param name="logger">Optional logger for structured logging.</param>
        /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
        public static void ReportProgress(this IJobExecutionContext context, string message, ILogger? logger = null)
        {
            ArgumentNullException.ThrowIfNull(context);

            context.Progress?.Report(message);
            logger?.LogInformation("Job Progress - {JobId}: {Message}", context.Job.Id, message);
        }

        /// <summary>
        /// Reports progress with additional context data and structured logging.
        /// </summary>
        /// <typeparam name="T">The type of additional context data.</typeparam>
        /// <param name="context">The execution context.</param>
        /// <param name="message">The progress message to report.</param>
        /// <param name="data">Additional context data to include in structured logging.</param>
        /// <param name="logger">Optional logger for structured logging.</param>
        /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
        public static void ReportProgress<T>(this IJobExecutionContext context, string message, T data, ILogger? logger = null)
        {
            ArgumentNullException.ThrowIfNull(context);

            context.Progress?.Report(message);
            logger?.LogInformation("Job Progress - {JobId}: {Message} | Data: {@Data}", context.Job.Id, message, data);
        }
    }
}
