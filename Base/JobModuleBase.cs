using Cadtastic.JobHost.SDK.Interfaces;

using Microsoft.Extensions.DependencyInjection;

namespace Cadtastic.JobHost.SDK.Base;

/// <summary>
/// Base class for job modules that provides common functionality.
/// This class implements IJobModule and should be inherited by specific job module implementations.
/// </summary>
public abstract class JobModuleBase : IJobModule
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<JobModuleBase> _logger;
    private readonly List<ITaskExecutor> _taskExecutors;

    /// <summary>
    /// Initializes a new instance of the JobModuleBase class.
    /// </summary>
    /// <param name="serviceProvider">The service provider for dependency injection.</param>
    /// <exception cref="ArgumentNullException">Thrown when serviceProvider is null.</exception>
    protected JobModuleBase(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = serviceProvider.GetRequiredService<ILogger<JobModuleBase>>();
        _taskExecutors = new List<ITaskExecutor>();
        
        InitializeTaskExecutors();
    }

    /// <summary>
    /// Gets the service provider for dependency injection.
    /// </summary>
    protected IServiceProvider ServiceProvider => _serviceProvider;

    /// <summary>
    /// Gets the logger for this job module.
    /// </summary>
    protected ILogger<JobModuleBase> Logger => _logger;

    /// <summary>
    /// Gets the name of the job module.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Gets the description of the job module.
    /// </summary>
    public abstract string Description { get; }

    /// <summary>
    /// Gets the version of the job module.
    /// </summary>
    public abstract string Version { get; }

    /// <summary>
    /// Gets the job types supported by this module.
    /// </summary>
    public abstract string[] SupportedJobTypes { get; }

    /// <summary>
    /// Gets the unique identifier for this job module (primary job type).
    /// </summary>
    public abstract string JobType { get; }

    /// <summary>
    /// Gets the types of task executors that this job module exposes.
    /// </summary>
    public abstract Type[] ExecutorTypes { get; }

    /// <summary>
    /// Gets the task executors that make up this job.
    /// </summary>
    public IEnumerable<ITaskExecutor> TaskExecutors => _taskExecutors.AsReadOnly();

    /// <summary>
    /// Validates the job configuration.
    /// </summary>
    /// <param name="job">The job to validate.</param>
    /// <returns>True if the job configuration is valid, false otherwise.</returns>
    public virtual bool ValidateJob(IJob job)
    {
        if (job == null)
            return false;
            
        return SupportedJobTypes.Contains(job.JobType, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Called by the host to register all services and executors for this job module.
    /// Override this method to register custom services.
    /// </summary>
    /// <param name="services">The service collection to register services with.</param>
    public virtual void RegisterServices(IServiceCollection services)
    {
        // Register each executor type as a service
        foreach (var executorType in ExecutorTypes)
        {
            if (typeof(ITaskExecutor).IsAssignableFrom(executorType))
            {
                services.AddTransient(executorType);
                services.AddTransient(typeof(ITaskExecutor), executorType);
            }
        }
    }

    /// <summary>
    /// Initializes the task executors for this job module.
    /// This method creates instances of all executor types and adds them to the task executors collection.
    /// </summary>
    private void InitializeTaskExecutors()
    {
        foreach (var executorType in ExecutorTypes)
        {
            if (typeof(ITaskExecutor).IsAssignableFrom(executorType))
            {
                try
                {
                    var executor = (ITaskExecutor)_serviceProvider.GetRequiredService(executorType);
                    _taskExecutors.Add(executor);
                    _logger.LogDebug("Initialized task executor: {ExecutorType}", executorType.Name);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to initialize task executor: {ExecutorType}", executorType.Name);
                    throw;
                }
            }
        }

        // Sort executors by dependencies to ensure proper execution order
        var sortedExecutors = SortTaskExecutorsByDependencies(_taskExecutors);
        _taskExecutors.Clear();
        _taskExecutors.AddRange(sortedExecutors);
    }

    /// <summary>
    /// Sorts task executors based on their dependencies using topological sorting.
    /// </summary>
    /// <param name="executors">The collection of task executors to sort.</param>
    /// <returns>A sorted collection of task executors.</returns>
    private IEnumerable<ITaskExecutor> SortTaskExecutorsByDependencies(IEnumerable<ITaskExecutor> executors)
    {
        var executorList = executors.ToList();
        var sorted = new List<ITaskExecutor>();
        var visited = new HashSet<Type>();
        var visiting = new HashSet<Type>();

        foreach (var executor in executorList)
        {
            if (!visited.Contains(executor.GetType()))
            {
                VisitExecutor(executor, executorList, visited, visiting, sorted);
            }
        }

        return sorted;
    }

    /// <summary>
    /// Recursively visits executors to perform topological sorting.
    /// </summary>
    private void VisitExecutor(
        ITaskExecutor executor,
        IList<ITaskExecutor> allExecutors,
        ISet<Type> visited,
        ISet<Type> visiting,
        IList<ITaskExecutor> sorted)
    {
        var executorType = executor.GetType();
        
        if (visiting.Contains(executorType))
        {
            throw new InvalidOperationException($"Circular dependency detected involving task executor: {executorType.Name}");
        }

        if (visited.Contains(executorType))
        {
            return;
        }

        visiting.Add(executorType);

        // Visit all dependencies first
        foreach (var dependencyType in executor.Dependencies)
        {
            var dependency = allExecutors.FirstOrDefault(e => e.GetType() == dependencyType);
            if (dependency != null)
            {
                VisitExecutor(dependency, allExecutors, visited, visiting, sorted);
            }
        }

        visiting.Remove(executorType);
        visited.Add(executorType);
        sorted.Add(executor);
    }
} 