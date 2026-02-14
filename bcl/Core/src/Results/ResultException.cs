using System.Diagnostics;

namespace FrostYeti.Results;

/// <summary>
/// Represents the ResultException class.
/// </summary>
/// <remarks>
/// <example>
/// <code lang="csharp">
/// var instance = default(object);
/// </code>
/// </example>
/// </remarks>
public class ResultException : System.Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResultException"/> class.
    /// </summary>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var ex = new ResultException();
    /// Assert.Equal(string.Empty, ex.Message);
    /// </code>
    /// </example>
    /// </remarks>
    public ResultException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResultException"/> class with a message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var ex = new ResultException("Operation failed");
    /// Assert.Equal("Operation failed", ex.Message);
    /// </code>
    /// </example>
    /// </remarks>
    public ResultException(string? message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResultException"/> class with a message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="inner">The exception that is the cause of this exception.</param>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var inner = new InvalidOperationException("inner");
    /// var ex = new ResultException("outer", inner);
    /// Assert.Equal("outer", ex.Message);
    /// Assert.Same(inner, ex.InnerException);
    /// </code>
    /// </example>
    /// </remarks>
    public ResultException(string? message, System.Exception? inner)
        : base(message, inner)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResultException"/> class from an existing exception.
    /// </summary>
    /// <param name="ex">The exception to wrap.</param>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var inner = new InvalidOperationException("inner error");
    /// var ex = new ResultException(inner);
    /// Assert.Contains("Result error:", ex.Message);
    /// Assert.Same(inner, ex.InnerException);
    /// </code>
    /// </example>
    /// </remarks>
    public ResultException(System.Exception ex)
        : base($"Result error: {ex.Message}", ex)
    {
    }

    public virtual string Target { get; protected set; } = string.Empty;

    public virtual int LineNumber { get; protected set; }

    public virtual string FilePath { get; protected set; } = string.Empty;

    /// <summary>
    /// Creates a <see cref="ResultException"/> from an unknown error object.
    /// </summary>
    /// <param name="error">The error object to convert.</param>
    /// <param name="message">An optional message prefix.</param>
    /// <returns>A <see cref="ResultException"/> representing the error.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var ex = ResultException.FromUnknown("something went wrong", "Error:");
    /// Assert.Contains("Error:", ex.Message);
    /// </code>
    /// </example>
    /// </remarks>
    public static ResultException FromUnknown(object? error, string? message = null)
    {
        message ??= "Unknown error: ";
        if (error is Exception e)
            return new ResultException(e);

        if (error is System.Exception ex)
            return new ResultException(ex);

        if (error != null)
            return new ResultException($"{message} {error}");

        return new ResultException(message);
    }

    /// <summary>
    /// Creates a <see cref="ResultException"/> from an <see cref="Exception"/>.
    /// </summary>
    /// <param name="error">The exception to convert.</param>
    /// <returns>A <see cref="ResultException"/> wrapping the exception.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var inner = new InvalidOperationException("error");
    /// var ex = ResultException.FromError(inner);
    /// Assert.Contains("Result error:", ex.Message);
    /// </code>
    /// </example>
    /// </remarks>
    public static ResultException FromError(Exception error)
    {
        return new ResultException(error);
    }

    /// <summary>
    /// Creates a <see cref="ResultException"/> from a <see cref="System.Exception"/>.
    /// </summary>
    /// <param name="ex">The exception to convert.</param>
    /// <returns>A <see cref="ResultException"/> wrapping the exception.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var inner = new InvalidOperationException("error");
    /// var ex = ResultException.FromException(inner);
    /// Assert.Contains("Result error:", ex.Message);
    /// </code>
    /// </example>
    /// </remarks>
    public static ResultException FromException(System.Exception ex)
    {
        return new ResultException(ex);
    }

    public ResultException TrackCallerInfo(
        [System.Runtime.CompilerServices.CallerLineNumber] int line = 0,
        [System.Runtime.CompilerServices.CallerFilePath] string file = "",
        [System.Runtime.CompilerServices.CallerMemberName] string target = "")
    {
        this.Target = target;
        this.LineNumber = line;
        this.FilePath = file;
        return this;
    }
}