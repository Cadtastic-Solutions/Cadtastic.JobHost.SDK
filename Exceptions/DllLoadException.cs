namespace Cadtastic.JobHost.SDK.Exceptions
{
    /// <summary>
    /// Exception thrown when there is an error loading or unloading a DLL.
    /// </summary>
    public class DllLoadException : Exception
    {
        /// <summary>
        /// Gets the path of the DLL that caused the error.
        /// </summary>
        public string? DllPath { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DllLoadException"/> class.
        /// </summary>
        public DllLoadException() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DllLoadException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DllLoadException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DllLoadException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public DllLoadException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DllLoadException"/> class with a specified DLL path and error message.
        /// </summary>
        /// <param name="dllPath">The path of the DLL that caused the error.</param>
        /// <param name="message">The message that describes the error.</param>
        public DllLoadException(string dllPath, string message) : base(message) { DllPath = dllPath; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DllLoadException"/> class with a specified DLL path, error message, and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="dllPath">The path of the DLL that caused the error.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public DllLoadException(string dllPath, string message, Exception innerException) : base(message, innerException) { DllPath = dllPath; }
    }
}