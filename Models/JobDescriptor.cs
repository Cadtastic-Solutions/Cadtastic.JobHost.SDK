namespace Cadtastic.JobHost.SDK.Models;

/// <summary>
/// Lightweight descriptor for a job, containing metadata discovered at startup.
/// This allows listing available jobs without loading the actual job classes.
/// </summary>
public class JobDescriptor
{
    /// <summary>
    /// Gets or sets the unique job type identifier.
    /// </summary>
    public string JobType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name of the job.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the job.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the version of the job.
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the full path to the assembly containing the job.
    /// </summary>
    public string AssemblyPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the full type name of the job class.
    /// </summary>
    public string TypeName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the assembly name.
    /// </summary>
    public string AssemblyName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the discovery timestamp.
    /// </summary>
    public DateTime DiscoveredAt { get; set; } = DateTime.UtcNow;
} 