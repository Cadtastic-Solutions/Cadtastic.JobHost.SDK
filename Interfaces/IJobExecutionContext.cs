using Microsoft.Extensions.Logging;

namespace Cadtastic.JobHost.SDK.Interfaces;

/// <summary>
/// Context provided to job executors during execution
/// </summary>
public interface IJobExecutionContext
{
    /// <summary>
    /// The job being executed
    /// </summary>
    IJob Job { get; }

    /// <summary>
    /// Service provider for dependency injection
    /// </summary>
    IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Progress reporter for job execution updates
    /// </summary>
    IProgress<string>? Progress { get; }

    /// <summary>
    /// Cancellation token to allow job cancellation
    /// </summary>
    CancellationToken CancellationToken { get; }
}

/// <summary>
/// Extension methods for <see cref="IServiceProvider"/> to provide convenient access
/// to commonly used services in job execution contexts.
/// </summary>
public static class ServiceProviderExtensions
{
    /// <summary>
    /// Gets a logger instance for the specified type from the service provider.
    /// </summary>
    /// <typeparam name="T">The type to create a logger for.</typeparam>
    /// <param name="serviceProvider">The service provider instance.</param>
    /// <returns>A logger instance for the specified type, or null if logging is not configured.</returns>
    /// <exception cref="ArgumentNullException">Thrown when serviceProvider is null.</exception>
    public static ILogger<T>? GetLogger<T>(this IServiceProvider serviceProvider)
    {
        return serviceProvider == null
            ? throw new ArgumentNullException(nameof(serviceProvider))
            : serviceProvider.GetService(typeof(ILogger<T>)) as ILogger<T>;
    }

    /// <summary>
    /// Gets a logger instance for the specified category from the service provider.
    /// </summary>
    /// <param name="serviceProvider">The service provider instance.</param>
    /// <param name="categoryName">The category name for the logger.</param>
    /// <returns>A logger instance for the specified category, or null if logging is not configured.</returns>
    /// <exception cref="ArgumentNullException">Thrown when serviceProvider or categoryName is null.</exception>
    public static ILogger? GetLogger(this IServiceProvider serviceProvider, string categoryName)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        if (string.IsNullOrWhiteSpace(categoryName)) 
            throw new ArgumentException("Category name cannot be null or empty.", nameof(categoryName));
        
        var loggerFactory = serviceProvider.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
        return loggerFactory?.CreateLogger(categoryName);
    }

    /// <summary>
    /// Gets a logger instance for the specified type from the service provider.
    /// </summary>
    /// <param name="serviceProvider">The service provider instance.</param>
    /// <param name="type">The type to create a logger for.</param>
    /// <returns>A logger instance for the specified type, or null if logging is not configured.</returns>
    /// <exception cref="ArgumentNullException">Thrown when serviceProvider or type is null.</exception>
    public static ILogger? GetLogger(this IServiceProvider serviceProvider, Type type)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(type);
        
        var loggerFactory = serviceProvider.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
        return loggerFactory?.CreateLogger(type);
    }

    /// <summary>
    /// Gets a required service from the service provider, throwing an exception if not found.
    /// </summary>
    /// <typeparam name="T">The type of service to retrieve.</typeparam>
    /// <param name="serviceProvider">The service provider instance.</param>
    /// <returns>The requested service instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when serviceProvider is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the requested service is not registered.</exception>
    public static T GetRequiredService<T>(this IServiceProvider serviceProvider) where T : notnull
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        var service = serviceProvider.GetService(typeof(T));
        return service == null 
            ? throw new InvalidOperationException($"Service of type {typeof(T).Name} is not registered.") 
            : (T)service;
    }

    /// <summary>
    /// Safely gets a service from the service provider, returning null if not found.
    /// </summary>
    /// <typeparam name="T">The type of service to retrieve.</typeparam>
    /// <param name="serviceProvider">The service provider instance.</param>
    /// <returns>The requested service instance, or null if not found.</returns>
    /// <exception cref="ArgumentNullException">Thrown when serviceProvider is null.</exception>
    public static T? GetService<T>(this IServiceProvider serviceProvider) where T : class
    {
        return serviceProvider == null
            ? throw new ArgumentNullException(nameof(serviceProvider))
            : serviceProvider.GetService(typeof(T)) as T;
    }

    /// <summary>
    /// Gets all services of the specified type from the service provider.
    /// </summary>
    /// <typeparam name="T">The type of services to retrieve.</typeparam>
    /// <param name="serviceProvider">The service provider instance.</param>
    /// <returns>An enumerable of all registered services of the specified type.</returns>
    /// <exception cref="ArgumentNullException">Thrown when serviceProvider is null.</exception>
    public static IEnumerable<T> GetServices<T>(this IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        
        var services = serviceProvider.GetService(typeof(IEnumerable<T>)) as IEnumerable<T>;
        return services ?? Enumerable.Empty<T>();
    }
}

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