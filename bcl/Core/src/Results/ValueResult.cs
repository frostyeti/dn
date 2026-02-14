using System.Security.Cryptography.X509Certificates;

namespace FrostYeti.Results;

public readonly struct ValueResult : IEmptyResult<System.Exception>,
    IEquatable<ValueResult>
{
    private readonly System.Exception? error;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueResult"/> struct with a successful result.
    /// </summary>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var result = new ValueResult();
    /// Assert.True(result.IsOk);
    /// </code>
    /// </example>
    /// </remarks>
    public ValueResult()
    {
        this.IsOk = true;
        this.error = default;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueResult"/> struct with an error.
    /// </summary>
    /// <param name="error">The exception representing the error.</param>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var error = new InvalidOperationException("Something went wrong");
    /// var result = new ValueResult(error);
    /// Assert.True(result.IsError);
    /// </code>
    /// </example>
    /// </remarks>
    public ValueResult(System.Exception error)
    {
        this.IsOk = false;
        this.error = error;
    }

#pragma warning disable SA1129
    public static ValueResult Default { get; } = new ValueResult();

    public bool IsOk { get; }

    public bool IsError => !this.IsOk;

    public System.Exception Error
    {
        get
        {
            if (!this.IsOk)
                return this.error!;

#pragma warning disable S2372
            throw new ResultException("No error present");
        }
    }

    object IResult.Error => this.Error;

    /// <summary>
    /// Implicitly converts a <see cref="ValueResult"/> to an <see cref="Exception"/> by extracting the error.
    /// </summary>
    /// <param name="valueResult">The result to convert.</param>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var result = ValueResult.FailRef(new InvalidOperationException("error"));
    /// Exception ex = result;
    /// Assert.IsType&lt;InvalidOperationException&gt;(ex);
    /// </code>
    /// </example>
    /// </remarks>
    public static implicit operator Exception(ValueResult valueResult)
        => valueResult.Error;

    /// <summary>
    /// Implicitly converts an <see cref="Exception"/> to a <see cref="ValueResult"/> with an error state.
    /// </summary>
    /// <param name="error">The exception to convert.</param>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// ValueResult result = new InvalidOperationException("error");
    /// Assert.True(result.IsError);
    /// </code>
    /// </example>
    /// </remarks>
    public static implicit operator ValueResult(System.Exception error)
        => new(error);

    /// <summary>
    /// Implicitly converts a <see cref="ValueResult"/> to a <see cref="Task{ValueResult}"/>.
    /// </summary>
    /// <param name="valueResult">The result to convert.</param>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// ValueResult result = ValueResult.Default;
    /// Task&lt;ValueResult&gt; task = result;
    /// Assert.True(task.IsCompletedSuccessfully);
    /// </code>
    /// </example>
    /// </remarks>
    public static implicit operator Task<ValueResult>(ValueResult valueResult)
        => Task.FromResult(valueResult);

    /// <summary>
    /// Implicitly converts a <see cref="ValueResult"/> to a <see cref="ValueTask{ValueResult}"/>.
    /// </summary>
    /// <param name="valueResult">The result to convert.</param>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// ValueResult result = ValueResult.Default;
    /// ValueTask&lt;ValueResult&gt; valueTask = result;
    /// Assert.True(valueTask.IsCompletedSuccessfully);
    /// </code>
    /// </example>
    /// </remarks>
    public static implicit operator ValueTask<ValueResult>(ValueResult valueResult)
        => new(valueResult);

    /// <summary>
    /// Implicitly converts a <see cref="ValueResult"/> to a <see cref="ValueResult{Never, Exception}"/>.
    /// </summary>
    /// <param name="valueResult">The result to convert.</param>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// ValueResult result = ValueResult.Default;
    /// ValueResult&lt;Never, Exception&gt; converted = result;
    /// Assert.True(converted.IsOk);
    /// </code>
    /// </example>
    /// </remarks>
    public static implicit operator ValueResult<Never, Exception>(ValueResult valueResult)
        => valueResult.IsOk ? new(Never.Value, default, true) : new(default, valueResult.Error, false);

    public static bool operator ==(ValueResult left, ValueResult right)
        => left.Equals(right);

    public static bool operator !=(ValueResult left, ValueResult right)
        => !left.Equals(right);

    /// <summary>
    /// Returns a successful <see cref="ValueResult"/> with no value.
    /// </summary>
    /// <returns>A <see cref="ValueResult"/> in an ok state.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var result = ValueResult.OkRef();
    /// Assert.True(result.IsOk);
    /// </code>
    /// </example>
    /// </remarks>
    public static ValueResult OkRef()
        => Default;

    /// <summary>
    /// Creates a successful <see cref="ValueResult{TValue}"/> with the specified value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value to wrap.</param>
    /// <returns>A <see cref="ValueResult{TValue}"/> in an ok state containing the value.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var result = ValueResult.OkRef(42);
    /// Assert.True(result.IsOk);
    /// Assert.Equal(42, result.Value);
    /// </code>
    /// </example>
    /// </remarks>
    public static ValueResult<TValue> OkRef<TValue>(TValue value)
        where TValue : notnull
        => new(value);

    /// <summary>
    /// Creates a successful <see cref="ValueResult{TValue, TError}"/> with the specified value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="value">The value to wrap.</param>
    /// <returns>A <see cref="ValueResult{TValue, TError}"/> in an ok state containing the value.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var result = ValueResult.OkRef&lt;int, string&gt;(42);
    /// Assert.True(result.IsOk);
    /// Assert.Equal(42, result.Value);
    /// </code>
    /// </example>
    /// </remarks>
    public static ValueResult<TValue, TError> OkRef<TValue, TError>(TValue value)
        where TValue : notnull
        where TError : notnull
        => new(value, default, true);

    /// <summary>
    /// Creates a failed <see cref="ValueResult"/> with the specified error.
    /// </summary>
    /// <param name="error">The exception representing the error.</param>
    /// <returns>A <see cref="ValueResult"/> in an error state.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var result = ValueResult.FailRef(new InvalidOperationException("failed"));
    /// Assert.True(result.IsError);
    /// </code>
    /// </example>
    /// </remarks>
    public static ValueResult FailRef(System.Exception error)
        => new(error);

    /// <summary>
    /// Creates a failed <see cref="ValueResult{TValue}"/> with the specified error.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="error">The exception representing the error.</param>
    /// <returns>A <see cref="ValueResult{TValue}"/> in an error state.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var result = ValueResult.FailRef&lt;int&gt;(new InvalidOperationException("failed"));
    /// Assert.True(result.IsError);
    /// </code>
    /// </example>
    /// </remarks>
    public static ValueResult<TValue> FailRef<TValue>(System.Exception error)
        where TValue : notnull
        => new(error);

    /// <summary>
    /// Creates a failed <see cref="ValueResult{TValue, TError}"/> with the specified error.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="error">The error value.</param>
    /// <returns>A <see cref="ValueResult{TValue, TError}"/> in an error state.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var result = ValueResult.FailRef&lt;int, string&gt;("error message");
    /// Assert.True(result.IsError);
    /// Assert.Equal("error message", result.Error);
    /// </code>
    /// </example>
    /// </remarks>
    public static ValueResult<TValue, TError> FailRef<TValue, TError>(TError error)
        where TValue : notnull
        where TError : notnull
        => new(default, error, false);

    /// <summary>
    /// Executes an action and catches any exceptions, returning a <see cref="ValueResult"/>.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>A <see cref="ValueResult"/> that is ok if no exception was thrown, or contains the error otherwise.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var okResult = ValueResult.TryCatchRef(() => { });
    /// Assert.True(okResult.IsOk);
    ///
    /// var errorResult = ValueResult.TryCatchRef(() => throw new InvalidOperationException("error"));
    /// Assert.True(errorResult.IsError);
    /// </code>
    /// </example>
    /// </remarks>
    public static ValueResult TryCatchRef(Action action)
    {
        try
        {
            action();
            return ValueResult.Default;
        }
        catch (System.Exception ex)
        {
            return new ValueResult(ex);
        }
    }

    /// <summary>
    /// Executes a function and catches any exceptions, returning a <see cref="ValueResult{TValue}"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the return value.</typeparam>
    /// <param name="func">The function to execute.</param>
    /// <returns>A <see cref="ValueResult{TValue}"/> containing the value or the error.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var okResult = ValueResult.TryCatchRef(() => 42);
    /// Assert.True(okResult.IsOk);
    /// Assert.Equal(42, okResult.Value);
    ///
    /// var errorResult = ValueResult.TryCatchRef&lt;int&gt;(() => throw new InvalidOperationException("error"));
    /// Assert.True(errorResult.IsError);
    /// </code>
    /// </example>
    /// </remarks>
    public static ValueResult<TValue> TryCatchRef<TValue>(Func<TValue> func)
        where TValue : notnull
    {
        try
        {
            return new ValueResult<TValue>(func());
        }
        catch (System.Exception ex)
        {
            return new ValueResult<TValue>(ex);
        }
    }

    /// <summary>
    /// Executes a function and catches any exceptions, converting them to a custom error type.
    /// </summary>
    /// <typeparam name="TValue">The type of the return value.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="func">The function to execute.</param>
    /// <param name="errorFactory">A function that converts an exception to the error type.</param>
    /// <returns>A <see cref="ValueResult{TValue, TError}"/> containing the value or the converted error.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var okResult = ValueResult.TryCatchRef(() => 42, ex => ex.Message);
    /// Assert.True(okResult.IsOk);
    ///
    /// var errorResult = ValueResult.TryCatchRef&lt;int, string&gt;(
    ///     () => throw new InvalidOperationException("failed"),
    ///     ex => ex.Message);
    /// Assert.True(errorResult.IsError);
    /// Assert.Equal("failed", errorResult.Error);
    /// </code>
    /// </example>
    /// </remarks>
    public static ValueResult<TValue, TError> TryCatchRef<TValue, TError>(Func<TValue> func, Func<System.Exception, TError> errorFactory)
        where TValue : notnull
        where TError : notnull
    {
        try
        {
            return new ValueResult<TValue, TError>(func(), default, true);
        }
        catch (System.Exception ex)
        {
            return new ValueResult<TValue, TError>(default, errorFactory(ex), false);
        }
    }

    /// <summary>
    /// Executes an async action and catches any exceptions, returning a <see cref="ValueResult"/>.
    /// </summary>
    /// <param name="action">The async action to execute.</param>
    /// <returns>A <see cref="ValueTask{ValueResult}"/> that is ok if no exception was thrown.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var okResult = await ValueResult.TryCatchRefAsync(async () => await Task.CompletedTask);
    /// Assert.True(okResult.IsOk);
    ///
    /// var errorResult = await ValueResult.TryCatchRefAsync(async () => await Task.FromException(new InvalidOperationException("error")));
    /// Assert.True(errorResult.IsError);
    /// </code>
    /// </example>
    /// </remarks>
    public static async ValueTask<ValueResult> TryCatchRefAsync(Func<Task> action)
    {
        try
        {
            await action();
            return Default;
        }
        catch (System.Exception ex)
        {
            return new ValueResult(ex);
        }
    }

    /// <summary>
    /// Executes an async function and catches any exceptions, returning a <see cref="ValueResult{TValue}"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the return value.</typeparam>
    /// <param name="func">The async function to execute.</param>
    /// <returns>A <see cref="ValueTask{T}"/> containing the value or the error.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var okResult = await ValueResult.TryCatchRefAsync(() => Task.FromResult(42));
    /// Assert.True(okResult.IsOk);
    /// Assert.Equal(42, okResult.Value);
    ///
    /// var errorResult = await ValueResult.TryCatchRefAsync&lt;int&gt;(() => Task.FromException&lt;int&gt;(new InvalidOperationException("error")));
    /// Assert.True(errorResult.IsError);
    /// </code>
    /// </example>
    /// </remarks>
    public static async ValueTask<ValueResult<TValue>> TryCatchRefAsync<TValue>(Func<Task<TValue>> func)
        where TValue : notnull
    {
        try
        {
            return new ValueResult<TValue>(await func());
        }
        catch (System.Exception ex)
        {
            return new ValueResult<TValue>(ex);
        }
    }

    public static async ValueTask<ValueResult<TValue, TError>> TryCatchRefAsync<TValue, TError>(
        Func<Task<TValue>> func,
        Func<System.Exception, TError> errorFactory)
        where TValue : notnull
        where TError : notnull
    {
        try
        {
            return new ValueResult<TValue, TError>(await func(), default, true);
        }
        catch (System.Exception ex)
        {
            return new ValueResult<TValue, TError>(default, errorFactory(ex), false);
        }
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code suitable for use in hashing algorithms and data structures.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var result = ValueResult.Default;
    /// var hash = result.GetHashCode();
    /// Assert.NotEqual(0, hash);
    /// </code>
    /// </example>
    /// </remarks>
    public override int GetHashCode()
    {
        if (this.IsOk)
            return HashCode.Combine(this.IsOk);
        return this.error!.GetHashCode() ^ HashCode.Combine(this.IsError);
    }

    /// <summary>
    /// Determines whether the specified object is equal to this instance.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns><c>true</c> if the objects are equal; otherwise, <c>false</c>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var result1 = ValueResult.Default;
    /// var result2 = ValueResult.Default;
    /// Assert.True(result1.Equals((object)result2));
    /// </code>
    /// </example>
    /// </remarks>
    public override bool Equals(object? obj)
    {
        if (obj is null)
            return !this.IsOk;

        if (obj is Result other2)
            return this.Equals(other2);

        if (obj is IEmptyResult<Exception> other1)
            return this.Equals(other1);

        if (obj is IResult<Exception> other)
            return this.Equals(other);

        return false;
    }

    /// <summary>
    /// Determines whether the specified <see cref="IEmptyResult{Exception}"/> is equal to this instance.
    /// </summary>
    /// <param name="other">The result to compare.</param>
    /// <returns><c>true</c> if the results are equal; otherwise, <c>false</c>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var result = ValueResult.Default;
    /// IEmptyResult&lt;Exception&gt; other = ValueResult.Default;
    /// Assert.True(result.Equals(other));
    /// </code>
    /// </example>
    /// </remarks>
    public bool Equals(IEmptyResult<Exception>? other)
    {
        if (other is null)
            return !this.IsOk;

        if (!this.IsOk)
            return !other.IsOk;

        return true;
    }

    /// <summary>
    /// Determines whether the specified <see cref="ValueResult"/> is equal to this instance.
    /// </summary>
    /// <param name="other">The result to compare.</param>
    /// <returns><c>true</c> if the results are equal; otherwise, <c>false</c>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var result1 = ValueResult.Default;
    /// var result2 = ValueResult.Default;
    /// Assert.True(result1.Equals(result2));
    /// </code>
    /// </example>
    /// </remarks>
    public bool Equals(ValueResult other)
    {
        if (this.IsOk == other.IsOk)
            return true;

        if (this.IsError && other.IsError && this.error == other.error)
            return true;

        return false;
    }

    /// <summary>
    /// Executes an action on the error if this result is in an error state.
    /// </summary>
    /// <param name="action">The action to execute on the error.</param>
    /// <returns>This result for method chaining.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Exception? capturedError = null;
    /// var result = ValueResult.FailRef(new InvalidOperationException("error"));
    /// result.InspectError(ex => capturedError = ex);
    /// Assert.NotNull(capturedError);
    /// </code>
    /// </example>
    /// </remarks>
    public ValueResult InspectError(Action<Exception> action)
    {
        if (!this.IsOk)
            action(this.error!);

        return this;
    }

    bool IResult.TryGetError(out object? error)
    {
        var res = this.TryGetError(out var e);
        error = e;
        return res;
    }

    /// <summary>
    /// Attempts to get the error from this result.
    /// </summary>
    /// <param name="error">The error if this result is in an error state; otherwise, <c>null</c>.</param>
    /// <returns><c>true</c> if this result is in an error state; otherwise, <c>false</c>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var okResult = ValueResult.Default;
    /// Assert.False(okResult.TryGetError(out var noError));
    ///
    /// var errorResult = ValueResult.FailRef(new InvalidOperationException("error"));
    /// Assert.True(errorResult.TryGetError(out var error));
    /// Assert.NotNull(error);
    /// </code>
    /// </example>
    /// </remarks>
    public bool TryGetError(out System.Exception? error)
    {
        error = this.error;
        return !this.IsOk;
    }
}