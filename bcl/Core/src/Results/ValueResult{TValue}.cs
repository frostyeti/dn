// ReSharper disable ParameterHidesMember
namespace FrostYeti.Results;

/// <summary>
/// Represents the outcome of an operation that either succeeds with a value or fails with an <see cref="Exception"/>.
/// </summary>
/// <typeparam name="TValue">The success value type.</typeparam>
/// <remarks>
/// <example>
/// <code lang="csharp">
/// ValueResult&lt;int&gt; ok = new ValueResult&lt;int&gt;(42);
/// ValueResult&lt;int&gt; failed = new ValueResult&lt;int&gt;(new InvalidOperationException("failed"));
/// </code>
/// </example>
/// </remarks>
public readonly struct ValueResult<TValue> : IValueResult<TValue, Exception>,
    IEquatable<ValueResult<TValue>>
    where TValue : notnull
{
    private readonly TValue? value;

    private readonly Exception? error;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueResult{TValue}"/> struct using the default value for <typeparamref name="TValue"/>.
    /// </summary>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var result = new ValueResult&lt;string&gt;();
    /// bool isError = result.IsError;
    /// </code>
    /// </example>
    /// </remarks>
    public ValueResult()
    {
        this.value = default;
        this.IsOk = this.value is not null;
        if (this.IsError)
            this.error = new ResultException("No value present");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueResult{TValue}"/> struct in a successful state.
    /// </summary>
    /// <param name="value">The success value.</param>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var result = new ValueResult&lt;int&gt;(42);
    /// int value = result.Value;
    /// </code>
    /// </example>
    /// </remarks>
    public ValueResult(TValue value)
    {
        this.IsOk = true;
        this.value = value;
        this.error = default;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueResult{TValue}"/> struct in a failed state.
    /// </summary>
    /// <param name="error">The failure exception.</param>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var result = new ValueResult&lt;int&gt;(new InvalidOperationException("failed"));
    /// Exception error = result.Error;
    /// </code>
    /// </example>
    /// </remarks>
    public ValueResult(Exception error)
    {
        this.IsOk = false;
        this.value = default;
        this.error = error;
    }

    /// <summary>
    /// Gets a value indicating whether the result is successful.
    /// </summary>
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
    /// Gets the failure exception.
    /// </summary>
    /// <exception cref="ResultException">Thrown when the result is successful.</exception>
    public Exception Error
    {
        get
        {
            if (!this.IsOk)
                return this.error!;

            throw new ResultException("No error present");
        }
    }

    object IValueResult.Value => this.Value;

    object IResult.Error => this.Error;

    /// <summary>
    /// Converts a value result into its success value.
    /// </summary>
    /// <param name="valueResult">The source value result.</param>
    /// <returns>The success value from <paramref name="valueResult"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// ValueResult&lt;int&gt; result = new(42);
    /// int value = result;
    /// </code>
    /// </example>
    /// </remarks>
    public static implicit operator TValue(ValueResult<TValue> valueResult)
        => valueResult.Value;

    /// <summary>
    /// Converts a failed value result into its error.
    /// </summary>
    /// <param name="valueResult">The source value result.</param>
    /// <returns>The error from <paramref name="valueResult"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// ValueResult&lt;int&gt; result = new(new InvalidOperationException("failed"));
    /// Exception error = result;
    /// </code>
    /// </example>
    /// </remarks>
    public static implicit operator Exception(ValueResult<TValue> valueResult)
        => valueResult.Error;

    /// <summary>
    /// Converts a value into a successful value result.
    /// </summary>
    /// <param name="value">The value to wrap.</param>
    /// <returns>A successful value result containing <paramref name="value"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// ValueResult&lt;int&gt; result = 42;
    /// </code>
    /// </example>
    /// </remarks>
    public static implicit operator ValueResult<TValue>(TValue value)
        => new(value);

    /// <summary>
    /// Converts an exception into a failed value result.
    /// </summary>
    /// <param name="error">The error to wrap.</param>
    /// <returns>A failed value result containing <paramref name="error"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// ValueResult&lt;int&gt; result = new InvalidOperationException("failed");
    /// </code>
    /// </example>
    /// </remarks>
    public static implicit operator ValueResult<TValue>(Exception error)
        => new(error);

    /// <summary>
    /// Converts a value result into a completed <see cref="Task{TResult}"/>.
    /// </summary>
    /// <param name="valueResult">The value result to wrap.</param>
    /// <returns>A completed task containing <paramref name="valueResult"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// ValueResult&lt;int&gt; result = new(42);
    /// Task&lt;ValueResult&lt;int&gt;&gt; task = result;
    /// </code>
    /// </example>
    /// </remarks>
    public static implicit operator Task<ValueResult<TValue>>(ValueResult<TValue> valueResult)
        => Task.FromResult(valueResult);

    /// <summary>
    /// Converts a value result into a completed <see cref="ValueTask{TResult}"/>.
    /// </summary>
    /// <param name="valueResult">The value result to wrap.</param>
    /// <returns>A completed value task containing <paramref name="valueResult"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// ValueResult&lt;int&gt; result = new(42);
    /// ValueTask&lt;ValueResult&lt;int&gt;&gt; task = result;
    /// </code>
    /// </example>
    /// </remarks>
    public static implicit operator ValueTask<ValueResult<TValue>>(ValueResult<TValue> valueResult)
        => new(valueResult);

    /// <summary>
    /// Converts a value result with <see cref="Exception"/> errors to a typed error value result.
    /// </summary>
    /// <param name="valueResult">The source value result.</param>
    /// <returns>A value result with the same state and values.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// ValueResult&lt;int&gt; source = new(42);
    /// ValueResult&lt;int, Exception&gt; converted = source;
    /// </code>
    /// </example>
    /// </remarks>
    public static implicit operator ValueResult<TValue, Exception>(ValueResult<TValue> valueResult)
        => valueResult.IsOk ?
            new(valueResult.Value, default, true) :
            new(default, valueResult.Error, false);

    /// <summary>
    /// Converts a typed error value result with <see cref="Exception"/> errors to <see cref="ValueResult{TValue}"/>.
    /// </summary>
    /// <param name="result">The source value result.</param>
    /// <returns>A value result with the same state.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// ValueResult&lt;int, Exception&gt; source = ValueResult&lt;int, Exception&gt;.Ok(42);
    /// ValueResult&lt;int&gt; converted = source;
    /// </code>
    /// </example>
    /// </remarks>
    public static implicit operator ValueResult<TValue>(ValueResult<TValue, Exception> result)
        => result.IsOk ? new(result.Value) : new(result.Error);

    public static bool operator ==(ValueResult<TValue> left, ValueResult<TValue> right)
        => left.Equals(right);

    public static bool operator !=(ValueResult<TValue> left, ValueResult<TValue> right)
        => !left.Equals(right);

    /// <summary>
    /// Returns the hash code for the current instance.
    /// </summary>
    /// <returns>A hash code for this value result.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// int hash = new ValueResult&lt;int&gt;(42).GetHashCode();
    /// </code>
    /// </example>
    /// </remarks>
    public override int GetHashCode()
        => HashCode.Combine(
            this.IsOk,
            this.value,
            this.error);

    /// <summary>
    /// Determines whether the current value result is equal to the specified object.
    /// </summary>
    /// <param name="obj">The object to compare with the current value result.</param>
    /// <returns><see langword="true"/> when <paramref name="obj"/> represents the same state and values; otherwise <see langword="false"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// bool equal = new ValueResult&lt;int&gt;(42).Equals((object)new ValueResult&lt;int&gt;(42));
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
    /// Determines whether the current value result is equal to another value result abstraction.
    /// </summary>
    /// <param name="other">The value result to compare with the current value result.</param>
    /// <returns><see langword="true"/> when both represent the same state and values; otherwise <see langword="false"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// IValueResult&lt;int, Exception&gt; other = new ValueResult&lt;int&gt;(42);
    /// bool equal = new ValueResult&lt;int&gt;(42).Equals(other);
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
    /// Determines whether the current value result is equal to another value result.
    /// </summary>
    /// <param name="other">The value result to compare with the current value result.</param>
    /// <returns><see langword="true"/> when both represent the same state and values; otherwise <see langword="false"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// bool equal = new ValueResult&lt;int&gt;(42).Equals(new ValueResult&lt;int&gt;(42));
    /// </code>
    /// </example>
    /// </remarks>
    public bool Equals(ValueResult<TValue> other)
    {
        if (this.IsOk != other.IsOk)
            return false;

        if (this.IsOk)
            return this.value!.Equals(other.value);

        return this.error!.Equals(other.error);
    }

    /// <summary>
    /// Runs an action when the value result is successful.
    /// </summary>
    /// <param name="action">The action to execute for the success value.</param>
    /// <returns>The current value result.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// new ValueResult&lt;int&gt;(42).Inspect(value => Console.WriteLine(value));
    /// </code>
    /// </example>
    /// </remarks>
    public ValueResult<TValue> Inspect(Action<TValue> action)
    {
        if (this.IsOk)
            action(this.value!);

        return this;
    }

    /// <summary>
    /// Runs an action when the value result is failed.
    /// </summary>
    /// <param name="action">The action to execute for the error.</param>
    /// <returns>The current value result.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// new ValueResult&lt;int&gt;(new InvalidOperationException("failed"))
    ///     .InspectError(error => Console.WriteLine(error.Message));
    /// </code>
    /// </example>
    /// </remarks>
    public ValueResult<TValue> InspectError(Action<Exception> action)
    {
        if (!this.IsOk)
            action(this.error!);

        return this;
    }

    /// <summary>
    /// Determines whether the value result is successful and its value matches a predicate.
    /// </summary>
    /// <param name="predicate">The predicate to evaluate.</param>
    /// <returns><see langword="true"/> when the value result is successful and the predicate returns <see langword="true"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// bool matches = new ValueResult&lt;int&gt;(42).IsOkAnd(value => value &gt; 0);
    /// </code>
    /// </example>
    /// </remarks>
    public bool IsOkAnd(Func<TValue, bool> predicate)
        => this.IsOk && predicate(this.value!);

    /// <summary>
    /// Determines whether the value result is failed and its error matches a predicate.
    /// </summary>
    /// <param name="predicate">The predicate to evaluate.</param>
    /// <returns><see langword="true"/> when the value result is failed and the predicate returns <see langword="true"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// bool matches = new ValueResult&lt;int&gt;(new InvalidOperationException("failed"))
    ///     .IsErrorAnd(error => error is InvalidOperationException);
    /// </code>
    /// </example>
    /// </remarks>
    public bool IsErrorAnd(Func<Exception, bool> predicate)
        => this.IsError && predicate(this.error!);

    /// <summary>
    /// Maps the success value to another value type.
    /// </summary>
    /// <typeparam name="TOther">The mapped success value type.</typeparam>
    /// <param name="func">The success mapping function.</param>
    /// <returns>A mapped value result.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// ValueResult&lt;string&gt; mapped = new ValueResult&lt;int&gt;(42).Map(value => value.ToString());
    /// </code>
    /// </example>
    /// </remarks>
    public ValueResult<TOther> Map<TOther>(Func<TValue, TOther> func)
        where TOther : notnull
        => this.IsOk ? new(func(this.value!)) : new(this.error!);

    /// <summary>
    /// Maps both success and error values to another value result type.
    /// </summary>
    /// <typeparam name="TOther">The mapped success value type.</typeparam>
    /// <typeparam name="TOtherError">The mapped error type.</typeparam>
    /// <param name="map">The success mapping function.</param>
    /// <param name="mapError">The error mapping function.</param>
    /// <returns>A mapped value result.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// ValueResult&lt;string, string&gt; mapped = new ValueResult&lt;int&gt;(42)
    ///     .Map(value => value.ToString(), error => error.Message);
    /// </code>
    /// </example>
    /// </remarks>
    public ValueResult<TOther, TOtherError> Map<TOther, TOtherError>(
        Func<TValue, TOther> map,
        Func<Exception, TOtherError> mapError)
        where TOther : notnull
        where TOtherError : notnull
        => this.IsOk ?
            new(map(this.value!), default, true) :
            new(default, mapError(this.error!), false);

    /// <summary>
    /// Returns the current value result when successful; otherwise returns a successful result with the specified value.
    /// </summary>
    /// <param name="other">The fallback value.</param>
    /// <returns>The current value result when successful; otherwise a successful fallback result.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// ValueResult&lt;int&gt; result = new ValueResult&lt;int&gt;(new InvalidOperationException("failed")).Or(42);
    /// </code>
    /// </example>
    /// </remarks>
    public ValueResult<TValue> Or(TValue other)
        => this.IsOk ? this : new(other);

    /// <summary>
    /// Returns the current value result when successful; otherwise returns a successful result with a lazily created value.
    /// </summary>
    /// <param name="other">The fallback value factory.</param>
    /// <returns>The current value result when successful; otherwise a successful fallback result.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// ValueResult&lt;int&gt; result = new ValueResult&lt;int&gt;(new InvalidOperationException("failed")).Or(() => 42);
    /// </code>
    /// </example>
    /// </remarks>
    public ValueResult<TValue> Or(Func<TValue> other)
        => this.IsOk ? this : new(other());

    /// <summary>
    /// Returns the success value or a fallback value.
    /// </summary>
    /// <param name="defaultValue">The fallback value.</param>
    /// <returns>The success value when available; otherwise <paramref name="defaultValue"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// int value = new ValueResult&lt;int&gt;(new InvalidOperationException("failed")).OrDefault(0);
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
    /// int value = new ValueResult&lt;int&gt;(new InvalidOperationException("failed")).OrDefault(() => 0);
    /// </code>
    /// </example>
    /// </remarks>
    public TValue OrDefault(Func<TValue> defaultValue)
        => this.IsOk ? this.value! : defaultValue();

    /// <summary>
    /// Returns the error value or a fallback error.
    /// </summary>
    /// <param name="defaultError">The fallback error.</param>
    /// <returns>The current error when failed; otherwise <paramref name="defaultError"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Exception error = new ValueResult&lt;int&gt;(42)
    ///     .OrErrorDefault(new InvalidOperationException("fallback"));
    /// </code>
    /// </example>
    /// </remarks>
    public Exception OrErrorDefault(Exception defaultError)
        => !this.IsOk ? this.error! : defaultError;

    /// <summary>
    /// Returns the current value result when failed; otherwise returns a failed result with the specified error.
    /// </summary>
    /// <param name="error">The fallback error.</param>
    /// <returns>The current failed value result or a failed fallback result.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// ValueResult&lt;int&gt; result = new ValueResult&lt;int&gt;(42)
    ///     .OrError(new InvalidOperationException("forced"));
    /// </code>
    /// </example>
    /// </remarks>
    public ValueResult<TValue> OrError(Exception error)
        => !this.IsOk ? this : new(error);

    /// <summary>
    /// Returns the current value result when failed; otherwise returns a failed result with a lazily created error.
    /// </summary>
    /// <param name="error">The fallback error factory.</param>
    /// <returns>The current failed value result or a failed fallback result.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// ValueResult&lt;int&gt; result = new ValueResult&lt;int&gt;(42)
    ///     .OrError(() => new InvalidOperationException("forced"));
    /// </code>
    /// </example>
    /// </remarks>
    public ValueResult<TValue> OrError(Func<Exception> error)
        => !this.IsOk ? this : new(error());

    /// <summary>
    /// Returns the error value or a lazily created fallback error.
    /// </summary>
    /// <param name="defaultError">The fallback error factory.</param>
    /// <returns>The current error when failed; otherwise the fallback error.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Exception error = new ValueResult&lt;int&gt;(42)
    ///     .OrErrorDefault(() => new InvalidOperationException("fallback"));
    /// </code>
    /// </example>
    /// </remarks>
    public Exception OrErrorDefault(Func<Exception> defaultError)
        => !this.IsOk ? this.error! : defaultError();

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
    /// <returns><see langword="true"/> when the value result is successful; otherwise <see langword="false"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// bool hasValue = new ValueResult&lt;int&gt;(42).TryGetValue(out int? value);
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
    /// <returns><see langword="true"/> when the value result is failed; otherwise <see langword="false"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// bool hasError = new ValueResult&lt;int&gt;(new InvalidOperationException("failed"))
    ///     .TryGetError(out Exception? error);
    /// </code>
    /// </example>
    /// </remarks>
    public bool TryGetError(out Exception? error)
    {
        error = this.error;
        return !this.IsOk;
    }
}