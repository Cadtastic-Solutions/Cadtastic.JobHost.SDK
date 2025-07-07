# Cadtastic.JobHost.SDK Process Flows

## Overview

This document describes the key process flows and call sequences in the Cadtastic.JobHost.SDK.

## Job Execution Flow

```mermaid
sequenceDiagram
    participant Client
    participant JobModule
    participant TaskExecutor
    participant ServiceProvider
    participant Logger

    Client->>JobModule: Create JobModule
    JobModule->>ServiceProvider: Get Required Services
    ServiceProvider-->>JobModule: Return Services
    JobModule->>JobModule: Initialize Tasks

    Client->>JobModule: ExecuteAsync()
    JobModule->>JobModule: OnExecutingAsync()
    JobModule->>Logger: Log Start
    JobModule->>JobModule: ExecuteTasksAsync()

    loop For Each Task
        JobModule->>JobModule: Find Executable Tasks
        JobModule->>TaskExecutor: ExecuteAsync()
        TaskExecutor->>ServiceProvider: Get Required Services
        ServiceProvider-->>TaskExecutor: Return Services
        TaskExecutor->>Logger: Log Progress
        TaskExecutor-->>JobModule: Return Result
        JobModule->>JobModule: OnTaskCompletedAsync()
    end

    JobModule->>JobModule: OnExecutedAsync()
    JobModule->>Logger: Log Completion
    JobModule-->>Client: Return Result
```

## Task Dependency Management

```mermaid
graph TD
    A[Task A] --> B[Task B]
    A --> C[Task C]
    B --> D[Task D]
    C --> D
    D --> E[Task E]

    subgraph "Dependency Graph"
        A
        B
        C
        D
        E
    end

    subgraph "Execution Order"
        A
        B & C
        D
        E
    end
```

## Task Execution States

```mermaid
stateDiagram-v2
    [*] --> Waiting
    Waiting --> Running: ExecuteAsync()
    Running --> Completed: Success
    Running --> Failed: Error
    Running --> Cancelled: Cancel()
    Completed --> [*]
    Failed --> [*]
    Cancelled --> [*]
```

## Service Provider and Logging

```mermaid
sequenceDiagram
    participant Task
    participant ServiceProvider
    participant LoggerFactory
    participant Logger

    Task->>ServiceProvider: GetLogger<T>()
    ServiceProvider->>ServiceProvider: GetRequiredService<ILoggerFactory>
    ServiceProvider->>LoggerFactory: CreateLogger<T>
    LoggerFactory-->>Task: Return Logger

    Task->>Logger: Log Information
    Task->>Logger: Log Warning
    Task->>Logger: Log Error
```

## Task Result Flow

```mermaid
sequenceDiagram
    participant Task
    participant JobModule
    participant TaskResult
    participant Logger

    Task->>Task: Execute Logic
    alt Success
        Task->>TaskResult: Create Success Result
    else Failure
        Task->>TaskResult: Create Failure Result
    end
    Task-->>JobModule: Return Result
    JobModule->>Logger: Log Result
    JobModule->>JobModule: Store Result
```

## Cancellation Flow

```mermaid
sequenceDiagram
    participant Client
    participant JobModule
    participant Task
    participant CancellationTokenSource

    Client->>JobModule: Cancel()
    JobModule->>CancellationTokenSource: Cancel()
    CancellationTokenSource->>Task: Signal Cancellation
    Task->>Task: Handle Cancellation
    Task-->>JobModule: Return Cancelled Result
    JobModule->>JobModule: Update Status
```

## Error Handling Flow

```mermaid
sequenceDiagram
    participant Task
    participant JobModule
    participant Logger
    participant TaskResult

    Task->>Task: Execute Logic
    alt Error Occurs
        Task->>Logger: Log Error
        Task->>TaskResult: Create Error Result
        Task-->>JobModule: Return Error Result
        JobModule->>Logger: Log Task Failure
        JobModule->>JobModule: Update Status
    end
```

## Task Dependency Validation

```mermaid
graph TD
    A[Start] --> B[Get All Tasks]
    B --> C[For Each Task]
    C --> D{Has Dependencies?}
    D -->|Yes| E[Check Dependencies]
    D -->|No| F[Mark as Valid]
    E --> G{Has Cycle?}
    G -->|Yes| H[Mark as Invalid]
    G -->|No| I[Mark as Valid]
    H --> J[End]
    F --> J
    I --> J
```

## Task Execution Strategy

```mermaid
graph TD
    A[Start] --> B[Get Executable Tasks]
    B --> C{Has Tasks?}
    C -->|Yes| D[Execute Tasks]
    C -->|No| E[End]
    D --> F{Task Completed?}
    F -->|Yes| G[Update Dependencies]
    F -->|No| D
    G --> B
```

## Notes

1. All flows include proper error handling and logging
2. Task execution is managed with thread safety in mind
3. Dependencies are validated before execution
4. Cancellation is supported at all levels
5. Results are tracked and stored for each task
6. Logging is comprehensive and categorized
7. Service provider is used for dependency injection
8. All operations are asynchronous where appropriate 