using System.Collections.Concurrent;
using System.Reflection;

using Cadtastic.JobHost.SDK.Interfaces;
using Cadtastic.JobHost.SDK.Models;

using Microsoft.Extensions.DependencyInjection;

namespace Cadtastic.JobHost.SDK.Registry;

/// <summary>
/// Registry for managing job discovery and lazy loading.
/// Discovers job metadata at startup and loads actual job instances on-demand.
/// </summary>
public class JobRegistry
{
    private readonly ConcurrentDictionary<string, JobDescriptor> _jobDescriptors = new();
    private readonly ConcurrentDictionary<string, IJob> _loadedJobs = new();
    private readonly IServiceCollection _services;
    private readonly string _jobDirectory;

    /// <summary>
    /// Initializes a new instance of the <see cref="JobRegistry"/> class.
    /// </summary>
    /// <param name="services">The service collection for registering job services.</param>
    /// <param name="jobDirectory">The directory to scan for job assemblies.</param>
    public JobRegistry(IServiceCollection services, string jobDirectory)
    {
        _services = services;
        _jobDirectory = jobDirectory;
    }

    /// <summary>
    /// Discovers jobs in the jobs directory using safe reflection-only loading.
    /// This method scans DLL files and extracts job metadata without loading dependencies.
    /// </summary>
    public void DiscoverJobs()
    {
        _jobDescriptors.Clear();
        Console.WriteLine($"Starting job discovery in: {_jobDirectory}");

        if (!Directory.Exists(_jobDirectory))
        {
            Console.WriteLine($"Job directory does not exist: {_jobDirectory}");
            return;
        }

        var dllFiles = Directory.GetFiles(_jobDirectory, "*.dll", SearchOption.AllDirectories);
        Console.WriteLine($"Found {dllFiles.Length} DLL files to analyze");

        foreach (var dllPath in dllFiles)
        {
            try
            {
                // Skip known dependency DLLs
                var fileName = Path.GetFileName(dllPath);
                if (IsKnownDependency(fileName))
                {
                    Console.WriteLine($"Skipping known dependency DLL: {fileName}");
                    continue;
                }

                // Use safe discovery for job DLLs
                if (TryDiscoverJobsInAssembly(dllPath, fileName))
                {
                    Console.WriteLine($"Successfully analyzed job assembly: {fileName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error analyzing {dllPath}: {ex.Message}");
            }
        }

        Console.WriteLine($"Job discovery completed. Found {_jobDescriptors.Count} jobs from {dllFiles.Length} DLL files.");
    }

    /// <summary>
    /// Gets all available job descriptors for listing purposes.
    /// </summary>
    /// <returns>Collection of job descriptors.</returns>
    public IEnumerable<JobDescriptor> GetAvailableJobs()
    {
        return _jobDescriptors.Values;
    }

    /// <summary>
    /// Gets a job instance, loading it if not already loaded.
    /// </summary>
    /// <param name="jobType">The job type identifier.</param>
    /// <returns>The job instance.</returns>
    public IJob GetJob(string jobType)
    {
        return _loadedJobs.GetOrAdd(jobType, type => LoadJob(type));
    }

    /// <summary>
    /// Checks if a job type is available.
    /// </summary>
    /// <param name="jobType">The job type identifier.</param>
    /// <returns>True if the job is available, false otherwise.</returns>
    public bool HasJob(string jobType)
    {
        return _jobDescriptors.ContainsKey(jobType);
    }

    /// <summary>
    /// Loads a job instance and registers its services.
    /// </summary>
    /// <param name="jobType">The job type identifier.</param>
    /// <returns>The loaded job instance.</returns>
    private IJob LoadJob(string jobType)
    {
        if (!_jobDescriptors.TryGetValue(jobType, out var descriptor))
        {
            throw new InvalidOperationException($"Job type '{jobType}' not found.");
        }

       // Console.WriteLine($"Loading job: {descriptor.Name} ({jobType})");

        try
        {
            // Load the assembly and create the job instance
            var assembly = Assembly.LoadFrom(descriptor.AssemblyPath);
            var type = assembly.GetType(descriptor.TypeName) ?? 
                throw new InvalidOperationException($"Could not find type '{descriptor.TypeName}' in assembly '{descriptor.AssemblyPath}'");

            var job = (IJob)Activator.CreateInstance(type)! ?? 
                throw new InvalidOperationException($"Could not create instance of job type '{jobType}'");

            // Register the job's services
            job.RegisterJobServices(_services);

            return job;
        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Safely discovers jobs in a single assembly using MetadataLoadContext.
    /// </summary>
    /// <param name="dllPath">Path to the DLL file.</param>
    /// <param name="fileName">Name of the DLL file.</param>
    /// <returns>True if jobs were found, false otherwise.</returns>
    private bool TryDiscoverJobsInAssembly(string dllPath, string fileName)
    {
        try
        {
            // Create a comprehensive list of assemblies for the MetadataLoadContext
            var assemblyPaths = new List<string> { dllPath };
            
            // Add core .NET assemblies from the runtime directory
            var runtimeDir = Path.GetDirectoryName(typeof(object).Assembly.Location);
            if (!string.IsNullOrEmpty(runtimeDir))
            {
                // Add essential core assemblies
                var coreAssemblies = new[]
                {
                    "System.Private.CoreLib.dll",
                    "System.Runtime.dll", 
                    "System.Collections.dll",
                    "System.Linq.dll",
                    "System.ComponentModel.dll",
                    "netstandard.dll"
                };

                foreach (var coreAssembly in coreAssemblies)
                {
                    var corePath = Path.Combine(runtimeDir, coreAssembly);
                    if (File.Exists(corePath))
                    {
                        assemblyPaths.Add(corePath);
                    }
                }
            }

            // Add other DLLs from the jobs directory that might be dependencies
            var jobDirDlls = Directory.GetFiles(_jobDirectory, "*.dll", SearchOption.AllDirectories);
            assemblyPaths.AddRange(jobDirDlls.Where(p => p != dllPath));

            // Create a temporary MetadataLoadContext for reflection-only loading
            var resolver = new PathAssemblyResolver(assemblyPaths);
            using var context = new MetadataLoadContext(resolver, "System.Private.CoreLib");
            
            var assembly = context.LoadFromAssemblyPath(dllPath);
            bool foundJobs = false;

            foreach (var type in assembly.GetTypes())
            {
                // Check if the type has a JobAttribute
                var jobAttribute = type.GetCustomAttributesData()
                    .FirstOrDefault(attr => attr.AttributeType.Name == "JobAttribute");

                if (jobAttribute != null)
                {
                    // Extract job information from the attribute
                    var jobType = GetAttributeValue(jobAttribute, 0) as string ?? type.Name;
                    var name = GetAttributeProperty(jobAttribute, "Name") as string ?? CreateFriendlyName(type.Name);
                    var description = GetAttributeProperty(jobAttribute, "Description") as string ?? $"Job implementation for {type.Name}";
                    var version = GetAttributeProperty(jobAttribute, "Version") as string ?? "1.0.0";

                    var descriptor = new JobDescriptor
                    {
                        JobType = jobType,
                        Name = name,
                        Description = description,
                        Version = version,
                        AssemblyPath = dllPath,
                        TypeName = type.FullName!,
                        AssemblyName = assembly.GetName().Name!,
                        DiscoveredAt = DateTime.UtcNow
                    };

                    _jobDescriptors.TryAdd(descriptor.JobType, descriptor);
                    Console.WriteLine($"Successfully discovered job: {descriptor.Name} from {fileName}");
                    foundJobs = true;
                }
            }

            return foundJobs;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during metadata analysis of {fileName}: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Gets a positional argument value from a CustomAttributeData.
    /// </summary>
    private static object? GetAttributeValue(CustomAttributeData attributeData, int index)
    {
        if (attributeData.ConstructorArguments.Count > index)
        {
            return attributeData.ConstructorArguments[index].Value;
        }
        return null;
    }

    /// <summary>
    /// Gets a named property value from a CustomAttributeData.
    /// </summary>
    private static object? GetAttributeProperty(CustomAttributeData attributeData, string propertyName)
    {
        var namedArg = attributeData.NamedArguments.FirstOrDefault(arg => arg.MemberName == propertyName);
        return namedArg.TypedValue.Value;
    }

    /// <summary>
    /// Checks if a DLL is a known dependency that should be skipped during job discovery.
    /// </summary>
    /// <param name="fileName">The DLL file name.</param>
    /// <returns>True if it's a known dependency, false otherwise.</returns>
    private static bool IsKnownDependency(string fileName)
    {
        var knownDependencies = new[]
        {
            "Cadtastic.JobHost.SDK.dll",
            "EndpointManager.dll",
            "FluentFTP.dll",
            "Microsoft.Extensions.Configuration.Abstractions.dll",
            "Microsoft.Extensions.Configuration.Binder.dll",
            "Microsoft.Extensions.Configuration.dll",
            "Microsoft.Extensions.Configuration.FileExtensions.dll",
            "Microsoft.Extensions.Configuration.Json.dll",
            "Microsoft.Extensions.DependencyInjection.Abstractions.dll",
            "Microsoft.Extensions.DependencyInjection.dll",
            "Microsoft.Extensions.Diagnostics.Abstractions.dll",
            "Microsoft.Extensions.Diagnostics.dll",
            "Microsoft.Extensions.FileProviders.Abstractions.dll",
            "Microsoft.Extensions.FileProviders.Physical.dll",
            "Microsoft.Extensions.FileSystemGlobbing.dll",
            "Microsoft.Extensions.Http.dll",
            "Microsoft.Extensions.Logging.Abstractions.dll",
            "Microsoft.Extensions.Logging.dll",
            "Microsoft.Extensions.Options.ConfigurationExtensions.dll",
            "Microsoft.Extensions.Options.dll",
            "Microsoft.Extensions.Primitives.dll",
            "Newtonsoft.Json.dll",
            "Polly.dll",
            "System.Diagnostics.DiagnosticSource.dll"
        };

        return knownDependencies.Contains(fileName);
    }

    /// <summary>
    /// Creates a friendly name from a type name.
    /// </summary>
    /// <param name="typeName">The type name to convert.</param>
    /// <returns>A friendly name with spaces between words.</returns>
    private static string CreateFriendlyName(string typeName)
    {
        // Remove common suffixes
        var name = typeName;
        if (name.EndsWith("Job"))
            name = name.Substring(0, name.Length - 3);

        // Add spaces between words (simple camelCase handling)
        var result = System.Text.RegularExpressions.Regex.Replace(
            name,
            "([a-z])([A-Z])",
            "$1 $2"
        );

        return result.Trim();
    }
} 
