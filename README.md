# Cadtastic Job Host SDK

**Last Updated:** 06-09-2025 17:16:43

SDK for developing jobs that can be executed by the Cadtastic Job Host.

## Overview

The Cadtastic Job Host SDK provides the essential interfaces and models needed to create job executors that can be dynamically loaded and executed by the Cadtastic Job Host. This ensures that all job implementations and the Job Host are always referencing the same exact interfaces.

## Key Interfaces

### IJobExecutor

The main interface that your job DLL must implement:

```csharp
public interface IJobExecutor
{
    string[] SupportedJobTypes { get; }
    string Description { get; }
    bool CanExecute(IJob job);
    Task<IJobExecutionResult> ExecuteAsync(IJobExecutionContext context);
}
```

### IJob

Represents a job to be executed:

```csharp
public interface IJob
{
    string Id { get; set; }
    string Name { get; set; }
    string JobType { get; set; }
    JobState State { get; set; }
    object Configuration { get; set; }
    IJobExecutionHistory ExecutionHistory { get; set; }
}
```

### JobState Enum

Represents the current execution state of a job:

```csharp
public enum JobState
{
    Inactive = 0,    // Job is not currently active
    Active = 1,      // Job is active and available for execution
    Processing = 2,  // Job is currently being processed
    Unknown = 4      // Job state cannot be determined
}
```

### IJobExecutionContext

Provides context and services during job execution:

```csharp
public interface IJobExecutionContext
{
    IJob Job { get; }
    IServiceProvider ServiceProvider { get; }
    IProgress<string>? Progress { get; }
    CancellationToken CancellationToken { get; }
}
```

### IJobExecutionResult

Represents the result of job execution:

```csharp
public interface IJobExecutionResult
{
    bool IsSuccess { get; }
    ResultState State { get; }
    string? ErrorMessage { get; }
    string? Details { get; }
    DateTime StartTime { get; }
    DateTime EndTime { get; }
    TimeSpan Duration { get; }
}
```

### ResultState Enum

Represents the result state of a job execution:

```csharp
public enum ResultState
{
    Failed = 0,      // Job execution failed
    Successful = 1,  // Job execution completed successfully
    Cancelled = 2,   // Job execution was cancelled
    Unknown = 4      // Job execution result state is unknown
}
```

### IJobExecutionHistory

Tracks job execution history and statistics:

```csharp
public interface IJobExecutionHistory
{
    IEnumerable<IJobExecutionResult> Results { get; set; }
    int TotalExecutions { get; set; }
    int TotalSucceeded { get; set; }
    int TotalFailed { get; set; }
}
```

## Extension Methods

The SDK provides several extension methods to reduce code duplication and improve developer experience. Extension methods are co-located with their respective interfaces for better organization:

### JobExecutorExtensions

Located in `Interfaces/IJobExecutor.cs`:

```csharp
using Cadtastic.JobHost.SDK.Interfaces;

// Execute with automatic timing and error handling - returns strongly typed result
var result = await this.ExecuteWithTimingAsync(context, async ctx => 
{
    // Your job logic here that returns a specific type
    return new { ProcessedCount = 100, Status = "Success" };
});

// Execute with automatic timing for void operations
var result = await this.ExecuteWithTimingAsync(context, async ctx => 
{
    // Your job logic here
    await DoSomeWork();
});

// Check if executor supports a job type
if (this.SupportsJobType("Archival"))
{
    // Handle archival jobs
}

// Get typed logger for the executor
var logger = this.GetLogger(context);
logger?.LogInformation("Starting job execution");
```

### ServiceProviderExtensions & JobExecutionContextExtensions

Located in `Interfaces/IJobExecutionContext.cs`:

```csharp
using Cadtastic.JobHost.SDK.Interfaces;

// Get logger with type safety directly from context
var logger = context.GetLogger<MyJobExecutor>();

// Get logger by category name
var logger = context.GetLogger("MyCategory");

// Get required service (throws if not found)
var myService = context.GetRequiredService<IMyService>();

// Get optional service (returns null if not found)
var optionalService = context.GetService<IOptionalService>();

// Report progress with structured logging
context.ReportProgress("Processing items...", logger);

// Report progress with additional data
context.ReportProgress("Processed batch", new { BatchSize = 50, Total = 500 }, logger);

// Service provider extensions work the same way
var logger2 = context.ServiceProvider.GetLogger<MyService>();
var allServices = context.ServiceProvider.GetServices<IMyInterface>();
```

## Working with Job and Result States

The SDK provides strongly-typed enums for job and execution states:

### Job State Management

```csharp
using Cadtastic.JobHost.SDK.Interfaces;

// Check if a job can be executed
if (job.CanBeExecuted())
{
    // Set job to processing state and store previous state
    var previousState = job.SetToProcessing();
    
    try
    {
        // Execute the job
        var result = await executor.ExecuteAsync(context);
        
        // Set job back to active if successful
        if (result.IsSuccessful())
        {
            job.SetToActive();
        }
        else
        {
            job.SetToInactive();
        }
    }
    catch
    {
        // Restore previous state on error
        job.State = previousState;
        throw;
    }
}

// Get human-readable state description
var stateDescription = job.GetStateDescription();
Console.WriteLine($"Job state: {stateDescription}");
```

### Result State Analysis

```csharp
using Cadtastic.JobHost.SDK.Interfaces;

// Analyze execution results
var result = await executor.ExecuteAsync(context);

// Check specific result states
if (result.IsSuccessful())
{
    Console.WriteLine("Job completed successfully!");
}
else if (result.IsFailed())
{
    Console.WriteLine($"Job failed: {result.ErrorMessage}");
}
else if (result.IsCancelled())
{
    Console.WriteLine("Job was cancelled by user or system");
}

// Get result summary
var summary = result.GetSummary();
Console.WriteLine(summary); // "The job execution completed successfully in 2.45 seconds"

// Determine if job should be retried
if (result.ShouldRetry())
{
    Console.WriteLine("Job should be retried");
}

// Create results with specific states
var successResult = JobExecutionResult.Successful("Processed 100 records");
var failedResult = JobExecutionResult.Failed("Database connection failed");
var cancelledResult = JobExecutionResult.Cancelled("User requested cancellation");
var unknownResult = JobExecutionResult.Unknown("Unable to determine result");

// Create result from specific state
var stateResult = JobExecutionResult.FromState(ResultState.Successful, "Custom success message");
```

### History Analysis with States

```csharp
using Cadtastic.JobHost.SDK.Models;

var history = JobExecutionHistory.Empty();

// Add results and analyze by state
history.AddExecutionResult(successResult);
history.AddExecutionResult(failedResult);
history.AddExecutionResult(cancelledResult);

// Get results by specific state
var successfulRuns = history.GetSuccessfulResults();
var failedRuns = history.GetFailedResults();
var cancelledRuns = history.GetCancelledResults();

// Get counts by state
var successCount = history.GetSuccessfulExecutionCount();
var failedCount = history.GetFailedExecutionCount();
var cancelledCount = history.GetCancelledExecutionCount();

// Filter by any state
var unknownResults = history.GetResultsByState(ResultState.Unknown);
var specificStateCount = history.GetExecutionCountByState(ResultState.Cancelled);

Console.WriteLine($"Success rate: {history.GetSuccessRate():F1}%");
```

## Quick Start Guide

To create a job executor that the JobHost can run:

1. **Create a new Class Library project** targeting .NET 8.0
2. **Add NuGet reference** to `Cadtastic.JobHost.SDK`
3. **Implement IJobExecutor interface** in your class
4. **Build and deploy** the DLL to the JobHost's `JobTypes` directory

```csharp
// Minimal working example
public class MyJobExecutor : IJobExecutor
{
    public string[] SupportedJobTypes => new[] { "MyJobType" };
    public string Description => "My custom job executor";
    
    public bool CanExecute(IJob job) => this.SupportsJobType(job.JobType);
    
    public async Task<IJobExecutionResult> ExecuteAsync(IJobExecutionContext context)
    {
        return await this.ExecuteWithTimingAsync(context, async ctx =>
        {
            // Your job logic here
            await Task.Delay(1000, ctx.CancellationToken);
            return "Job completed successfully!";
        });
    }
}
```

## Creating Job Executors for the JobHost

### 1. Minimal Job Executor Implementation

Here's the simplest possible job executor that the JobHost can run:

```csharp
using Cadtastic.JobHost.SDK.Interfaces;
using Cadtastic.JobHost.SDK.Models;

namespace MyCompany.Jobs;

/// <summary>
/// Minimal job executor demonstrating the simplest implementation pattern.
/// </summary>
public class HelloWorldJobExecutor : IJobExecutor
{
    public string[] SupportedJobTypes => new[] { "HelloWorld", "Test" };
    
    public string Description => "Simple hello world job for testing";

    public bool CanExecute(IJob job)
    {
        return this.SupportsJobType(job.JobType);
    }

    public async Task<IJobExecutionResult> ExecuteAsync(IJobExecutionContext context)
    {
        // Using extension method for automatic timing and error handling
        return await this.ExecuteWithTimingAsync(context, async ctx =>
        {
            var logger = this.GetLogger(ctx);
            
            logger?.LogInformation("Hello World job {JobId} is running!", ctx.Job.Id);
            ctx.ReportProgress("Saying hello to the world...", logger);
            
            // Simulate some work
            await Task.Delay(1000, ctx.CancellationToken);
            
            var message = $"Hello from job {ctx.Job.Name}!";
            logger?.LogInformation("Job completed with message: {Message}", message);
            
            return message;
        });
    }
}
```

### 2. Basic Job Executor Implementation

Here's a simple job executor that the JobHost can discover and run:

```csharp
using Cadtastic.JobHost.SDK.Interfaces;
using Cadtastic.JobHost.SDK.Models;

namespace MyCompany.Jobs;

/// <summary>
/// Simple backup job executor that demonstrates basic implementation.
/// </summary>
public class BackupJobExecutor : IJobExecutor
{
    public string[] SupportedJobTypes => new[] { "Backup", "DatabaseBackup" };
    
    public string Description => "Performs backup operations for databases and files";

    public bool CanExecute(IJob job)
    {
        return SupportedJobTypes.Contains(job.JobType, StringComparer.OrdinalIgnoreCase);
    }

    public async Task<IJobExecutionResult> ExecuteAsync(IJobExecutionContext context)
    {
        var startTime = DateTime.Now;
        
        try
        {
            context.Progress?.Report("Starting backup process...");
            
            // Extract configuration
            var config = context.Job.Configuration as BackupConfiguration 
                ?? throw new InvalidOperationException("Invalid backup configuration");
            
            // Perform backup
            await PerformBackup(config, context.CancellationToken);
            
            context.Progress?.Report("Backup completed successfully");
            
            return JobExecutionResult.Successful(
                $"Backup completed for {config.SourcePath}", 
                startTime, 
                DateTime.Now);
        }
        catch (Exception ex)
        {
            return JobExecutionResult.Failed(
                ex.Message, 
                ex.StackTrace, 
                startTime, 
                DateTime.Now);
        }
    }

    private async Task PerformBackup(BackupConfiguration config, CancellationToken cancellationToken)
    {
        // Simulate backup work
        await Task.Delay(2000, cancellationToken);
    }
}

/// <summary>
/// Configuration class for backup jobs.
/// </summary>
public class BackupConfiguration
{
    public string SourcePath { get; set; } = string.Empty;
    public string DestinationPath { get; set; } = string.Empty;
    public bool CompressBackup { get; set; } = true;
}
```

### 3. Advanced Job Executor with Extension Methods

```csharp
using Cadtastic.JobHost.SDK.Interfaces;
using Microsoft.Extensions.Logging;

namespace MyCompany.Jobs;

/// <summary>
/// Advanced data processing job executor using SDK extension methods.
/// </summary>
public class DataProcessingJobExecutor : IJobExecutor
{
    public string[] SupportedJobTypes => new[] { "DataProcessing", "ETL", "DataTransform" };
    
    public string Description => "Processes and transforms data with comprehensive logging";

    public bool CanExecute(IJob job)
    {
        return this.SupportsJobType(job.JobType);
    }

    public async Task<IJobExecutionResult> ExecuteAsync(IJobExecutionContext context)
    {
        // Use extension method for automatic timing and error handling
        return await this.ExecuteWithTimingAsync(context, async ctx =>
        {
            var logger = this.GetLogger(ctx);
            var dataService = ctx.GetRequiredService<IDataService>();
            
            logger?.LogInformation("Starting data processing for job {JobId}", ctx.Job.Id);
            
            // Extract configuration with validation
            var config = ExtractConfiguration<DataProcessingConfiguration>(ctx.Job);
            
            // Process data in batches with progress reporting
            var result = await ProcessDataInBatches(config, dataService, ctx, logger);
            
            logger?.LogInformation("Data processing completed. Processed {RecordCount} records", result.TotalRecords);
            
            return new ProcessingResult
            {
                TotalRecords = result.TotalRecords,
                ProcessedRecords = result.ProcessedRecords,
                FailedRecords = result.FailedRecords,
                ProcessingDuration = result.Duration,
                BatchCount = result.BatchCount
            };
        });
    }

    private async Task<ProcessingResult> ProcessDataInBatches(
        DataProcessingConfiguration config, 
        IDataService dataService, 
        IJobExecutionContext context,
        ILogger? logger)
    {
        var result = new ProcessingResult();
        var batchSize = config.BatchSize;
        var totalRecords = await dataService.GetRecordCountAsync(config.SourceQuery);
        var totalBatches = (int)Math.Ceiling((double)totalRecords / batchSize);
        
        result.TotalRecords = totalRecords;
        result.BatchCount = totalBatches;
        
        for (int batch = 0; batch < totalBatches; batch++)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            
            var batchData = await dataService.GetBatchAsync(config.SourceQuery, batch * batchSize, batchSize);
            
            try
            {
                await dataService.ProcessBatchAsync(batchData, config.TransformationRules);
                result.ProcessedRecords += batchData.Count;
                
                // Report progress with structured data
                var progressData = new { 
                    Batch = batch + 1, 
                    TotalBatches = totalBatches, 
                    RecordsProcessed = result.ProcessedRecords,
                    PercentComplete = Math.Round((double)(batch + 1) / totalBatches * 100, 1)
                };
                
                context.ReportProgress($"Processed batch {batch + 1}/{totalBatches}", progressData, logger);
            }
            catch (Exception ex)
            {
                logger?.LogWarning(ex, "Failed to process batch {BatchNumber}", batch + 1);
                result.FailedRecords += batchData.Count;
            }
        }
        
        return result;
    }

    private T ExtractConfiguration<T>(IJob job) where T : class
    {
        return job.Configuration as T 
            ?? throw new InvalidOperationException($"Invalid configuration type. Expected {typeof(T).Name}");
    }
}

/// <summary>
/// Configuration for data processing jobs.
/// </summary>
public class DataProcessingConfiguration
{
    public string SourceQuery { get; set; } = string.Empty;
    public int BatchSize { get; set; } = 1000;
    public Dictionary<string, object> TransformationRules { get; set; } = new();
}

/// <summary>
/// Result object for data processing operations.
/// </summary>
public class ProcessingResult
{
    public int TotalRecords { get; set; }
    public int ProcessedRecords { get; set; }
    public int FailedRecords { get; set; }
    public TimeSpan Duration { get; set; }
    public int BatchCount { get; set; }
}

/// <summary>
/// Service interface for data operations.
/// </summary>
public interface IDataService
{
    Task<int> GetRecordCountAsync(string query);
    Task<List<DataRecord>> GetBatchAsync(string query, int offset, int batchSize);
    Task ProcessBatchAsync(List<DataRecord> batch, Dictionary<string, object> rules);
}

/// <summary>
/// Represents a data record for processing.
/// </summary>
public class DataRecord
{
    public string Id { get; set; } = string.Empty;
    public Dictionary<string, object> Properties { get; set; } = new();
}
```

### 4. Job Executor with History Tracking

```csharp
using Cadtastic.JobHost.SDK.Interfaces;
using Cadtastic.JobHost.SDK.Models;

namespace MyCompany.Jobs;

/// <summary>
/// Cleanup job executor that tracks execution history and performance metrics.
/// </summary>
public class CleanupJobExecutor : IJobExecutor
{
    private readonly IJobExecutionHistory _executionHistory;

    public string[] SupportedJobTypes => new[] { "Cleanup", "Purge", "Maintenance" };
    
    public string Description => "Performs cleanup operations with execution history tracking";

    public CleanupJobExecutor()
    {
        _executionHistory = JobExecutionHistory.Empty();
    }

    public bool CanExecute(IJob job)
    {
        return this.SupportsJobType(job.JobType);
    }

    public async Task<IJobExecutionResult> ExecuteAsync(IJobExecutionContext context)
    {
        var result = await this.ExecuteWithTimingAsync(context, async ctx =>
        {
            var logger = this.GetLogger(ctx);
            var fileService = ctx.GetService<IFileService>();
            
            logger?.LogInformation("Starting cleanup job {JobId}. Historical success rate: {SuccessRate:F1}%", 
                ctx.Job.Id, _executionHistory.GetSuccessRate());
            
            var config = ExtractConfiguration<CleanupConfiguration>(ctx.Job);
            var cleanupResult = await PerformCleanup(config, fileService, ctx, logger);
            
            // Log performance metrics
            var avgDuration = _executionHistory.GetAverageExecutionDuration();
            logger?.LogInformation("Cleanup completed. Average execution time: {AvgDuration}", avgDuration);
            
            return cleanupResult;
        });

        // Track execution history
        _executionHistory.AddExecutionResult(result);
        
        // Log historical statistics
        var logger = context.GetLogger<CleanupJobExecutor>();
        logger?.LogInformation("Job execution history - Total: {Total}, Success Rate: {SuccessRate:F1}%, Avg Duration: {AvgDuration}",
            _executionHistory.TotalExecutions,
            _executionHistory.GetSuccessRate(),
            _executionHistory.GetAverageExecutionDuration());

        return result;
    }

    private async Task<CleanupResult> PerformCleanup(
        CleanupConfiguration config, 
        IFileService? fileService, 
        IJobExecutionContext context,
        ILogger? logger)
    {
        var result = new CleanupResult();
        
        if (fileService == null)
        {
            logger?.LogWarning("File service not available, skipping file cleanup");
            return result;
        }

        // Clean up files older than specified days
        var cutoffDate = DateTime.Now.AddDays(-config.RetentionDays);
        var filesToDelete = await fileService.GetFilesOlderThanAsync(config.CleanupPath, cutoffDate);
        
        result.TotalFilesFound = filesToDelete.Count;
        
        foreach (var file in filesToDelete)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            
            try
            {
                await fileService.DeleteFileAsync(file.Path);
                result.FilesDeleted++;
                result.BytesFreed += file.Size;
                
                if (result.FilesDeleted % 100 == 0)
                {
                    context.ReportProgress($"Deleted {result.FilesDeleted}/{result.TotalFilesFound} files", 
                        new { DeletedCount = result.FilesDeleted, BytesFreed = result.BytesFreed }, logger);
                }
            }
            catch (Exception ex)
            {
                logger?.LogWarning(ex, "Failed to delete file {FilePath}", file.Path);
                result.FailedDeletions++;
            }
        }
        
        return result;
    }

    private T ExtractConfiguration<T>(IJob job) where T : class
    {
        return job.Configuration as T 
            ?? throw new InvalidOperationException($"Invalid configuration type. Expected {typeof(T).Name}");
    }

    /// <summary>
    /// Gets the current execution history for monitoring purposes.
    /// </summary>
    public IJobExecutionHistory GetExecutionHistory() => _executionHistory;
}

/// <summary>
/// Configuration for cleanup jobs.
/// </summary>
public class CleanupConfiguration
{
    public string CleanupPath { get; set; } = string.Empty;
    public int RetentionDays { get; set; } = 30;
    public bool RecursiveCleanup { get; set; } = true;
}

/// <summary>
/// Result object for cleanup operations.
/// </summary>
public class CleanupResult
{
    public int TotalFilesFound { get; set; }
    public int FilesDeleted { get; set; }
    public int FailedDeletions { get; set; }
    public long BytesFreed { get; set; }
}

/// <summary>
/// Service interface for file operations.
/// </summary>
public interface IFileService
{
    Task<List<FileInfo>> GetFilesOlderThanAsync(string path, DateTime cutoffDate);
    Task DeleteFileAsync(string filePath);
}

/// <summary>
/// File information for cleanup operations.
/// </summary>
public class FileInfo
{
    public string Path { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime LastModified { get; set; }
}
```

### 5. Multi-Type Job Executor

```csharp
using Cadtastic.JobHost.SDK.Interfaces;

namespace MyCompany.Jobs;

/// <summary>
/// Versatile job executor that handles multiple related job types.
/// </summary>
public class FileOperationsJobExecutor : IJobExecutor
{
    public string[] SupportedJobTypes => new[] { "FileCopy", "FileMove", "FileCompress", "FileEncrypt" };
    
    public string Description => "Handles various file operations including copy, move, compress, and encrypt";

    public bool CanExecute(IJob job)
    {
        return this.SupportsJobType(job.JobType);
    }

    public async Task<IJobExecutionResult> ExecuteAsync(IJobExecutionContext context)
    {
        return await this.ExecuteWithTimingAsync(context, async ctx =>
        {
            var logger = this.GetLogger(ctx);
            
            // Route to appropriate handler based on job type
            return ctx.Job.JobType.ToUpperInvariant() switch
            {
                "FILECOPY" => await HandleFileCopy(ctx, logger),
                "FILEMOVE" => await HandleFileMove(ctx, logger),
                "FILECOMPRESS" => await HandleFileCompress(ctx, logger),
                "FILEENCRYPT" => await HandleFileEncrypt(ctx, logger),
                _ => throw new NotSupportedException($"Job type {ctx.Job.JobType} is not supported")
            };
        });
    }

    private async Task<FileOperationResult> HandleFileCopy(IJobExecutionContext context, ILogger? logger)
    {
        var config = ExtractConfiguration<FileCopyConfiguration>(context.Job);
        logger?.LogInformation("Starting file copy from {Source} to {Destination}", config.SourcePath, config.DestinationPath);
        
        // Simulate file copy operation
        await Task.Delay(1000, context.CancellationToken);
        
        return new FileOperationResult
        {
            Operation = "Copy",
            SourcePath = config.SourcePath,
            DestinationPath = config.DestinationPath,
            FilesProcessed = 1,
            Success = true
        };
    }

    private async Task<FileOperationResult> HandleFileMove(IJobExecutionContext context, ILogger? logger)
    {
        var config = ExtractConfiguration<FileMoveConfiguration>(context.Job);
        logger?.LogInformation("Starting file move from {Source} to {Destination}", config.SourcePath, config.DestinationPath);
        
        // Simulate file move operation
        await Task.Delay(800, context.CancellationToken);
        
        return new FileOperationResult
        {
            Operation = "Move",
            SourcePath = config.SourcePath,
            DestinationPath = config.DestinationPath,
            FilesProcessed = 1,
            Success = true
        };
    }

    private async Task<FileOperationResult> HandleFileCompress(IJobExecutionContext context, ILogger? logger)
    {
        var config = ExtractConfiguration<FileCompressConfiguration>(context.Job);
        logger?.LogInformation("Starting file compression for {Source}", config.SourcePath);
        
        // Simulate compression operation
        await Task.Delay(2000, context.CancellationToken);
        
        return new FileOperationResult
        {
            Operation = "Compress",
            SourcePath = config.SourcePath,
            DestinationPath = config.OutputPath,
            FilesProcessed = config.FileCount,
            CompressionRatio = config.CompressionLevel,
            Success = true
        };
    }

    private async Task<FileOperationResult> HandleFileEncrypt(IJobExecutionContext context, ILogger? logger)
    {
        var config = ExtractConfiguration<FileEncryptConfiguration>(context.Job);
        logger?.LogInformation("Starting file encryption for {Source}", config.SourcePath);
        
        // Simulate encryption operation
        await Task.Delay(1500, context.CancellationToken);
        
        return new FileOperationResult
        {
            Operation = "Encrypt",
            SourcePath = config.SourcePath,
            DestinationPath = config.OutputPath,
            FilesProcessed = 1,
            EncryptionAlgorithm = config.Algorithm,
            Success = true
        };
    }

    private T ExtractConfiguration<T>(IJob job) where T : class
    {
        return job.Configuration as T 
            ?? throw new InvalidOperationException($"Invalid configuration type. Expected {typeof(T).Name}");
    }
}

// Configuration classes for different operations
public class FileCopyConfiguration
{
    public string SourcePath { get; set; } = string.Empty;
    public string DestinationPath { get; set; } = string.Empty;
    public bool OverwriteExisting { get; set; } = false;
}

public class FileMoveConfiguration
{
    public string SourcePath { get; set; } = string.Empty;
    public string DestinationPath { get; set; } = string.Empty;
}

public class FileCompressConfiguration
{
    public string SourcePath { get; set; } = string.Empty;
    public string OutputPath { get; set; } = string.Empty;
    public int CompressionLevel { get; set; } = 5;
    public int FileCount { get; set; } = 1;
}

public class FileEncryptConfiguration
{
    public string SourcePath { get; set; } = string.Empty;
    public string OutputPath { get; set; } = string.Empty;
    public string Algorithm { get; set; } = "AES-256";
    public string KeyPath { get; set; } = string.Empty;
}

public class FileOperationResult
{
    public string Operation { get; set; } = string.Empty;
    public string SourcePath { get; set; } = string.Empty;
    public string DestinationPath { get; set; } = string.Empty;
    public int FilesProcessed { get; set; }
    public int CompressionRatio { get; set; }
    public string EncryptionAlgorithm { get; set; } = string.Empty;
    public bool Success { get; set; }
}
```

## Summary of Job Executor Patterns

| Pattern | Use Case | Key Features | Complexity |
|---------|----------|--------------|------------|
| **Minimal** | Simple tasks, testing, proof of concept | Basic implementation, extension method usage | Low |
| **Basic** | Standard business operations | Manual error handling, custom configuration | Medium |
| **Advanced** | Data processing, ETL operations | Batch processing, structured logging, service injection | High |
| **History Tracking** | Operations requiring monitoring | Performance metrics, execution statistics | Medium |
| **Multi-Type** | Related operations in one executor | Job type routing, shared functionality | Medium-High |

## Project Structure

Your job executor project should follow this structure:

```text
MyJobExecutor/
├── MyJobExecutor.csproj
├── Executors/
│   ├── BackupJobExecutor.cs
│   ├── DataProcessingJobExecutor.cs
│   ├── CleanupJobExecutor.cs
│   └── FileOperationsJobExecutor.cs
├── Configuration/
│   ├── BackupConfiguration.cs
│   ├── DataProcessingConfiguration.cs
│   └── CleanupConfiguration.cs
├── Services/
│   ├── IDataService.cs
│   ├── IFileService.cs
│   └── Implementations/
├── Models/
│   ├── ProcessingResult.cs
│   ├── CleanupResult.cs
│   └── FileOperationResult.cs
└── README.md
```

## Deployment

1. Build your project: `C:\Program Files\dotnet\dotnet.exe build --configuration Release`
2. Copy the output DLL to the JobHost's `JobTypes` directory
3. The JobHost will automatically discover and register your executors
4. Ensure all dependencies are available in the JobHost's runtime environment

## Best Practices

1. **Single Responsibility**: Each executor should handle only one type of job or closely related job types.
2. **Error Handling**: Always wrap your execution logic in try-catch blocks or use the extension methods.
3. **Progress Reporting**: Use the provided `Progress<string>` to report execution status.
4. **Cancellation Support**: Respect the `CancellationToken` for graceful shutdown.
5. **Logging**: Use the `IServiceProvider` to access logging services when needed.
6. **Use Extensions**: Leverage the provided extension methods to reduce code duplication.
7. **Dependency Injection**: Use the service provider to access required services.
8. **Type Safety**: Use generic extension methods for strongly typed results and services.
9. **Structured Logging**: Use the enhanced progress reporting with structured logging support.
10. **Configuration Validation**: Always validate job configuration before processing.
11. **History Tracking**: Consider implementing execution history for monitoring and analytics.
12. **Batch Processing**: For large datasets, implement batch processing with progress reporting.

## Testing Your Job Executors

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Cadtastic.JobHost.SDK.Interfaces;
using Cadtastic.JobHost.SDK.Models;

// Example unit test setup
public class JobExecutorTests
{
    [Test]
    public async Task BackupJobExecutor_ShouldExecuteSuccessfully()
    {
        // Arrange
        var services = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();

        var job = new TestJob
        {
            Id = "test-001",
            Name = "Test Backup",
            JobType = "Backup",
            Configuration = new BackupConfiguration
            {
                SourcePath = "/source",
                DestinationPath = "/backup"
            }
        };

        var context = new JobExecutionContext(
            job, 
            services, 
            new Progress<string>(msg => Console.WriteLine($"Progress: {msg}")));

        var executor = new BackupJobExecutor();

        // Act
        var result = await executor.ExecuteAsync(context);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.ErrorMessage, Is.Null);
    }
}

public class TestJob : IJob
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string JobType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public object Configuration { get; set; } = new();
}
```

## NuGet Package

The SDK can be packaged and distributed as a NuGet package for easy consumption by job developers.

## Versioning

The SDK follows semantic versioning. Always ensure your job executors are compatible with the SDK version used by the Job Host. 