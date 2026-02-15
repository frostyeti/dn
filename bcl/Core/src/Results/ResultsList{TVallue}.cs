namespace FrostYeti.Results;

/// <summary>
/// Represents the ResultsList class.
/// </summary>
/// <typeparam name="TValue">The type of the values in the results list.</typeparam>
/// <remarks>
/// <example>
/// <code lang="csharp">
/// var instance = default(object);
/// </code>
/// </example>
/// </remarks>
public class ResultsList<TValue>
    where TValue : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResultsList{TValue}"/> class with an empty list.
    /// </summary>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var list = new ResultsList&lt;int&gt;();
    /// Assert.Empty(list.Results);
    /// </code>
    /// </example>
    /// </remarks>
    public ResultsList()
    {
        this.Results = new List<Result<TValue>>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResultsList{TValue}"/> class with the specified capacity.
    /// </summary>
    /// <param name="capacity">The initial capacity of the internal list.</param>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var list = new ResultsList&lt;int&gt;(10);
    /// Assert.Empty(list.Results);
    /// </code>
    /// </example>
    /// </remarks>
    public ResultsList(int capacity)
    {
        this.Results = new List<Result<TValue>>(capacity);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResultsList{TValue}"/> class from an enumerable of results.
    /// </summary>
    /// <param name="results">The results to initialize the list with.</param>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var results = new[] { Result.Ok(1), Result.Ok(2) };
    /// var list = new ResultsList&lt;int&gt;(results);
    /// Assert.Equal(2, list.Results.Count);
    /// </code>
    /// </example>
    /// </remarks>
    public ResultsList(IEnumerable<Result<TValue>> results)
    {
        this.Results = results.ToList();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResultsList{TValue}"/> class from a list of results.
    /// </summary>
    /// <param name="results">The results to initialize the list with.</param>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var results = new List&lt;Result&lt;int&gt;&gt; { Result.Ok(1), Result.Ok(2) };
    /// var list = new ResultsList&lt;int&gt;(results);
    /// Assert.Equal(2, list.Results.Count);
    /// </code>
    /// </example>
    /// </remarks>
    public ResultsList(IList<Result<TValue>> results)
    {
        this.Results = results ?? throw new ArgumentNullException(nameof(results));
    }

    public IList<Result<TValue>> Results { get; }

    public bool IsError => this.Results.Any(r => r.IsError);

    public bool IsOk => !this.IsError;

    /// <summary>
    /// Extracts all successful values from the results list.
    /// </summary>
    /// <param name="throwOnError">If <c>true</c>, throws an aggregate exception when any result is an error.</param>
    /// <returns>A list of values from successful results.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var results = new[] { Result.Ok(1), Result.Ok(2), Result.Fail&lt;int&gt;(new Exception("error")) };
    /// var list = new ResultsList&lt;int&gt;(results);
    /// var values = list.ToValues(throwOnError: false);
    /// Assert.Equal(2, values.Count);
    /// </code>
    /// </example>
    /// </remarks>
    public List<TValue> ToValues(bool throwOnError = true)
    {
        if (this.IsError && throwOnError)
            throw this.ToAggregateException();

        return this.Results
            .Where(r => r.IsOk)
            .Select(r => r.Value)
            .ToList();
    }

    /// <summary>
    /// Creates an aggregate exception from all errors in the results list.
    /// </summary>
    /// <returns>An <see cref="AggregateException"/> containing all errors from failed results.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var results = new[] { Result.Fail&lt;int&gt;(new InvalidOperationException("error1")), Result.Fail&lt;int&gt;(new ArgumentException("error2")) };
    /// var list = new ResultsList&lt;int&gt;(results);
    /// var aggEx = list.ToAggregateException();
    /// Assert.Equal(2, aggEx.InnerExceptions.Count);
    /// </code>
    /// </example>
    /// </remarks>
    public AggregateException ToAggregateException()
    {
        var errors = this.Results
            .Where(r => r.IsError)
            .Select(r => r.Error)
            .ToList();

        if (errors.Count == 0)
            return new AggregateException("No errors present in results list.");

        return new AggregateException(errors);
    }
}