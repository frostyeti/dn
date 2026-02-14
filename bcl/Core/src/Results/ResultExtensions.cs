namespace FrostYeti.Results;

/// <summary>
/// Provides helper methods for working with result types.
/// </summary>
/// <remarks>
/// <example>
/// <code lang="csharp">
/// var ok = ValueResult.OkRef&lt;int, string&gt;(42);
/// var isPositive = ok.IsOkAnd(value =&gt; value &gt; 0);
/// </code>
/// </example>
/// </remarks>
public static class ResultExtensions
{
    /// <summary>
    /// Returns <see langword="true"/> when the result is successful and the value matches the predicate.
    /// </summary>
    /// <typeparam name="TValue">The successful result value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="r">The result to evaluate.</param>
    /// <param name="predicate">The predicate used to validate the success value.</param>
    /// <returns><see langword="true"/> when the result is ok and the predicate returns <see langword="true"/>; otherwise, <see langword="false"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var result = ValueResult.OkRef&lt;int, string&gt;(10);
    /// var isEven = result.IsOkAnd(value =&gt; value % 2 == 0);
    /// </code>
    /// </example>
    /// </remarks>
    public static bool IsOkAnd<TValue, TError>(this IValueResult<TValue, TError> r, Func<TValue, bool> predicate)
        where TValue : notnull
        where TError : notnull
        => r.IsOk && predicate(r.Value);

    /// <summary>
    /// Returns <see langword="true"/> when the result is an error and the error matches the predicate.
    /// </summary>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="r">The result to evaluate.</param>
    /// <param name="predicate">The predicate used to validate the error value.</param>
    /// <returns><see langword="true"/> when the result is an error and the predicate returns <see langword="true"/>; otherwise, <see langword="false"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var result = ValueResult.ErrorRef&lt;int, string&gt;("boom");
    /// var hasBoom = result.IsErrorAnd(error =&gt; error == "boom");
    /// </code>
    /// </example>
    /// </remarks>
    public static bool IsErrorAnd<TError>(this IResult<TError> r, Func<TError, bool> predicate)
        where TError : notnull
        => r.IsError && predicate(r.Error);

    /// <summary>
    /// Returns the success value or throws a <see cref="ResultException"/> with the provided message.
    /// </summary>
    /// <typeparam name="TValue">The successful result value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="r">The result to unwrap.</param>
    /// <param name="message">The exception message used when the result is an error.</param>
    /// <returns>The success value when the result is ok.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var result = ValueResult.OkRef&lt;string, string&gt;("ready");
    /// var value = result.Expect("expected success");
    /// </code>
    /// </example>
    /// </remarks>
    public static TValue Expect<TValue, TError>(this IValueResult<TValue, TError> r, string message)
        where TValue : notnull
        where TError : notnull
    {
        if (r.IsOk)
            return r.Value;

        throw new ResultException(message);
    }

    /// <summary>
    /// Returns the error value or throws a <see cref="ResultException"/> with the provided message.
    /// </summary>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="r">The result to unwrap.</param>
    /// <param name="message">The exception message used when the result is successful.</param>
    /// <returns>The error value when the result is an error.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var result = ValueResult.ErrorRef&lt;int, string&gt;("failed");
    /// var error = result.ExpectError("expected error");
    /// </code>
    /// </example>
    /// </remarks>
    public static TError ExpectError<TError>(this IResult<TError> r, string message)
        where TError : notnull
    {
        if (r.IsError)
            return r.Error;

        throw new ResultException(message);
    }
}