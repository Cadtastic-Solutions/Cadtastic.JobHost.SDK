using Cadtastic.JobHost.SDK.Interfaces;

using Microsoft.Extensions.Logging;

namespace Cadtastic.JobHost.SDK.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IJobExecutionContext"/> to provide convenient access
    /// to commonly used services and functionality.
    /// </summary>
    public static class IJobExecutionContextExtensions
    {
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
