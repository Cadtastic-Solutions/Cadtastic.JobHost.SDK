using Cadtastic.JobHost.SDK.Attributes;
using Cadtastic.JobHost.SDK.Base;
using Cadtastic.JobHost.SDK.Interfaces;

using Microsoft.Extensions.DependencyInjection;

namespace Cadtastic.JobHost.SDK.Examples;

/// <summary>
/// Example job demonstrating the attribute-based approach for simple job implementation.
/// This job shows how to use the [Job] and [Task] attributes for automatic discovery and execution.
/// </summary>
[Job("SimpleAttribute", Name = "Simple Attribute Job", Description = "A simple example job using attribute-based task discovery")]
public class SimpleAttributeJob : BaseJob
{
    /// <summary>
    /// Step 1: Initialize the job and perform setup tasks.
    /// </summary>
    /// <param name="context">The task execution context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Task(1, TaskId = "Initialize", Description = "Initialize job and perform setup")]
    public async Task<bool> InitializeAsync(ITaskContext context)
    {
        context.ReportTaskProgress(0, "Initializing job...");

        // Simulate initialization work
        await Task.Delay(100);

        context.JobContext.Log(JobLogLevel.Information, "✓ Job initialization completed");
        context.ReportTaskProgress(100, "Initialization complete");

        // Store initialization result for later tasks
        context.JobContext.SharedData["InitializationComplete"] = true;

        return true;
    }

    /// <summary>
    /// Step 2: Process data (simulated).
    /// </summary>
    /// <param name="context">The task execution context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Task(2, TaskId = "ProcessData", Description = "Process data", DependsOn = new[] { "Initialize" })]
    public async Task ProcessDataAsync(ITaskContext context)
    {
        context.ReportTaskProgress(0, "Processing data...");

        // Check if initialization completed
        var initializationComplete = context.JobContext.SharedData.TryGetValue("InitializationComplete", out var result)
            ? (bool)result
            : false;

        if (!initializationComplete)
        {
            context.JobContext.Log(JobLogLevel.Warning, "Skipping data processing due to failed initialization");
            return;
        }

        // Simulate data processing
        for (int i = 0; i <= 100; i += 25)
        {
            await Task.Delay(50);
            context.ReportTaskProgress(i, $"Processing data... {i}%");
        }

        context.JobContext.Log(JobLogLevel.Information, "✓ Data processing completed");
        context.ReportTaskProgress(100, "Data processing complete");
    }

    /// <summary>
    /// Step 3: Generate a summary report.
    /// </summary>
    /// <returns>A summary object containing job execution details.</returns>
    [Task(3, TaskId = "GenerateReport", Description = "Generate summary report", DependsOn = new[] { "ProcessData" })]
    public async Task<object> GenerateReportAsync()
    {
        await Task.Delay(25); // Minimal delay

        return new
        {
            Message = "Simple Attribute Job completed successfully",
            Timestamp = DateTime.UtcNow,
            StepsCompleted = new[]
            {
                "Initialization",
                "Data Processing",
                "Report Generation"
            }
        };
    }

    /// <summary>
    /// Override to register job-specific services.
    /// </summary>
    /// <param name="services">The service collection to register services with.</param>
    public override void RegisterJobServices(IServiceCollection services)
    {
        Console.WriteLine($"[DEBUG] SimpleAttributeJob.RegisterJobServices called");
        
        // Call base implementation first
        base.RegisterJobServices(services);
        
        // No additional services needed for SimpleAttributeJob
        Console.WriteLine($"[DEBUG] SimpleAttributeJob.RegisterJobServices completed");
    }

    /// <summary>
    /// Override to provide custom validation logic.
    /// </summary>
    /// <returns>True if the job configuration is valid, false otherwise.</returns>
    public override bool ValidateConfiguration()
    {
        // Add custom validation logic here if needed
        return base.ValidateConfiguration();
    }
}