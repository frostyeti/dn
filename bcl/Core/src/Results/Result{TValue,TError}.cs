// ReSharper disable ParameterHidesMember
namespace FrostYeti.Results;

/// <summary>
/// Represents the outcome of an operation that either succeeds with a value or fails with a typed error.
/// </summary>
/// <typeparam name="TValue">The success value type.</typeparam>
/// <typeparam name="TError">The error type.</typeparam>
/// <remarks>
/// <example>
/// <code lang="csharp">
/// Result&lt;int, string&gt; ok = Result&lt;int, string&gt;.Ok(42);
/// Result&lt;int, string&gt; failed = Result&lt;int, string&gt;.Fail("failed");
/// </code>
/// </example>
/// </remarks>
public class Result<TValue, TError> : IValueResult<TValue, TError>,
    IEquatable<Result<TValue, TError>>
    where TValue : notnull
    where TError : notnull
{
    private readonly TValue? value;

    private readonly TError? error;

    internal Result(TValue? value, TError? error, bool isOk)
    {
        this.value = value;
        this.error = error;
        this.IsOk = isOk;
        if (isOk && value is null)
            throw new ArgumentNullException(nameof(value), "Value cannot be null when result is ok.");
    }

    /// <summary>
    /// Gets a value indicating whether the result is successful.
    /// </summary>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var result = Result&lt;string, int&gt;.Ok("success");
    /// Assert.True(result.IsOk);
    /// </code>
    /// </example>
    /// </remarks>
    public bool IsOk { get; }

    /// <summary>
    /// Gets a value indicating whether the result is failed.
    /// </summary>
    public bool IsError => !this.IsOk;

    /// <summary>
    /// Gets the success value.
    /// </summary>
    /// <exception cref="ResultException">Thrown when the result is failed.</exception>
    public TValue Value
    {
        get
        {
            if (this.IsOk)
                return this.value!;

            throw new ResultException("No value present");
        }
    }

    /// <summary>
    /// Gets the error value.
    /// </summary>
    /// <exception cref="ResultException">Thrown when the result is successful.</exception>
    public TError Error
    {
        get
        {
            if (this.IsError)
                return this.error!;

            throw new ResultException("No error present");
        }
    }

    object IValueResult.Value => this.Value;

    object IResult.Error => this.Error;

    /// <summary>
    /// Converts a result into a completed <see cref="Task{TResult}"/>.
    /// </summary>
    /// <param name="result">The result to wrap.</param>
    /// <returns>A completed task containing <paramref name="result"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result&lt;int, string&gt; result = Result&lt;int, string&gt;.Ok(42);
    /// Task&lt;Result&lt;int, string&gt;&gt; task = result;
    /// </code>
    /// </example>
    /// </remarks>
    public static implicit operator Task<Result<TValue, TError>>(Result<TValue, TError> result)
        => Task.FromResult(result);

    /// <summary>
    /// Converts a result into a completed <see cref="ValueTask{TResult}"/>.
    /// </summary>
    /// <param name="result">The result to wrap.</param>
    /// <returns>A completed value task containing <paramref name="result"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result&lt;int, string&gt; result = Result&lt;int, string&gt;.Ok(42);
    /// ValueTask&lt;Result&lt;int, string&gt;&gt; task = result;
    /// </code>
    /// </example>
    /// </remarks>
    public static implicit operator ValueTask<Result<TValue, TError>>(Result<TValue, TError> result)
        => new(result);

    public static bool operator ==(Result<TValue, TError> left, Result<TValue, TError> right)
        => left.Equals(right);

    public static bool operator !=(Result<TValue, TError> left, Result<TValue, TError> right)
        => !left.Equals(right);

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <param name="value">The success value.</param>
    /// <returns>A successful result containing <paramref name="value"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result&lt;int, string&gt; result = Result&lt;int, string&gt;.Ok(42);
    /// </code>
    /// </example>
    /// </remarks>
    public static Result<TValue, TError> Ok(TValue value)
        => new(value, default, true);

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="error">The error value.</param>
    /// <returns>A failed result containing <paramref name="error"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result&lt;int, string&gt; result = Result&lt;int, string&gt;.Fail("failed");
    /// </code>
    /// </example>
    /// </remarks>
    public static Result<TValue, TError> Fail(TError error)
        => new(default, error, false);

    /// <summary>
    /// Returns the hash code for the current instance.
    /// </summary>
    /// <returns>A hash code for this result.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// int hash = Result&lt;int, string&gt;.Ok(42).GetHashCode();
    /// </code>
    /// </example>
    /// </remarks>
    public override int GetHashCode()
        => HashCode.Combine(
            this.IsOk,
            this.value,
            this.error);

    /// <summary>
    /// Determines whether the current result is equal to the specified object.
    /// </summary>
    /// <param name="obj">The object to compare with the current result.</param>
    /// <returns><see langword="true"/> when <paramref name="obj"/> represents the same state and values; otherwise <see langword="false"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// bool equal = Result&lt;int, string&gt;.Ok(42).Equals((object)Result&lt;int, string&gt;.Ok(42));
    /// </code>
    /// </example>
    /// </remarks>
    public override bool Equals(object? obj)
    {
        if (obj is null)
            return !this.IsOk;

        if (obj is Result<TValue, TError> other2)
            return this.Equals(other2);

        if (obj is IValueResult<TValue, TError> other)
            return this.Equals(other);

        return false;
    }

    /// <summary>
    /// Determines whether the current result is equal to another value result.
    /// </summary>
    /// <param name="other">The result to compare with the current result.</param>
    /// <returns><see langword="true"/> when both results represent the same state and value; otherwise <see langword="false"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// IValueResult&lt;int, string&gt; other = Result&lt;int, string&gt;.Ok(42);
    /// bool equal = Result&lt;int, string&gt;.Ok(42).Equals(other);
    /// </code>
    /// </example>
    /// </remarks>
    public bool Equals(IValueResult<TValue, TError>? other)
    {
        if (other is null)
            return !this.IsOk;

        if (!this.IsOk)
            return !other.IsOk;

        return this.value!.Equals(other.Value);
    }

    /// <summary>
    /// Determines whether the current result is equal to another result.
    /// </summary>
    /// <param name="other">The result to compare with the current result.</param>
    /// <returns><see langword="true"/> when both results represent the same state and value; otherwise <see langword="false"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// bool equal = Result&lt;int, string&gt;.Ok(42).Equals(Result&lt;int, string&gt;.Ok(42));
    /// </code>
    /// </example>
    /// </remarks>
    public bool Equals(Result<TValue, TError>? other)
    {
        if (other is null)
            return false;

        if (this.IsOk != other.IsOk)
            return false;

        if (this.IsOk)
            return this.value!.Equals(other.value);

        return this.error!.Equals(other.error);
    }

    /// <summary>
    /// Runs an action when the result is successful.
    /// </summary>
    /// <param name="inspect">The action to execute for the success value.</param>
    /// <returns>The current result.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result&lt;int, string&gt;.Ok(42).Inspect(value => Console.WriteLine(value));
    /// </code>
    /// </example>
    /// </remarks>
    public Result<TValue, TError> Inspect(Action<TValue> inspect)
    {
        if (this.IsOk)
            inspect(this.value!);

        return this;
    }

    /// <summary>
    /// Runs an action when the result is failed.
    /// </summary>
    /// <param name="inspect">The action to execute for the error value.</param>
    /// <returns>The current result.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result&lt;int, string&gt;.Fail("failed").InspectError(error => Console.WriteLine(error));
    /// </code>
    /// </example>
    /// </remarks>
    public Result<TValue, TError> InspectError(Action<TError> inspect)
    {
        if (this.IsError)
            inspect(this.error!);

        return this;
    }

    /// <summary>
    /// Determines whether the result is successful and its value matches a predicate.
    /// </summary>
    /// <param name="predicate">The predicate to evaluate.</param>
    /// <returns><see langword="true"/> when the result is successful and the predicate returns <see langword="true"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// bool matches = Result&lt;int, string&gt;.Ok(42).IsOkAnd(value => value &gt; 0);
    /// </code>
    /// </example>
    /// </remarks>
    public bool IsOkAnd(Func<TValue, bool> predicate)
        => this.IsOk && predicate(this.value!);

    /// <summary>
    /// Determines whether the result is failed and its error matches a predicate.
    /// </summary>
    /// <param name="predicate">The predicate to evaluate.</param>
    /// <returns><see langword="true"/> when the result is failed and the predicate returns <see langword="true"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// bool matches = Result&lt;int, string&gt;.Fail("failed").IsErrorAnd(error => error == "failed");
    /// </code>
    /// </example>
    /// </remarks>
    public bool IsErrorAnd(Func<TError, bool> predicate)
        => this.IsError && predicate(this.error!);

    /// <summary>
    /// Maps the success value to another success value type.
    /// </summary>
    /// <typeparam name="TOther">The mapped success type.</typeparam>
    /// <param name="map">The success mapping function.</param>
    /// <returns>A mapped result.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result&lt;string, string&gt; mapped = Result&lt;int, string&gt;.Ok(42).Map(value => value.ToString());
    /// </code>
    /// </example>
    /// </remarks>
    public Result<TOther, TError> Map<TOther>(Func<TValue, TOther> map)
        where TOther : notnull
        => this.IsOk ?
            new(map(this.value!), default, true) :
            new(default, this.error!, false);

    /// <summary>
    /// Maps both success and error values to another result type.
    /// </summary>
    /// <typeparam name="TOther">The mapped success type.</typeparam>
    /// <typeparam name="TOtherError">The mapped error type.</typeparam>
    /// <param name="map">The success mapping function.</param>
    /// <param name="mapError">The error mapping function.</param>
    /// <returns>A mapped result.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result&lt;string, int&gt; mapped = Result&lt;int, string&gt;.Fail("failed")
    ///     .Map(value => value.ToString(), error => error.Length);
    /// </code>
    /// </example>
    /// </remarks>
    public Result<TOther, TOtherError> Map<TOther, TOtherError>(Func<TValue, TOther> map, Func<TError, TOtherError> mapError)
        where TOther : notnull
        where TOtherError : notnull
        => this.IsOk ?
            new(map(this.value!), default, true) :
            new(default, mapError(this.error!), false);

    /// <summary>
    /// Returns the current result when successful; otherwise returns a successful result with the specified value.
    /// </summary>
    /// <param name="other">The fallback value.</param>
    /// <returns>The current result when successful; otherwise a successful fallback result.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result&lt;int, string&gt; result = Result&lt;int, string&gt;.Fail("failed").Or(42);
    /// </code>
    /// </example>
    /// </remarks>
    public Result<TValue, TError> Or(TValue other)
        => this.IsOk ? this : new(other, default, true);

    /// <summary>
    /// Returns the current result when successful; otherwise returns a successful result with a lazily created value.
    /// </summary>
    /// <param name="other">The fallback value factory.</param>
    /// <returns>The current result when successful; otherwise a successful fallback result.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result&lt;int, string&gt; result = Result&lt;int, string&gt;.Fail("failed").Or(() => 42);
    /// </code>
    /// </example>
    /// </remarks>
    public Result<TValue, TError> Or(Func<TValue> other)
        => this.IsOk ? this : new(other(), default, true);

    /// <summary>
    /// Returns the success value or a fallback value.
    /// </summary>
    /// <param name="other">The fallback value.</param>
    /// <returns>The success value when available; otherwise <paramref name="other"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// int value = Result&lt;int, string&gt;.Fail("failed").OrDefault(0);
    /// </code>
    /// </example>
    /// </remarks>
    public TValue OrDefault(TValue other)
        => this.IsOk ? this.value! : other;

    /// <summary>
    /// Returns the success value or a lazily created fallback value.
    /// </summary>
    /// <param name="other">The fallback value factory.</param>
    /// <returns>The success value when available; otherwise the fallback value.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// int value = Result&lt;int, string&gt;.Fail("failed").OrDefault(() => 0);
    /// </code>
    /// </example>
    /// </remarks>
    public TValue OrDefault(Func<TValue> other)
        => this.IsOk ? this.value! : other();

    /// <summary>
    /// Returns the current result when failed; otherwise returns a failed result with the specified error.
    /// </summary>
    /// <param name="error">The fallback error.</param>
    /// <returns>The current failed result or a failed fallback result.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result&lt;int, string&gt; result = Result&lt;int, string&gt;.Ok(42).OrError("forced");
    /// </code>
    /// </example>
    /// </remarks>
    public Result<TValue, TError> OrError(TError error)
        => this.IsError ? this : new(default, error, false);

    /// <summary>
    /// Returns the current result when failed; otherwise returns a failed result with a lazily created error.
    /// </summary>
    /// <param name="error">The fallback error factory.</param>
    /// <returns>The current failed result or a failed fallback result.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result&lt;int, string&gt; result = Result&lt;int, string&gt;.Ok(42).OrError(() => "forced");
    /// </code>
    /// </example>
    /// </remarks>
    public Result<TValue, TError> OrError(Func<TError> error)
        => this.IsError ? this : new(default, error(), false);

    /// <summary>
    /// Returns the error value or a fallback error value.
    /// </summary>
    /// <param name="other">The fallback error.</param>
    /// <returns>The error value when failed; otherwise <paramref name="other"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// string error = Result&lt;int, string&gt;.Ok(42).OrErrorDefault("fallback");
    /// </code>
    /// </example>
    /// </remarks>
    public TError OrErrorDefault(TError other)
        => this.IsError ? this.error! : other;

    /// <summary>
    /// Returns the error value or a lazily created fallback error value.
    /// </summary>
    /// <param name="other">The fallback error factory.</param>
    /// <returns>The error value when failed; otherwise the fallback error value.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// string error = Result&lt;int, string&gt;.Ok(42).OrErrorDefault(() => "fallback");
    /// </code>
    /// </example>
    /// </remarks>
    public TError OrErrorDefault(Func<TError> other)
        => this.IsError ? this.error! : other();

    bool IValueResult.TryGetValue(out object? value)
    {
        var res = this.TryGetValue(out var v);
        value = v;
        return res;
    }

    bool IResult.TryGetError(out object? error)
    {
        var res = this.TryGetError(out var e);
        error = e;
        return res;
    }

    /// <summary>
    /// Attempts to retrieve the success value.
    /// </summary>
    /// <param name="value">When this method returns, contains the value if successful; otherwise <see langword="null"/>.</param>
    /// <returns><see langword="true"/> when the result is successful; otherwise <see langword="false"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// bool hasValue = Result&lt;int, string&gt;.Ok(42).TryGetValue(out int? value);
    /// </code>
    /// </example>
    /// </remarks>
    public bool TryGetValue(out TValue? value)
    {
        value = this.value;
        return this.IsOk;
    }

    /// <summary>
    /// Attempts to retrieve the error value.
    /// </summary>
    /// <param name="error">When this method returns, contains the error if failed; otherwise <see langword="null"/>.</param>
    /// <returns><see langword="true"/> when the result is failed; otherwise <see langword="false"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// bool hasError = Result&lt;int, string&gt;.Fail("failed").TryGetError(out string? error);
    /// </code>
    /// </example>
    /// </remarks>
    public bool TryGetError(out TError? error)
    {
        error = this.error;
        return this.IsError;
    }
}