using Cadtastic.JobHost.SDK.Interfaces;

namespace Cadtastic.JobHost.SDK.Exceptions
{
    /// <summary>
    /// Exception thrown when an error occurs during job execution.
    /// </summary>
    public class JobExecutionException : Exception
    {
        /// <summary>
        /// Gets the job that was being executed when the exception occurred.
        /// </summary>
        public IJob Job { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JobExecutionException"/> class with a specified job and message.
        /// </summary>
        /// <param name="job">The job that was being executed.</param>
        /// <param name="message">The message that describes the error.</param>
        public JobExecutionException(IJob job, string message) : base(message)
        {
            Job = job;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JobExecutionException"/> class with a specified job, message, and inner exception.
        /// </summary>
        /// <param name="job">The job that was being executed.</param>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public JobExecutionException(IJob job, string message, Exception innerException) : base(message, innerException)
        {
            Job = job;
        }
    }
} 