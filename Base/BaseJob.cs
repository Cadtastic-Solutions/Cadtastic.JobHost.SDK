using System.Reflection;

using Cadtastic.JobHost.SDK.Attributes;
using Cadtastic.JobHost.SDK.Defaults;
using Cadtastic.JobHost.SDK.Interfaces;

using Microsoft.Extensions.DependencyInjection;

namespace Cadtastic.JobHost.SDK.Base;

/// <summary>
/// Base class for jobs that provides automatic task discovery via attributes.
/// Inherit from this class and mark methods with [Task] attribute for simple job implementation.
/// </summary>
public abstract partial class BaseJob : IJob
{
    private readonly JobAttribute? _jobAttribute;

    /// <inheritdoc/>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <inheritdoc/>
    public string Name { get; set; }

    /// <inheritdoc/>
    public string JobType { get; set; }

    /// <inheritdoc/>
    public virtual string Description => _jobAttribute?.Description ?? $"Job implementation for {GetType().Name}";

    /// <inheritdoc/>
    public virtual string Version => _jobAttribute?.Version ?? "1.0.0";

    /// <inheritdoc/>
    public JobState State { get; set; } = JobState.Inactive;

    /// <inheritdoc/>
    public object Configuration { get; set; } = new { };

    /// <inheritdoc/>
    public IJobExecutionHistory ExecutionHistory { get; set; } = null!;

    /// <inheritdoc/>
    public IServiceProvider JobServiceProvidor { get; set; } = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseJob"/> class.
    /// </summary>
    protected BaseJob()
    {
        var type = GetType();
        _jobAttribute = type.GetCustomAttribute<JobAttribute>();

        // Set default values from attribute or type
        JobType = _jobAttribute?.JobType ?? type.Name;
        Name = _jobAttribute?.Name ?? CreateFriendlyName(type.Name);
    }

    /// <summary>
    /// Gets the job executor that handles the execution of this job.
    /// By default, returns a DefaultJobExecutor that discovers tasks via reflection.
    /// Override this method to provide a custom executor.
    /// </summary>
    /// <returns>The job executor instance.</returns>
    public virtual IJobExecutor? GetExecutor()
    {
        return new DefaultJobExecutor(this);
    }

    /// <summary>
    /// Gets the job executor that handles the execution of this job with service provider support.
    /// By default, returns a DefaultJobExecutor that discovers tasks via reflection.
    /// Override this method to provide a custom executor.
    /// </summary>
    /// <param name="serviceProvider">The service provider for dependency injection.</param>
    /// <returns>The job executor instance.</returns>
    public virtual IJobExecutor? GetExecutor(IServiceProvider serviceProvider)
    {
        return new DefaultJobExecutor(this, serviceProvider);
    }

    /// <summary>
    /// Validates the job configuration.
    /// Override this method to provide custom validation logic.
    /// </summary>
    /// <returns>True if the job configuration is valid, false otherwise.</returns>
    public virtual bool ValidateConfiguration()
    {
        // Base implementation - override for custom validation
        return Configuration != null;
    }

    /// <summary>
    /// Registers job-specific services with the service collection.
    /// This method is called when the job is loaded to register its dependencies.
    /// Override this method in derived classes to register job-specific services.
    /// </summary>
    /// <param name="services">The service collection to register services with.</param>
    public virtual void RegisterJobServices(IServiceCollection services)
    {
        // Register the job instance itself
        services.AddSingleton(GetType(), this);
        services.AddSingleton<IJob>(this);
    }

    /// <summary>
    /// Creates a friendly name from a type name.
    /// </summary>
    /// <param name="typeName">The type name to convert.</param>
    /// <returns>A friendly name with spaces between words.</returns>
    private static string CreateFriendlyName(string typeName)
    {
        // Remove common suffixes
        var name = typeName;
        if (name.EndsWith("Job"))
            name = name.Substring(0, name.Length - 3);

        // Add spaces between words (simple camelCase handling)
        var result = camelCaseAddSpace().Replace(name, "$1 $2");

        return result.Trim();
    }

    [System.Text.RegularExpressions.GeneratedRegex("([a-z])([A-Z])")]
    private static partial System.Text.RegularExpressions.Regex camelCaseAddSpace();
}