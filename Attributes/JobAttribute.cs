namespace Cadtastic.JobHost.SDK.Attributes;

/// <summary>
/// Marks a class as a job that can be discovered and executed by the JobHost.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class JobAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the job type identifier.
    /// If not specified, the class name will be used.
    /// </summary>
    public string? JobType { get; set; }

    /// <summary>
    /// Gets or sets the display name of the job.
    /// If not specified, a friendly version of the class name will be used.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the job.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the version of the job.
    /// </summary>
    public string Version { get; set; } = "1.0.0";

    /// <summary>
    /// Initializes a new instance of the <see cref="JobAttribute"/> class.
    /// </summary>
    public JobAttribute()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JobAttribute"/> class with a job type.
    /// </summary>
    /// <param name="jobType">The job type identifier.</param>
    public JobAttribute(string jobType)
    {
        JobType = jobType;
    }
}