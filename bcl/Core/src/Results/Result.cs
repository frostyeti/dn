namespace FrostYeti.Results;

/// <summary>
/// Represents the outcome of an operation that either succeeds without a value or fails with an <see cref="Exception"/>.
/// </summary>
/// <remarks>
/// <example>
/// <code lang="csharp">
/// Result ok = Result.Ok();
/// Result failed = Result.Fail(new InvalidOperationException("boom"));
/// </code>
/// </example>
/// </remarks>
public class Result : IEmptyResult<Exception>
{
    private readonly Exception? error;

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class in a successful state.
    /// </summary>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var result = new Result();
    /// bool isSuccessful = result.IsOk;
    /// </code>
    /// </example>
    /// </remarks>
    public Result()
    {
        this.IsOk = true;
        this.error = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class in a failed state.
    /// </summary>
    /// <param name="error">The exception that describes the failure.</param>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var result = new Result(new InvalidOperationException("failed"));
    /// Exception error = result.Error;
    /// </code>
    /// </example>
    /// </remarks>
    public Result(Exception error)
    {
        this.error = error;
        this.IsOk = false;
    }

    /// <summary>
    /// Converts a non-generic result into a <see cref="Result{TValue}"/> using <see cref="Never"/> as the value type.
    /// </summary>
    /// <param name="result">The source result.</param>
    /// <returns>A generic result that preserves success or error state.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result source = Result.Ok();
    /// Result&lt;Never&gt; converted = source;
    /// </code>
    /// </example>
    /// </remarks>
    public static implicit operator Result<Never>(Result result)
    {
        if (result.IsError)
            return new Result<Never>(result.Error);

        return new Result<Never>(Never.Value);
    }

    /// <summary>
    /// Converts a <see cref="Result{TValue}"/> with <see cref="Never"/> value type into a non-generic result.
    /// </summary>
    /// <param name="result">The source generic result.</param>
    /// <returns>A non-generic result with the same success or failure state.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result&lt;Never&gt; source = new Result&lt;Never&gt;(Never.Value);
    /// Result converted = source;
    /// </code>
    /// </example>
    /// </remarks>
    public static implicit operator Result(Result<Never> result)
    {
        if (result.IsError)
            return new Result(result.Error);

        return Result.Ok();
    }

    /// <summary>
    /// Converts a result into a completed <see cref="Task{TResult}"/>.
    /// </summary>
    /// <param name="result">The result to wrap.</param>
    /// <returns>A completed task containing <paramref name="result"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result result = Result.Ok();
    /// Task&lt;Result&gt; task = result;
    /// </code>
    /// </example>
    /// </remarks>
    public static implicit operator Task<Result>(Result result)
    {
        return Task.FromResult(result);
    }

    /// <summary>
    /// Converts a result into a completed <see cref="ValueTask{TResult}"/>.
    /// </summary>
    /// <param name="result">The result to wrap.</param>
    /// <returns>A completed value task containing <paramref name="result"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result result = Result.Ok();
    /// ValueTask&lt;Result&gt; task = result;
    /// </code>
    /// </example>
    /// </remarks>
    public static implicit operator ValueTask<Result>(Result result)
    {
        return new ValueTask<Result>(result);
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <returns>A successful <see cref="Result"/> instance.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result result = Result.Ok();
    /// bool isSuccessful = result.IsOk;
    /// </code>
    /// </example>
    /// </remarks>
    public static Result Ok()
    {
        return new Result();
    }

    /// <summary>
    /// Creates a successful generic result.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="value">The successful value.</param>
    /// <returns>A successful <see cref="Result{TValue}"/> containing <paramref name="value"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result&lt;int&gt; result = Result.Ok(42);
    /// int value = result.Value;
    /// </code>
    /// </example>
    /// </remarks>
    public static Result<TValue> Ok<TValue>(TValue value)
        where TValue : notnull
    {
        return new Result<TValue>(value);
    }

    /// <summary>
    /// Creates a successful generic result with a custom error type.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="value">The successful value.</param>
    /// <returns>A successful <see cref="Result{TValue, TError}"/> containing <paramref name="value"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result&lt;int, string&gt; result = Result.Ok&lt;int, string&gt;(42);
    /// bool isSuccessful = result.IsOk;
    /// </code>
    /// </example>
    /// </remarks>
    public static Result<TValue, TError> Ok<TValue, TError>(TValue value)
        where TValue : notnull
        where TError : notnull
    {
        return Result<TValue, TError>.Ok(value);
    }

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="error">The failure exception.</param>
    /// <returns>A failed <see cref="Result"/> containing <paramref name="error"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result result = Result.Fail(new InvalidOperationException("failed"));
    /// bool isFailed = result.IsError;
    /// </code>
    /// </example>
    /// </remarks>
    public static Result Fail(Exception error)
    {
        return new Result(error);
    }

    /// <summary>
    /// Creates a failed generic result.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="error">The failure exception.</param>
    /// <returns>A failed <see cref="Result{TValue}"/> containing <paramref name="error"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result&lt;int&gt; result = Result.Fail&lt;int&gt;(new InvalidOperationException("failed"));
    /// bool isFailed = result.IsError;
    /// </code>
    /// </example>
    /// </remarks>
    public static Result<TValue> Fail<TValue>(Exception error)
        where TValue : notnull
    {
        return new Result<TValue>(error);
    }

    /// <summary>
    /// Creates a failed generic result with a custom error type.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="error">The failure value.</param>
    /// <returns>A failed <see cref="Result{TValue, TError}"/> containing <paramref name="error"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result&lt;int, string&gt; result = Result.Fail&lt;int, string&gt;("failed");
    /// string error = result.Error;
    /// </code>
    /// </example>
    /// </remarks>
    public static Result<TValue, TError> Fail<TValue, TError>(TError error)
        where TValue : notnull
        where TError : notnull
    {
        return Result<TValue, TError>.Fail(error);
    }

    /// <summary>
    /// Executes an action and converts thrown exceptions into a failed result.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>A successful result when the action completes; otherwise a failed result.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result result = Result.TryCatch(() => Console.WriteLine("hello"));
    /// bool isSuccessful = result.IsOk;
    /// </code>
    /// </example>
    /// </remarks>
    public static Result TryCatch(Action action)
    {
        try
        {
            action();
            return new();
        }
        catch (Exception ex)
        {
            return new Result(ex);
        }
    }

    /// <summary>
    /// Executes a function and converts thrown exceptions into a failed result.
    /// </summary>
    /// <typeparam name="TValue">The return value type.</typeparam>
    /// <param name="func">The function to execute.</param>
    /// <returns>A successful result containing the function value or a failed result containing the exception.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result&lt;int&gt; result = Result.TryCatch(() => 42);
    /// int value = result.OrDefault(0);
    /// </code>
    /// </example>
    /// </remarks>
    public static Result<TValue> TryCatch<TValue>(Func<TValue> func)
        where TValue : notnull
    {
        try
        {
            return new Result<TValue>(func());
        }
        catch (Exception ex)
        {
            return new Result<TValue>(ex);
        }
    }

    /// <summary>
    /// Executes a function and maps thrown exceptions into a custom error type.
    /// </summary>
    /// <typeparam name="TValue">The return value type.</typeparam>
    /// <typeparam name="TError">The custom error type.</typeparam>
    /// <param name="func">The function to execute.</param>
    /// <param name="errorFactory">Converts an exception into <typeparamref name="TError"/>.</param>
    /// <returns>A successful result containing the function value or a failed result containing a mapped error.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result&lt;int, string&gt; result = Result.TryCatch&lt;int, string&gt;(
    ///     () => 42,
    ///     ex => ex.Message);
    /// </code>
    /// </example>
    /// </remarks>
    public static Result<TValue, TError> TryCatch<TValue, TError>(
        Func<TValue> func,
        Func<Exception, TError> errorFactory)
        where TValue : notnull
        where TError : notnull
    {
        try
        {
            return Result<TValue, TError>.Ok(func());
        }
        catch (Exception ex)
        {
            return Result<TValue, TError>.Fail(errorFactory(ex));
        }
    }

    /// <summary>
    /// Executes an asynchronous action and converts thrown exceptions into a failed result.
    /// </summary>
    /// <param name="action">The asynchronous action to execute.</param>
    /// <returns>A task that resolves to a successful result when the action completes; otherwise a failed result.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result result = await Result.TryCatchAsync(async () =>
    /// {
    ///     await Task.Delay(10);
    /// });
    /// </code>
    /// </example>
    /// </remarks>
    public static async Task<Result> TryCatchAsync(Func<Task> action)
    {
        try
        {
            await action().ConfigureAwait(false);
            return new Result();
        }
        catch (Exception ex)
        {
            return new Result(ex);
        }
    }

    /// <summary>
    /// Executes an asynchronous function and converts thrown exceptions into a failed result.
    /// </summary>
    /// <typeparam name="TValue">The return value type.</typeparam>
    /// <param name="func">The asynchronous function to execute.</param>
    /// <returns>A task that resolves to a successful result containing the value or a failed result containing the exception.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result&lt;int&gt; result = await Result.TryCatchAsync(async () =>
    /// {
    ///     await Task.Delay(10);
    ///     return 42;
    /// });
    /// </code>
    /// </example>
    /// </remarks>
    public static async Task<Result<TValue>> TryCatchAsync<TValue>(Func<Task<TValue>> func)
        where TValue : notnull
    {
        try
        {
            var result = await func().ConfigureAwait(false);
            return new Result<TValue>(result);
        }
        catch (Exception ex)
        {
            return new Result<TValue>(ex);
        }
    }

    /// <summary>
    /// Executes an asynchronous function and maps thrown exceptions into a custom error type.
    /// </summary>
    /// <typeparam name="TValue">The return value type.</typeparam>
    /// <typeparam name="TError">The custom error type.</typeparam>
    /// <param name="func">The asynchronous function to execute.</param>
    /// <param name="errorFactory">Converts an exception into <typeparamref name="TError"/>.</param>
    /// <returns>A task that resolves to a successful result containing the value or a failed result containing a mapped error.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result&lt;int, string&gt; result = await Result.TryCatchAsync&lt;int, string&gt;(
    ///     async () =>
    ///     {
    ///         await Task.Delay(10);
    ///         return 42;
    ///     },
    ///     ex => ex.Message);
    /// </code>
    /// </example>
    /// </remarks>
    public static async Task<Result<TValue, TError>> TryCatchAsync<TValue, TError>(Func<Task<TValue>> func, Func<Exception, TError> errorFactory)
        where TValue : notnull
        where TError : notnull
    {
        try
        {
            var result = await func().ConfigureAwait(false);
            return Result<TValue, TError>.Ok(result);
        }
        catch (Exception ex)
        {
            return Result<TValue, TError>.Fail(errorFactory(ex));
        }
    }

    /// <summary>
    /// Gets a value indicating whether the result represents success.
    /// </summary>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var result = Result.Ok();
    /// Assert.True(result.IsOk);
    /// </code>
    /// </example>
    /// </remarks>
    public bool IsOk { get; }

    /// <summary>
    /// Gets a value indicating whether the result represents failure.
    /// </summary>
    public bool IsError => !this.IsOk;

    /// <summary>
    /// Gets the exception for a failed result.
    /// </summary>
    /// <exception cref="ResultException">Thrown when the result is successful.</exception>
    public Exception Error
    {
        get
        {
            if (this.IsError)
                return this.error!;

#pragma warning disable S2372 // Exceptions should not be thrown from property getters
            throw new ResultException("No error present");
        }
    }

    object IResult.Error => this.Error;

    /// <summary>
    /// Attempts to retrieve the error value.
    /// </summary>
    /// <param name="error">When this method returns, contains the error if present; otherwise <see langword="null"/>.</param>
    /// <returns><see langword="true"/> when the result is failed; otherwise <see langword="false"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// Result result = Result.Fail(new InvalidOperationException("failed"));
    /// bool hasError = result.TryGetError(out Exception? error);
    /// </code>
    /// </example>
    /// </remarks>
    public bool TryGetError(out Exception? error)
    {
        error = this.error;
        return this.IsError;
    }

    bool IResult.TryGetError(out object? error)
    {
        error = this.error;
        return this.IsError;
    }
}