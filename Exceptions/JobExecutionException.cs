using System.Runtime.Serialization;

namespace Cadtastic.JobHost.SDK.Exceptions
{
    /// <summary>
    /// Exception thrown when there is an error during job execution.
    /// This exception provides detailed information about the execution failure and the affected job.
    /// </summary>
    [Serializable]
    public class JobExecutionException : Exception
    {
        /// <summary>
        /// Gets the unique identifier of the job that failed to execute.
        /// </summary>
        public string JobId { get; }

        /// <summary>
        /// Gets the specific reason for the job execution failure.
        /// </summary>
        public JobExecutionFailureReason FailureReason { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JobExecutionException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="jobId">The unique identifier of the job that failed to execute.</param>
        /// <param name="failureReason">The specific reason for the job execution failure.</param>
        /// <exception cref="ArgumentNullException">Thrown when message or jobId is null.</exception>
        /// <exception cref="ArgumentException">Thrown when jobId is empty or consists only of white-space characters.</exception>
        public JobExecutionException(string message, string jobId, JobExecutionFailureReason failureReason)
            : base(message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException(nameof(message));
            if (string.IsNullOrWhiteSpace(jobId))
                throw new ArgumentException("Job ID cannot be null or empty.", nameof(jobId));

            JobId = jobId;
            FailureReason = failureReason;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JobExecutionException"/> class with a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="jobId">The unique identifier of the job that failed to execute.</param>
        /// <param name="failureReason">The specific reason for the job execution failure.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        /// <exception cref="ArgumentNullException">Thrown when message, jobId, or innerException is null.</exception>
        /// <exception cref="ArgumentException">Thrown when jobId is empty or consists only of white-space characters.</exception>
        public JobExecutionException(string message, string jobId, JobExecutionFailureReason failureReason, Exception innerException)
            : base(message, innerException)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException(nameof(message));
            if (string.IsNullOrWhiteSpace(jobId))
                throw new ArgumentException("Job ID cannot be null or empty.", nameof(jobId));
            if (innerException == null)
                throw new ArgumentNullException(nameof(innerException));

            JobId = jobId;
            FailureReason = failureReason;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JobExecutionException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="ArgumentNullException">Thrown when info is null.</exception>
        /// <exception cref="SerializationException">Thrown when the class name is null or <see cref="System.Exception.HResult"/> is zero (0).</exception>
        protected JobExecutionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            JobId = info.GetString(nameof(JobId)) ?? string.Empty;
            FailureReason = (JobExecutionFailureReason)info.GetInt32(nameof(FailureReason));
        }

        /// <summary>
        /// Sets the <see cref="SerializationInfo"/> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="ArgumentNullException">Thrown when info is null.</exception>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            base.GetObjectData(info, context);
            info.AddValue(nameof(JobId), JobId);
            info.AddValue(nameof(FailureReason), (int)FailureReason);
        }
    }

    /// <summary>
    /// Specifies the reason for a job execution failure.
    /// </summary>
    public enum JobExecutionFailureReason
    {
        /// <summary>
        /// The job failed because one or more of its tasks failed to execute.
        /// </summary>
        TaskFailure,

        /// <summary>
        /// The job failed because it was cancelled.
        /// </summary>
        Cancelled,

        /// <summary>
        /// The job failed because it timed out.
        /// </summary>
        Timeout,

        /// <summary>
        /// The job failed because it could not be validated.
        /// </summary>
        ValidationFailed,

        /// <summary>
        /// The job failed because it could not be initialized.
        /// </summary>
        InitializationFailed,

        /// <summary>
        /// The job failed because it could not be found.
        /// </summary>
        NotFound,

        /// <summary>
        /// The job failed because it is not in a valid state for execution.
        /// </summary>
        InvalidState,

        /// <summary>
        /// The job failed due to an unknown error.
        /// </summary>
        Unknown
    }
} 