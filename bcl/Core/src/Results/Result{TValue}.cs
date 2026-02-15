// ReSharper disable ParameterHidesMember
namespace FrostYeti.Results;

/// <summary>
/// Represents the outcome of an operation that either succeeds with a value or fails with an <see cref="Exception"/>.
/// </summary>
/// <typeparam name="TValue">The success value type.</typeparam>
/// <remarks>
/// <example>
/// <code lang="csharp">
/// Result&lt;int&gt; ok = new Result&lt;int&gt;(42);
/// Result&lt;int&gt; failed = new Result&lt;int&gt;(new InvalidOperationException("failed"));
/// </code>
/// </example>
/// </remarks>
public class Result<TValue> : IValueResult<TValue, Exception>,
    IEquatable<Result<TValue>>
    where TValue : notnull
{
    private readonly TValue? value;

    private readonly Exception? error;

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TValue}"/> class using the default value for <typeparamref name="TValue"/>.
    /// </summary>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var result = new Result&lt;string&gt;();
    /// bool isError = result.IsError;
    /// </code>
    /// </example>
    /// </remarks>
    public Result()
    {
        this.value = default;
        this.IsOk = this.value is not null;
        if (this.IsError)
            this.error = new ResultException("No value present");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TValue}"/> class in a successful state.
    /// </summary>
    /// <param name="value">The success value.</param>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var result = new Result&lt;int&gt;(42);
    /// int value = result.Value;
    /// </code>
    /// </example>
    /// </remarks>
    public Result(TValue value)
    {
        this.IsOk = true;
        this.value = value;
        this.error = default;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TValue}"/> class in a failed state.
    /// </summary>
    /// <param name="error">The failure exception.</param>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var result = new Result&lt;int&gt;(new InvalidOperationException("failed"));
    /// Exception error = result.Error;
    /// </code>
    /// </example>
    /// </remarks>
    public Result(Exception error)
    {
        this.IsOk = false;
        this.value = default;
        this.error = error;
    }

    /// <summary>
    /// Gets a value indicating whether the result is successful.
    /// </summary>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var result = Result&lt;string&gt;.Ok("success");
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

#pragma warning disable S2372
            throw new ResultException("No value present");
        }
    }

    /// <summary>
    /// Gets the failure exception.
    /// </summary>
    /// <exception cref="ResultException">Thrown when the result is successful.</exception>
    public Exception Error
    {
        get
        {
            if (!this.IsOk)
                return this.error!;

#pragma warning disable S2372
            throw new ResultException("No error present");
        }
    }

    object IValueResult.Value => this.Value;

    object IResult.Error => this.Error;

    /// <summary>
    /// Converts a result into its value.
    /// </summary>
    /// <param name="result">The source result.</param>
    /// <returns>The value from <paramref name="result"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result&lt;int&gt; result = new(42);
    /// int value = result;
    /// </code>
    /// </example>
    /// </remarks>
    public static implicit operator TValue(Result<TValue> result)
        => result.Value;

    /// <summary>
    /// Converts a failed result into its error.
    /// </summary>
    /// <param name="result">The source result.</param>
    /// <returns>The error from <paramref name="result"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result&lt;int&gt; result = new(new InvalidOperationException("failed"));
    /// Exception error = result;
    /// </code>
    /// </example>
    /// </remarks>
    public static implicit operator Exception(Result<TValue> result)
        => result.Error;

    /// <summary>
    /// Converts a value into a successful result.
    /// </summary>
    /// <param name="value">The value to wrap.</param>
    /// <returns>A successful result containing <paramref name="value"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result&lt;int&gt; result = 42;
    /// </code>
    /// </example>
    /// </remarks>
    public static implicit operator Result<TValue>(TValue value)
        => new(value);

    /// <summary>
    /// Converts an error into a failed result.
    /// </summary>
    /// <param name="error">The error to wrap.</param>
    /// <returns>A failed result containing <paramref name="error"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result&lt;int&gt; result = new InvalidOperationException("failed");
    /// </code>
    /// </example>
    /// </remarks>
    public static implicit operator Result<TValue>(Exception error)
        => new(error);

    /// <summary>
    /// Converts a result into a completed <see cref="Task{TResult}"/>.
    /// </summary>
    /// <param name="result">The result to wrap.</param>
    /// <returns>A completed task containing <paramref name="result"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result&lt;int&gt; result = new(42);
    /// Task&lt;Result&lt;int&gt;&gt; task = result;
    /// </code>
    /// </example>
    /// </remarks>
    public static implicit operator Task<Result<TValue>>(Result<TValue> result)
        => Task.FromResult(result);

    /// <summary>
    /// Converts a result into a completed <see cref="ValueTask{TResult}"/>.
    /// </summary>
    /// <param name="result">The result to wrap.</param>
    /// <returns>A completed value task containing <paramref name="result"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result&lt;int&gt; result = new(42);
    /// ValueTask&lt;Result&lt;int&gt;&gt; task = result;
    /// </code>
    /// </example>
    /// </remarks>
    public static implicit operator ValueTask<Result<TValue>>(Result<TValue> result)
        => new(result);

    /// <summary>
    /// Converts a result with <see cref="Exception"/> errors to a typed error result.
    /// </summary>
    /// <param name="result">The source result.</param>
    /// <returns>A <see cref="Result{TValue, TError}"/> with the same state.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result&lt;int&gt; source = new(42);
    /// Result&lt;int, Exception&gt; converted = source;
    /// </code>
    /// </example>
    /// </remarks>
    public static implicit operator Result<TValue, Exception>(Result<TValue> result)
        => result.IsOk ? new(result.Value, default, true) : new(default, result.Error, false);

    /// <summary>
    /// Converts a typed error result with <see cref="Exception"/> errors to <see cref="Result{TValue}"/>.
    /// </summary>
    /// <param name="result">The source result.</param>
    /// <returns>A <see cref="Result{TValue}"/> with the same state.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result&lt;int, Exception&gt; source = Result&lt;int, Exception&gt;.Ok(42);
    /// Result&lt;int&gt; converted = source;
    /// </code>
    /// </example>
    /// </remarks>
    public static implicit operator Result<TValue>(Result<TValue, Exception> result)
        => result.IsOk ? new(result.Value) : new(result.Error);

    public static bool operator ==(Result<TValue> left, Result<TValue> right)
        => left.Equals(right);

    public static bool operator !=(Result<TValue> left, Result<TValue> right)
        => !left.Equals(right);

    /// <summary>
    /// Returns the hash code for the current instance.
    /// </summary>
    /// <returns>A hash code for this result.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// int hash = new Result&lt;int&gt;(42).GetHashCode();
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
    /// <returns><see langword="true"/> when <paramref name="obj"/> represents the same state and value; otherwise <see langword="false"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// bool equal = new Result&lt;int&gt;(42).Equals((object)new Result&lt;int&gt;(42));
    /// </code>
    /// </example>
    /// </remarks>
    public override bool Equals(object? obj)
    {
        if (obj is null)
            return !this.IsOk;

        if (obj is Result<TValue, Exception> other2)
            return this.Equals(other2);

        if (obj is IValueResult<TValue, Exception> other)
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
    /// IValueResult&lt;int, Exception&gt; other = new Result&lt;int&gt;(42);
    /// bool equal = new Result&lt;int&gt;(42).Equals(other);
    /// </code>
    /// </example>
    /// </remarks>
    public bool Equals(IValueResult<TValue, Exception>? other)
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
    /// bool equal = new Result&lt;int&gt;(42).Equals(new Result&lt;int&gt;(42));
    /// </code>
    /// </example>
    /// </remarks>
    public bool Equals(Result<TValue>? other)
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
    /// <param name="action">The action to execute for the success value.</param>
    /// <returns>The current result.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// new Result&lt;int&gt;(42).Inspect(value => Console.WriteLine(value));
    /// </code>
    /// </example>
    /// </remarks>
    public Result<TValue> Inspect(Action<TValue> action)
    {
        if (this.IsOk)
            action(this.value!);

        return this;
    }

    /// <summary>
    /// Runs an action when the result is failed.
    /// </summary>
    /// <param name="action">The action to execute for the error.</param>
    /// <returns>The current result.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// new Result&lt;int&gt;(new InvalidOperationException("failed"))
    ///     .InspectError(error => Console.WriteLine(error.Message));
    /// </code>
    /// </example>
    /// </remarks>
    public Result<TValue> InspectError(Action<Exception> action)
    {
        if (!this.IsOk)
            action(this.error!);

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
    /// bool matches = new Result&lt;int&gt;(42).IsOkAnd(value => value &gt; 0);
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
    /// bool matches = new Result&lt;int&gt;(new InvalidOperationException("failed"))
    ///     .IsErrorAnd(error => error is InvalidOperationException);
    /// </code>
    /// </example>
    /// </remarks>
    public bool IsErrorAnd(Func<Exception, bool> predicate)
        => this.IsError && predicate(this.error!);

    /// <summary>
    /// Maps the success value to another success value type.
    /// </summary>
    /// <typeparam name="TOther">The mapped value type.</typeparam>
    /// <param name="func">The mapping function for the success value.</param>
    /// <returns>A mapped result.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result&lt;string&gt; mapped = new Result&lt;int&gt;(42).Map(value => value.ToString());
    /// </code>
    /// </example>
    /// </remarks>
    public Result<TOther> Map<TOther>(Func<TValue, TOther> func)
        where TOther : notnull
        => this.IsOk ? new(func(this.value!)) : new(this.error!);

    /// <summary>
    /// Maps both success and error values to another result type.
    /// </summary>
    /// <typeparam name="TOther">The mapped success value type.</typeparam>
    /// <typeparam name="TOtherError">The mapped error type.</typeparam>
    /// <param name="map">The success mapping function.</param>
    /// <param name="mapError">The error mapping function.</param>
    /// <returns>A mapped result.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result&lt;string, string&gt; mapped = new Result&lt;int&gt;(42)
    ///     .Map(value => value.ToString(), error => error.Message);
    /// </code>
    /// </example>
    /// </remarks>
    public Result<TOther, TOtherError> Map<TOther, TOtherError>(Func<TValue, TOther> map, Func<Exception, TOtherError> mapError)
        where TOther : notnull
        where TOtherError : notnull
        => this.IsOk ? new(map(this.value!), default, true) : new(default, mapError(this.error!), false);

    /// <summary>
    /// Returns the current result when successful; otherwise returns a successful result with the specified value.
    /// </summary>
    /// <param name="other">The fallback value.</param>
    /// <returns>The current result when successful; otherwise a result containing <paramref name="other"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result&lt;int&gt; result = new Result&lt;int&gt;(new InvalidOperationException("failed")).Or(42);
    /// </code>
    /// </example>
    /// </remarks>
    public Result<TValue> Or(TValue other)
        => this.IsOk ? this : new(other);

    /// <summary>
    /// Returns the current result when successful; otherwise returns a successful result with a lazily created value.
    /// </summary>
    /// <param name="other">The fallback value factory.</param>
    /// <returns>The current result when successful; otherwise a result containing the fallback value.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result&lt;int&gt; result = new Result&lt;int&gt;(new InvalidOperationException("failed")).Or(() => 42);
    /// </code>
    /// </example>
    /// </remarks>
    public Result<TValue> Or(Func<TValue> other)
        => this.IsOk ? this : new(other());

    /// <summary>
    /// Returns the success value or a fallback value.
    /// </summary>
    /// <param name="defaultValue">The fallback value.</param>
    /// <returns>The success value when available; otherwise <paramref name="defaultValue"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// int value = new Result&lt;int&gt;(new InvalidOperationException("failed")).OrDefault(0);
    /// </code>
    /// </example>
    /// </remarks>
    public TValue OrDefault(TValue defaultValue)
        => this.IsOk ? this.value! : defaultValue;

    /// <summary>
    /// Returns the success value or a lazily created fallback value.
    /// </summary>
    /// <param name="defaultValue">The fallback value factory.</param>
    /// <returns>The success value when available; otherwise the fallback value.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// int value = new Result&lt;int&gt;(new InvalidOperationException("failed")).OrDefault(() => 0);
    /// </code>
    /// </example>
    /// </remarks>
    public TValue OrDefault(Func<TValue> defaultValue)
        => this.IsOk ? this.value! : defaultValue();

    /// <summary>
    /// Returns the current result when failed; otherwise returns a failed result with the specified error.
    /// </summary>
    /// <param name="error">The fallback error.</param>
    /// <returns>The current failed result or a failed result containing <paramref name="error"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result&lt;int&gt; result = new Result&lt;int&gt;(42).OrError(new InvalidOperationException("forced"));
    /// </code>
    /// </example>
    /// </remarks>
    public Result<TValue> OrError(Exception error)
        => this.IsError ? this : new(error);

    /// <summary>
    /// Returns the current result when failed; otherwise returns a failed result with a lazily created error.
    /// </summary>
    /// <param name="error">The fallback error factory.</param>
    /// <returns>The current failed result or a failed result containing the fallback error.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result&lt;int&gt; result = new Result&lt;int&gt;(42).OrError(() => new InvalidOperationException("forced"));
    /// </code>
    /// </example>
    /// </remarks>
    public Result<TValue> OrError(Func<Exception> error)
        => this.IsError ? this : new(error());

    /// <summary>
    /// Returns the error value or a fallback error.
    /// </summary>
    /// <param name="defaultError">The fallback error.</param>
    /// <returns>The current error when failed; otherwise <paramref name="defaultError"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Exception error = new Result&lt;int&gt;(42).OrErrorDefault(new InvalidOperationException("fallback"));
    /// </code>
    /// </example>
    /// </remarks>
    public Exception OrErrorDefault(Exception defaultError)
        => this.IsError ? this.error! : defaultError;

    /// <summary>
    /// Returns the error value or a lazily created fallback error.
    /// </summary>
    /// <param name="defaultError">The fallback error factory.</param>
    /// <returns>The current error when failed; otherwise the fallback error.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Exception error = new Result&lt;int&gt;(42).OrErrorDefault(() => new InvalidOperationException("fallback"));
    /// </code>
    /// </example>
    /// </remarks>
    public Exception OrErrorDefault(Func<Exception> defaultError)
        => this.IsError ? this.error! : defaultError();

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
    /// <param name="value">When this method returns, contains the success value if present; otherwise <see langword="null"/>.</param>
    /// <returns><see langword="true"/> when the result is successful; otherwise <see langword="false"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// bool hasValue = new Result&lt;int&gt;(42).TryGetValue(out int? value);
    /// </code>
    /// </example>
    /// </remarks>
    public bool TryGetValue(out TValue? value)
    {
        value = this.value;
        return this.IsOk;
    }

    /// <summary>
    /// Attempts to retrieve the error.
    /// </summary>
    /// <param name="error">When this method returns, contains the error if present; otherwise <see langword="null"/>.</param>
    /// <returns><see langword="true"/> when the result is failed; otherwise <see langword="false"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// bool hasError = new Result&lt;int&gt;(new InvalidOperationException("failed"))
    ///     .TryGetError(out Exception? error);
    /// </code>
    /// </example>
    /// </remarks>
    public bool TryGetError(out Exception? error)
    {
        error = this.error;
        return this.IsError;
    }
}