namespace CatEats.UserService.Application.Queries.Handlers;

public interface IQueryHandler<in TQuery, TResult>
    where TQuery : IQuery<TResult>
    where TResult : class?
{
    Task<TResult> Query(TQuery query, CancellationToken cancellationToken);
}