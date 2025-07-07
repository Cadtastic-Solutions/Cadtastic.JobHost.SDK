# Cadtastic.JobHost.SDK

**Last Updated:** 06/17/2025 05:10 PM

The Cadtastic.JobHost.SDK provides the core interfaces, base classes, and utilities needed to create job modules and task executors for the Cadtastic.JobHost framework. This SDK enables developers to create modular, maintainable, and type-safe job implementations.

## SDK Components

### Core Interfaces

```mermaid
graph TD
    A[ITaskExecutor] --> B[TaskExecutorBase]
    C[IJobModule] --> D[JobModuleBase]
    E[ITaskContext] --> F[TaskContext]
    G[ITaskResult] --> H[TaskResult]
```

### Directory Structure

- `Base/`: Base classes for common implementations
- `Interfaces/`: Core interfaces for the SDK
- `Helpers/`: Utility classes and helper methods
- `Models/`: Data models and DTOs
- `Exceptions/`: Custom exception types
- `Extensions/`: Extension methods

## Getting Started

### Installation

```bash
dotnet add package Cadtastic.JobHost.SDK
```

### Basic Usage

1. Create a job module:

```csharp
public class MyJobModule : IJobModule
{
    public string JobType => "MyJob";

    public IEnumerable<Type> ExecutorTypes => new[]
    {
        typeof(DataPreparationTask),
        typeof(DataProcessingTask)
    };
}
```

2. Create task executors:

```csharp
public class DataPreparationTask : TaskExecutorBase
{
    public override string TaskId => "DataPreparation";
    
    public override IReadOnlyCollection<Type> Dependencies => 
        TaskDependency.None();

    public override async Task<ITaskResult> ExecuteAsync(ITaskContext context)
    {
        try
        {
            // Implementation
            return TaskResult.Success();
        }
        catch (Exception ex)
        {
            return TaskResult.Failure(ex.Message);
        }
    }
}
```

## Core Components

### Task Executors

Task executors are the building blocks of jobs. They implement specific functionality and can declare dependencies on other tasks.

#### Base Class

```csharp
public abstract class TaskExecutorBase : ITaskExecutor
{
    public abstract string TaskId { get; }
    public abstract IReadOnlyCollection<Type> Dependencies { get; }
    public virtual bool IsCritical => false;
    public virtual bool CanRunConcurrently => false;
    public abstract Task<ITaskResult> ExecuteAsync(ITaskContext context);
}
```

#### Dependency Management

Use the `TaskDependency` helper class to declare dependencies:

```csharp
// Single dependency
TaskDependency.On<DataPreparationTask>()

// Multiple dependencies
TaskDependency.On<DataPreparationTask, DataProcessingTask>()

// No dependencies
TaskDependency.None()
```

### Task Results

Task results provide information about task execution:

```csharp
// Success result
TaskResult.Success()

// Failure result
TaskResult.Failure("Error message")

// Custom result
TaskResult.Create(success: true, message: "Custom message")
```

### Task Context

The task context provides access to job information and task results:

```csharp
public interface ITaskContext
{
    IJob Job { get; }
    IReadOnlyDictionary<string, ITaskResult> Results { get; }
    void AddResult(string taskId, ITaskResult result);
}
```

## Best Practices

### Task Design

1. **Single Responsibility**
   - Each task should do one thing well
   - Keep tasks focused and maintainable
   - Use clear, descriptive names

2. **Dependency Management**
   - Minimize dependencies
   - Avoid circular dependencies
   - Use type-safe dependency declarations

3. **Error Handling**
   - Implement proper error handling
   - Use meaningful error messages
   - Consider task criticality

4. **Performance**
   - Use async/await properly
   - Consider task concurrency
   - Optimize resource usage

### Module Organization

1. **Task Grouping**
   - Group related tasks
   - Use consistent naming
   - Maintain clear boundaries

2. **Exposure Control**
   - Expose only necessary tasks
   - Use internal tasks when appropriate
   - Document public tasks

## Development Guidelines

### Code Style

1. **Naming Conventions**
   - Use PascalCase for public members
   - Use camelCase for private members
   - Use descriptive names

2. **Documentation**
   - Document all public members
   - Use XML comments
   - Include examples

3. **Error Handling**
   - Use custom exceptions
   - Provide meaningful messages
   - Handle all error cases

### Testing

1. **Unit Testing**
   - Test individual tasks
   - Mock dependencies
   - Test error cases

2. **Integration Testing**
   - Test task interactions
   - Test module loading
   - Test error handling

## API Reference

### Interfaces

#### ITaskExecutor

```csharp
public interface ITaskExecutor
{
    string TaskId { get; }
    IReadOnlyCollection<Type> Dependencies { get; }
    bool IsCritical { get; }
    bool CanRunConcurrently { get; }
    Task<ITaskResult> ExecuteAsync(ITaskContext context);
}
```

#### IJobModule

```csharp
public interface IJobModule
{
    string JobType { get; }
    IEnumerable<Type> ExecutorTypes { get; }
}
```

#### ITaskContext

```csharp
public interface ITaskContext
{
    IJob Job { get; }
    IReadOnlyDictionary<string, ITaskResult> Results { get; }
    void AddResult(string taskId, ITaskResult result);
}
```

### Models

#### TaskResult

```csharp
public class TaskResult : ITaskResult
{
    public bool IsSuccess { get; }
    public string? ErrorMessage { get; }
    public DateTime Timestamp { get; }
}
```

#### Job

```csharp
public class Job : IJob
{
    public string Id { get; }
    public string JobType { get; }
    public JobStatus Status { get; }
    public DateTime CreatedAt { get; }
}
```

## Contributing

1. Follow SOLID principles
2. Maintain XML documentation
3. Write unit tests
4. Follow code style guidelines
5. Update documentation

## License

[License information here] 