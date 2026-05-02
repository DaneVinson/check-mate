namespace CM.Domain.Cqrs;

/// <summary>
/// Non-generic base interface for query handlers, enabling dispatch without reflection.
/// </summary>
public interface IQueryHandler
{
    /// <summary>
    /// Handles the specified query asynchronously.
    /// </summary>
    /// <param name="query">The query to handle.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, containing the query result.</returns>
    Task<IResult> HandleAsync(object query, CancellationToken cancellationToken = default);
}

/// <summary>
/// Handles a query of type <typeparamref name="TQuery"/> and returns a result of type <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="TQuery">The type of query to handle.</typeparam>
/// <typeparam name="TResult">The type of result returned by the query.</typeparam>
public interface IQueryHandler<TQuery, TResult> : IQueryHandler
    where TQuery : IQuery<TResult>
{
    /// <summary>
    /// Handles the specified query asynchronously.
    /// </summary>
    /// <param name="query">The query to handle.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, containing the query result.</returns>
    Task<Result<TResult>> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
