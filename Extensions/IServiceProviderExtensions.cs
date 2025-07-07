using Cadtastic.JobHost.SDK.Interfaces;

namespace Cadtastic.JobHost.SDK.Extensions;

/// <summary>
/// Provides extension methods for working with service providers.
/// These extensions add convenience methods for common operations on service providers.
/// </summary>
public static class IServiceProviderExtensions
{
    /// <summary>
    /// Gets a logger for the specified type.
    /// </summary>
    /// <typeparam name="T">The type to get a logger for.</typeparam>
    /// <param name="serviceProvider">The service provider to get the logger from.</param>
    /// <returns>A logger for the specified type.</returns>
    /// <exception cref="ArgumentNullException">Thrown when serviceProvider is null.</exception>
    public static ILogger<T> GetLogger<T>(this IServiceProvider serviceProvider)
    {
        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));

        return serviceProvider.GetRequiredService<ILogger<T>>();
    }

    /// <summary>
    /// Gets a logger for the specified category name.
    /// </summary>
    /// <param name="serviceProvider">The service provider to get the logger from.</param>
    /// <param name="categoryName">The category name for the logger.</param>
    /// <returns>A logger for the specified category name.</returns>
    /// <exception cref="ArgumentNullException">Thrown when serviceProvider is null.</exception>
    /// <exception cref="ArgumentException">Thrown when categoryName is null or empty.</exception>
    public static ILogger GetLogger(this IServiceProvider serviceProvider, string categoryName)
    {
        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));
        if (string.IsNullOrEmpty(categoryName))
            throw new ArgumentException("Category name cannot be null or empty.", nameof(categoryName));

        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        return loggerFactory.CreateLogger(categoryName);
    }

    /// <summary>
    /// Gets a logger for the specified type and category.
    /// </summary>
    /// <typeparam name="T">The type to get a logger for.</typeparam>
    /// <param name="serviceProvider">The service provider to get the logger from.</param>
    /// <param name="category">The category to use for the logger.</param>
    /// <returns>A logger for the specified type and category.</returns>
    /// <exception cref="ArgumentNullException">Thrown when serviceProvider is null.</exception>
    /// <exception cref="ArgumentException">Thrown when category is null or empty.</exception>
    public static ILogger<T> GetLogger<T>(this IServiceProvider serviceProvider, string category)
    {
        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));
        if (string.IsNullOrEmpty(category))
            throw new ArgumentException("Category cannot be null or empty.", nameof(category));

        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        return (ILogger<T>)loggerFactory.CreateLogger(category);
    }

    /// <summary>
    /// Gets a logger for the specified type and category.
    /// </summary>
    /// <typeparam name="T">The type to get a logger for.</typeparam>
    /// <param name="serviceProvider">The service provider to get the logger from.</param>
    /// <param name="category">The category to use for the logger.</param>
    /// <param name="subCategory">The sub-category to use for the logger.</param>
    /// <returns>A logger for the specified type and category.</returns>
    /// <exception cref="ArgumentNullException">Thrown when serviceProvider is null.</exception>
    /// <exception cref="ArgumentException">Thrown when category or subCategory is null or empty.</exception>
    public static ILogger<T> GetLogger<T>(this IServiceProvider serviceProvider, string category, string subCategory)
    {
        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));
        if (string.IsNullOrEmpty(category))
            throw new ArgumentException("Category cannot be null or empty.", nameof(category));
        if (string.IsNullOrEmpty(subCategory))
            throw new ArgumentException("Sub-category cannot be null or empty.", nameof(subCategory));

        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        return (ILogger<T>)loggerFactory.CreateLogger($"{category}.{subCategory}");
    }

    /// <summary>
    /// Gets a logger for the specified type and categories.
    /// </summary>
    /// <typeparam name="T">The type to get a logger for.</typeparam>
    /// <param name="serviceProvider">The service provider to get the logger from.</param>
    /// <param name="categories">The categories to use for the logger.</param>
    /// <returns>A logger for the specified type and categories.</returns>
    /// <exception cref="ArgumentNullException">Thrown when serviceProvider or categories is null.</exception>
    /// <exception cref="ArgumentException">Thrown when categories is empty or contains null or empty strings.</exception>
    public static ILogger<T> GetLogger<T>(this IServiceProvider serviceProvider, IEnumerable<string> categories)
    {
        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));
        if (categories == null)
            throw new ArgumentNullException(nameof(categories));
        if (!categories.Any())
            throw new ArgumentException("Categories cannot be empty.", nameof(categories));
        if (categories.Any(string.IsNullOrEmpty))
            throw new ArgumentException("Categories cannot contain null or empty strings.", nameof(categories));

        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        return (ILogger<T>)loggerFactory.CreateLogger(string.Join(".", categories));
    }

    /// <summary>
    /// Gets a logger for the specified type and categories.
    /// </summary>
    /// <typeparam name="T">The type to get a logger for.</typeparam>
    /// <param name="serviceProvider">The service provider to get the logger from.</param>
    /// <param name="categories">The categories to use for the logger.</param>
    /// <returns>A logger for the specified type and categories.</returns>
    /// <exception cref="ArgumentNullException">Thrown when serviceProvider or categories is null.</exception>
    /// <exception cref="ArgumentException">Thrown when categories is empty or contains null or empty strings.</exception>
    public static ILogger<T> GetLogger<T>(this IServiceProvider serviceProvider, params string[] categories)
    {
        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));
        if (categories == null)
            throw new ArgumentNullException(nameof(categories));
        if (categories.Length == 0)
            throw new ArgumentException("Categories cannot be empty.", nameof(categories));
        if (categories.Any(string.IsNullOrEmpty))
            throw new ArgumentException("Categories cannot contain null or empty strings.", nameof(categories));

        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        return (ILogger<T>)loggerFactory.CreateLogger(string.Join(".", categories));
    }

    /// <summary>
    /// Gets a logger for the specified type and job.
    /// </summary>
    /// <typeparam name="T">The type to get a logger for.</typeparam>
    /// <param name="serviceProvider">The service provider to get the logger from.</param>
    /// <param name="job">The job to use for the logger.</param>
    /// <returns>A logger for the specified type and job.</returns>
    /// <exception cref="ArgumentNullException">Thrown when serviceProvider or job is null.</exception>
    public static ILogger<T> GetLogger<T>(this IServiceProvider serviceProvider, IJob job)
    {
        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));
        if (job == null)
            throw new ArgumentNullException(nameof(job));

        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        return (ILogger<T>)loggerFactory.CreateLogger($"Job.{job.JobType}");
    }

    /// <summary>
    /// Gets a logger for the specified type and task.
    /// </summary>
    /// <typeparam name="T">The type to get a logger for.</typeparam>
    /// <param name="serviceProvider">The service provider to get the logger from.</param>
    /// <param name="task">The task to use for the logger.</param>
    /// <returns>A logger for the specified type and task.</returns>
    /// <exception cref="ArgumentNullException">Thrown when serviceProvider or task is null.</exception>
    public static ILogger<T> GetLogger<T>(this IServiceProvider serviceProvider, ITaskExecutor task)
    {
        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));
        if (task == null)
            throw new ArgumentNullException(nameof(task));

        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        return (ILogger<T>)loggerFactory.CreateLogger($"Task.{task.TaskId}");
    }

    /// <summary>
    /// Gets a logger for the specified type, job, and task.
    /// </summary>
    /// <typeparam name="T">The type to get a logger for.</typeparam>
    /// <param name="serviceProvider">The service provider to get the logger from.</param>
    /// <param name="job">The job to use for the logger.</param>
    /// <param name="task">The task to use for the logger.</param>
    /// <returns>A logger for the specified type, job, and task.</returns>
    /// <exception cref="ArgumentNullException">Thrown when serviceProvider, job, or task is null.</exception>
    public static ILogger<T> GetLogger<T>(this IServiceProvider serviceProvider, IJob job, ITaskExecutor task)
    {
        if (serviceProvider == null)
            throw new ArgumentNullException(nameof(serviceProvider));
        if (job == null)
            throw new ArgumentNullException(nameof(job));
        if (task == null)
            throw new ArgumentNullException(nameof(task));

        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        return (ILogger<T>)loggerFactory.CreateLogger($"Job.{job.JobType}.Task.{task.TaskId}");
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
