using Microsoft.Extensions.DependencyInjection;

namespace Cadtastic.JobHost.SDK.Interfaces
{
    /// <summary>
    /// Represents a module that defines a job and its associated task executors.
    /// </summary>
    public interface IJobModule
    {
        /// <summary>
        /// Called by the host to register all services and executors for this job module.
        /// </summary>
        void RegisterServices(IServiceCollection services);

        /// <summary>
        /// The display name of the job module.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The description of the job module.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The version of the job module.
        /// </summary>
        string Version { get; }

        /// <summary>
        /// The job types supported by this module.
        /// </summary>
        string[] SupportedJobTypes { get; }

        /// <summary>
        /// Gets the unique identifier for this job module.
        /// </summary>
        string JobType { get; }

        /// <summary>
        /// Gets the types of task executors that this job module exposes to the host.
        /// Only these task executors will be available for job execution.
        /// </summary>
        Type[] ExecutorTypes { get; }

        /// <summary>
        /// Gets the task executors that make up this job.
        /// These are the actual instances that will be used for execution.
        /// </summary>
        IEnumerable<ITaskExecutor> TaskExecutors { get; }

        /// <summary>
        /// Validates the job configuration.
        /// </summary>
        /// <param name="job">The job to validate.</param>
        /// <returns>True if the job configuration is valid, false otherwise.</returns>
        bool ValidateJob(IJob job);
    }
} 