namespace Cadtastic.JobHost.SDK.Attributes;

/// <summary>
/// Marks a method as a task that can be executed as part of a job.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class TaskAttribute : Attribute
{
    /// <summary>
    /// Gets the execution step for this task.
    /// Tasks are executed in ascending order of steps.
    /// Tasks with the same step number can run concurrently if CanRunConcurrently is true.
    /// </summary>
    public int Step { get; }

    /// <summary>
    /// Gets or sets the unique identifier for this task.
    /// If not specified, the method name will be used.
    /// </summary>
    public string? TaskId { get; set; }

    /// <summary>
    /// Gets or sets whether this task can run concurrently with other tasks at the same step.
    /// Default is true.
    /// </summary>
    public bool CanRunConcurrently { get; set; } = true;

    /// <summary>
    /// Gets or sets whether this task is critical.
    /// If a critical task fails, the entire job will fail.
    /// Default is false.
    /// </summary>
    public bool IsCritical { get; set; } = false;

    /// <summary>
    /// Gets or sets the task IDs that this task depends on.
    /// The task will not execute until all dependencies have completed successfully.
    /// </summary>
    public string[]? DependsOn { get; set; }

    /// <summary>
    /// Gets or sets the description of what this task does.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskAttribute"/> class.
    /// </summary>
    /// <param name="step">The execution step for this task.</param>
    public TaskAttribute(int step)
    {
        Step = step;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskAttribute"/> class with a task ID.
    /// </summary>
    /// <param name="step">The execution step for this task.</param>
    /// <param name="taskId">The unique identifier for this task.</param>
    public TaskAttribute(int step, string taskId)
    {
        Step = step;
        TaskId = taskId;
    }
}