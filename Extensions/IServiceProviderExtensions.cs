using Microsoft.Extensions.Logging;

namespace Cadtastic.JobHost.SDK.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IServiceProvider"/> to provide convenient access
    /// to commonly used services in job execution contexts.
    /// </summary>
    public static class IServiceProviderExtensions
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
}
