namespace FrostYeti.Results;

/// <summary>
/// Represents the IResult interface.
/// </summary>
/// <remarks>
/// <example>
/// <code lang="csharp">
/// var instance = default(object);
/// </code>
/// </example>
/// </remarks>
public interface IResult
{
    bool IsOk { get; }

    bool IsError { get; }

    object Error { get; }

    bool TryGetError(out object? error);
}

/// <summary>
/// Represents the IEmptyResult interface.
/// </summary>
/// <remarks>
/// <example>
/// <code lang="csharp">
/// var instance = default(object);
/// </code>
/// </example>
/// </remarks>
public interface IEmptyResult : IResult
{
}

/// <summary>
/// Represents the IValueResult interface.
/// </summary>
/// <remarks>
/// <example>
/// <code lang="csharp">
/// var instance = default(object);
/// </code>
/// </example>
/// </remarks>
public interface IValueResult : IResult
{
    object Value { get; }

    bool TryGetValue(out object? value);
}

/// <summary>
/// Represents the IResult interface.
/// </summary>
/// <typeparam name="TError">The type of the error.</typeparam>
/// <remarks>
/// <example>
/// <code lang="csharp">
/// var instance = default(object);
/// </code>
/// </example>
/// </remarks>
public interface IResult<TError> : IResult
    where TError : notnull
{
    new TError Error { get; }

    bool TryGetError(out TError? error);
}

/// <summary>
/// Represents the IEmptyResult interface.
/// </summary>
/// <typeparam name="TError">The type of the error.</typeparam>
/// <remarks>
/// <example>
/// <code lang="csharp">
/// var instance = default(object);
/// </code>
/// </example>
/// </remarks>
public interface IEmptyResult<TError> : IEmptyResult, IResult<TError>
    where TError : notnull
{
}

/// <summary>
/// Represents the IValueResult interface.
/// </summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
/// <typeparam name="TError">The type of the error.</typeparam>
/// <remarks>
/// <example>
/// <code lang="csharp">
/// var instance = default(object);
/// </code>
/// </example>
/// </remarks>
public interface IValueResult<TValue, TError> : IResult<TError>, IEquatable<IValueResult<TValue, TError>>, IValueResult
    where TValue : notnull
    where TError : notnull
{
    new TValue Value { get; }

    bool TryGetValue(out TValue? value);
}