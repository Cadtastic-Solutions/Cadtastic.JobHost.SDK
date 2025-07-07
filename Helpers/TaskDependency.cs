using Cadtastic.JobHost.SDK.Interfaces;

namespace Cadtastic.JobHost.SDK.Helpers;

/// <summary>
/// Provides helper methods for working with task dependencies.
/// This class contains utility methods for managing and analyzing task dependencies in a job.
/// </summary>
public static class TaskDependency
{
    /// <summary>
    /// Validates that the task dependencies form a valid directed acyclic graph (DAG).
    /// A valid DAG ensures that there are no circular dependencies between tasks.
    /// </summary>
    /// <param name="tasks">The collection of tasks to validate.</param>
    /// <returns>True if the dependencies form a valid DAG; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when tasks is null.</exception>
    public static bool ValidateDependencies(IEnumerable<ITaskExecutor> tasks)
    {
        if (tasks == null)
            throw new ArgumentNullException(nameof(tasks));

        var visited = new HashSet<string>();
        var recursionStack = new HashSet<string>();

        foreach (var task in tasks)
        {
            if (!visited.Contains(task.TaskId))
            {
                if (HasCycle(task, tasks, visited, recursionStack))
                    return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Gets a collection of tasks that can be executed immediately (no dependencies).
    /// </summary>
    /// <param name="tasks">The collection of tasks to analyze.</param>
    /// <returns>A collection of tasks that can be executed immediately.</returns>
    /// <exception cref="ArgumentNullException">Thrown when tasks is null.</exception>
    public static IEnumerable<ITaskExecutor> GetExecutableTasks(IEnumerable<ITaskExecutor> tasks)
    {
        if (tasks == null)
            throw new ArgumentNullException(nameof(tasks));

        return tasks.Where(t => !t.Dependencies.Any());
    }

    /// <summary>
    /// Gets a collection of tasks that are blocked by dependencies.
    /// </summary>
    /// <param name="tasks">The collection of tasks to analyze.</param>
    /// <returns>A collection of tasks that are blocked by dependencies.</returns>
    /// <exception cref="ArgumentNullException">Thrown when tasks is null.</exception>
    public static IEnumerable<ITaskExecutor> GetBlockedTasks(IEnumerable<ITaskExecutor> tasks)
    {
        if (tasks == null)
            throw new ArgumentNullException(nameof(tasks));

        return tasks.Where(t => t.Dependencies.Any());
    }

    /// <summary>
    /// Gets a collection of tasks that depend on the specified task type.
    /// </summary>
    /// <param name="tasks">The collection of tasks to analyze.</param>
    /// <param name="taskType">The type of the task to find dependents for.</param>
    /// <returns>A collection of tasks that depend on the specified task type.</returns>
    /// <exception cref="ArgumentNullException">Thrown when tasks is null.</exception>
    /// <exception cref="ArgumentNullException">Thrown when taskType is null.</exception>
    public static IEnumerable<ITaskExecutor> GetDependentTasks(IEnumerable<ITaskExecutor> tasks, Type taskType)
    {
        if (tasks == null)
            throw new ArgumentNullException(nameof(tasks));
        if (taskType == null)
            throw new ArgumentNullException(nameof(taskType));

        return tasks.Where(t => t.Dependencies.Contains(taskType));
    }

    /// <summary>
    /// Gets a collection of task types that the specified task depends on.
    /// </summary>
    /// <param name="tasks">The collection of tasks to analyze.</param>
    /// <param name="taskType">The type of the task to find dependencies for.</param>
    /// <returns>A collection of task instances that the specified task type depends on.</returns>
    /// <exception cref="ArgumentNullException">Thrown when tasks is null.</exception>
    /// <exception cref="ArgumentNullException">Thrown when taskType is null.</exception>
    public static IEnumerable<ITaskExecutor> GetDependencyTasks(IEnumerable<ITaskExecutor> tasks, Type taskType)
    {
        if (tasks == null)
            throw new ArgumentNullException(nameof(tasks));
        if (taskType == null)
            throw new ArgumentNullException(nameof(taskType));

        var task = tasks.FirstOrDefault(t => t.GetType() == taskType);
        if (task == null)
            return Enumerable.Empty<ITaskExecutor>();

        return tasks.Where(t => task.Dependencies.Contains(t.GetType()));
    }

    /// <summary>
    /// Gets a collection of tasks that have no dependencies.
    /// </summary>
    /// <param name="tasks">The collection of tasks to analyze.</param>
    /// <returns>A collection of tasks that have no dependencies.</returns>
    /// <exception cref="ArgumentNullException">Thrown when tasks is null.</exception>
    public static IEnumerable<ITaskExecutor> GetIndependentTasks(IEnumerable<ITaskExecutor> tasks)
    {
        if (tasks == null)
            throw new ArgumentNullException(nameof(tasks));

        return tasks.Where(t => !t.Dependencies.Any());
    }

    /// <summary>
    /// Gets a collection of tasks that have dependencies.
    /// </summary>
    /// <param name="tasks">The collection of tasks to analyze.</param>
    /// <returns>A collection of tasks that have dependencies.</returns>
    /// <exception cref="ArgumentNullException">Thrown when tasks is null.</exception>
    public static IEnumerable<ITaskExecutor> GetDependentTasks(IEnumerable<ITaskExecutor> tasks)
    {
        if (tasks == null)
            throw new ArgumentNullException(nameof(tasks));

        return tasks.Where(t => t.Dependencies.Any());
    }

    /// <summary>
    /// Gets a collection of tasks that can be executed in parallel.
    /// Tasks can be executed in parallel if they have no dependencies on each other.
    /// </summary>
    /// <param name="tasks">The collection of tasks to analyze.</param>
    /// <returns>A collection of tasks that can be executed in parallel.</returns>
    /// <exception cref="ArgumentNullException">Thrown when tasks is null.</exception>
    public static IEnumerable<ITaskExecutor> GetParallelTasks(IEnumerable<ITaskExecutor> tasks)
    {
        if (tasks == null)
            throw new ArgumentNullException(nameof(tasks));

        var taskList = tasks.ToList();
        var parallelTasks = new List<ITaskExecutor>();

        foreach (var task in taskList)
        {
            if (task.CanRunConcurrently)
            {
                var dependencies = GetDependencyTasks(taskList, task.GetType());
                if (!dependencies.Any(d => !d.CanRunConcurrently))
                {
                    parallelTasks.Add(task);
                }
            }
        }

        return parallelTasks;
    }

    /// <summary>
    /// Gets a collection of tasks that must be executed sequentially.
    /// Tasks must be executed sequentially if they have dependencies on each other
    /// or if they cannot run concurrently.
    /// </summary>
    /// <param name="tasks">The collection of tasks to analyze.</param>
    /// <returns>A collection of tasks that must be executed sequentially.</returns>
    /// <exception cref="ArgumentNullException">Thrown when tasks is null.</exception>
    public static IEnumerable<ITaskExecutor> GetSequentialTasks(IEnumerable<ITaskExecutor> tasks)
    {
        if (tasks == null)
            throw new ArgumentNullException(nameof(tasks));

        var taskList = tasks.ToList();
        var sequentialTasks = new List<ITaskExecutor>();

        foreach (var task in taskList)
        {
            if (!task.CanRunConcurrently)
            {
                sequentialTasks.Add(task);
            }
            else
            {
                var dependencies = GetDependencyTasks(taskList, task.GetType());
                if (dependencies.Any(d => !d.CanRunConcurrently))
                {
                    sequentialTasks.Add(task);
                }
            }
        }

        return sequentialTasks;
    }

    /// <summary>
    /// Gets a collection of tasks that can be executed immediately and in parallel.
    /// </summary>
    /// <param name="tasks">The collection of tasks to analyze.</param>
    /// <returns>A collection of tasks that can be executed immediately and in parallel.</returns>
    /// <exception cref="ArgumentNullException">Thrown when tasks is null.</exception>
    public static IEnumerable<ITaskExecutor> GetImmediateParallelTasks(IEnumerable<ITaskExecutor> tasks)
    {
        if (tasks == null)
            throw new ArgumentNullException(nameof(tasks));

        var taskList = tasks.ToList();
        var immediateParallelTasks = new List<ITaskExecutor>();

        foreach (var task in taskList)
        {
            if (task.CanRunConcurrently && !task.Dependencies.Any())
            {
                immediateParallelTasks.Add(task);
            }
        }

        return immediateParallelTasks;
    }

    /// <summary>
    /// Gets a collection of tasks that can be executed immediately but must be executed sequentially.
    /// </summary>
    /// <param name="tasks">The collection of tasks to analyze.</param>
    /// <returns>A collection of tasks that can be executed immediately but must be executed sequentially.</returns>
    /// <exception cref="ArgumentNullException">Thrown when tasks is null.</exception>
    public static IEnumerable<ITaskExecutor> GetImmediateSequentialTasks(IEnumerable<ITaskExecutor> tasks)
    {
        if (tasks == null)
            throw new ArgumentNullException(nameof(tasks));

        var taskList = tasks.ToList();
        var immediateSequentialTasks = new List<ITaskExecutor>();

        foreach (var task in taskList)
        {
            if (!task.CanRunConcurrently && !task.Dependencies.Any())
            {
                immediateSequentialTasks.Add(task);
            }
        }

        return immediateSequentialTasks;
    }

    /// <summary>
    /// Gets a collection of tasks that are blocked by the specified task type.
    /// A task is blocked if it depends on the specified task type or any of its dependencies.
    /// </summary>
    /// <param name="tasks">The collection of tasks to analyze.</param>
    /// <param name="taskType">The type of the task to find blocked tasks for.</param>
    /// <returns>A collection of tasks that are blocked by the specified task type.</returns>
    /// <exception cref="ArgumentNullException">Thrown when tasks is null.</exception>
    /// <exception cref="ArgumentNullException">Thrown when taskType is null.</exception>
    public static IEnumerable<ITaskExecutor> GetBlockedTasks(IEnumerable<ITaskExecutor> tasks, Type taskType)
    {
        if (tasks == null)
            throw new ArgumentNullException(nameof(tasks));
        if (taskType == null)
            throw new ArgumentNullException(nameof(taskType));

        var taskList = tasks.ToList();
        var blockedTasks = new HashSet<ITaskExecutor>();
        var visited = new HashSet<Type>();

        void FindBlockedTasks(Type currentTaskType)
        {
            if (visited.Contains(currentTaskType))
                return;

            visited.Add(currentTaskType);
            var dependents = GetDependentTasks(taskList, currentTaskType);

            foreach (var dependent in dependents)
            {
                blockedTasks.Add(dependent);
                FindBlockedTasks(dependent.GetType());
            }
        }

        FindBlockedTasks(taskType);
        return blockedTasks;
    }

    /// <summary>
    /// Gets a collection of tasks that block the specified task.
    /// A task is blocking if the specified task depends on it or any of its dependencies.
    /// </summary>
    /// <param name="tasks">The collection of tasks to analyze.</param>
    /// <param name="taskType">The type of the task to find blocking tasks for.</param>
    /// <returns>A collection of tasks that block the specified task.</returns>
    /// <exception cref="ArgumentNullException">Thrown when tasks is null.</exception>
    /// <exception cref="ArgumentNullException">Thrown when taskType is null.</exception>
    public static IEnumerable<ITaskExecutor> GetBlockingTasks(IEnumerable<ITaskExecutor> tasks, Type taskType)
    {
        if (tasks == null)
            throw new ArgumentNullException(nameof(tasks));
        if (taskType == null)
            throw new ArgumentNullException(nameof(taskType));

        var taskList = tasks.ToList();
        var blockingTasks = new HashSet<ITaskExecutor>();
        var visited = new HashSet<Type>();

        void FindBlockingTasks(Type currentTaskType)
        {
            if (visited.Contains(currentTaskType))
                return;

            visited.Add(currentTaskType);
            var dependencies = GetDependencyTasks(taskList, currentTaskType);

            foreach (var dependency in dependencies)
            {
                blockingTasks.Add(dependency);
                FindBlockingTasks(dependency.GetType());
            }
        }

        FindBlockingTasks(taskType);
        return blockingTasks;
    }

    private static bool HasCycle(ITaskExecutor task, IEnumerable<ITaskExecutor> allTasks, HashSet<string> visited, HashSet<string> recursionStack)
    {
        if (recursionStack.Contains(task.TaskId))
            return true;

        if (visited.Contains(task.TaskId))
            return false;

        visited.Add(task.TaskId);
        recursionStack.Add(task.TaskId);

        foreach (var dependencyType in task.Dependencies)
        {
            var dependency = allTasks.FirstOrDefault(t => t.GetType() == dependencyType);
            if (dependency != null && HasCycle(dependency, allTasks, visited, recursionStack))
                return true;
        }

        recursionStack.Remove(task.TaskId);
        return false;
    }

    /// <summary>
    /// Validates that dependencies do not contain the specified executor type.
    /// </summary>
    /// <param name="dependencies">The collection of dependencies to validate.</param>
    /// <param name="executorType">The executor type to check for.</param>
    /// <exception cref="InvalidOperationException">Thrown when the executor type is found in its own dependencies.</exception>
    private static void ValidateNoCyclicalDependency(IReadOnlyCollection<Type> dependencies, Type executorType)
    {
        if (dependencies.Any(d => d == executorType))
        {
            throw new InvalidOperationException($"Task executor {executorType.Name} cannot depend on itself.");
        }
    }

    /// <summary>
    /// Validates that dependencies do not contain the specified executor type.
    /// </summary>
    /// <param name="dependencies">The collection of dependencies to validate.</param>
    /// <param name="executorType">The executor type to check for.</param>
    /// <exception cref="InvalidOperationException">Thrown when the executor type is found in its own dependencies.</exception>
    private static void ValidateNoCyclicalDependency(IReadOnlyCollection<Type> dependencies, Type executorType, string taskName)
    {
        if (dependencies.Any(d => d == executorType))
        {
            throw new InvalidOperationException($"Task executor {taskName} cannot depend on itself.");
        }
    }
} 