using System.Runtime.Serialization;

namespace Cadtastic.JobHost.SDK.Exceptions;

/// <summary>
/// Exception thrown when there is an error loading a DLL file or its contents.
/// This exception provides detailed information about the loading failure and the affected DLL.
/// </summary>
[Serializable]
public class DllLoadException : Exception
{
    /// <summary>
    /// Gets the path of the DLL file that failed to load.
    /// </summary>
    public string DllPath { get; }

    /// <summary>
    /// Gets the specific reason for the DLL loading failure.
    /// </summary>
    public DllLoadFailureReason FailureReason { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DllLoadException"/> class.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="dllPath">The path of the DLL file that failed to load.</param>
    /// <param name="failureReason">The specific reason for the DLL loading failure.</param>
    /// <exception cref="ArgumentNullException">Thrown when message or dllPath is null.</exception>
    /// <exception cref="ArgumentException">Thrown when dllPath is empty or consists only of white-space characters.</exception>
    public DllLoadException(string message, string dllPath, DllLoadFailureReason failureReason)
        : base(message)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentNullException(nameof(message));
        if (string.IsNullOrWhiteSpace(dllPath))
            throw new ArgumentException("DLL path cannot be null or empty.", nameof(dllPath));

        DllPath = dllPath;
        FailureReason = failureReason;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DllLoadException"/> class with a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="dllPath">The path of the DLL file that failed to load.</param>
    /// <param name="failureReason">The specific reason for the DLL loading failure.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    /// <exception cref="ArgumentNullException">Thrown when message, dllPath, or innerException is null.</exception>
    /// <exception cref="ArgumentException">Thrown when dllPath is empty or consists only of white-space characters.</exception>
    public DllLoadException(string message, string dllPath, DllLoadFailureReason failureReason, Exception innerException)
        : base(message, innerException)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentNullException(nameof(message));
        if (string.IsNullOrWhiteSpace(dllPath))
            throw new ArgumentException("DLL path cannot be null or empty.", nameof(dllPath));
        if (innerException == null)
            throw new ArgumentNullException(nameof(innerException));

        DllPath = dllPath;
        FailureReason = failureReason;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DllLoadException"/> class with serialized data.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
    /// <exception cref="ArgumentNullException">Thrown when info is null.</exception>
    /// <exception cref="SerializationException">Thrown when the class name is null or <see cref="System.Exception.HResult"/> is zero (0).</exception>
    protected DllLoadException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        if (info == null)
            throw new ArgumentNullException(nameof(info));

        DllPath = info.GetString(nameof(DllPath)) ?? string.Empty;
        FailureReason = (DllLoadFailureReason)info.GetInt32(nameof(FailureReason));
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
        info.AddValue(nameof(DllPath), DllPath);
        info.AddValue(nameof(FailureReason), (int)FailureReason);
    }
}

/// <summary>
/// Specifies the reason for a DLL loading failure.
/// </summary>
public enum DllLoadFailureReason
{
    /// <summary>
    /// The DLL file was not found at the specified path.
    /// </summary>
    FileNotFound,

    /// <summary>
    /// The DLL file exists but could not be loaded due to an invalid format or corruption.
    /// </summary>
    InvalidFormat,

    /// <summary>
    /// The DLL file exists but could not be loaded due to missing dependencies.
    /// </summary>
    MissingDependencies,

    /// <summary>
    /// The DLL file exists but could not be loaded due to version incompatibility.
    /// </summary>
    VersionMismatch,

    /// <summary>
    /// The DLL file exists but could not be loaded due to security restrictions.
    /// </summary>
    SecurityRestriction,

    /// <summary>
    /// The DLL file exists but could not be loaded due to an unknown error.
    /// </summary>
    Unknown
}