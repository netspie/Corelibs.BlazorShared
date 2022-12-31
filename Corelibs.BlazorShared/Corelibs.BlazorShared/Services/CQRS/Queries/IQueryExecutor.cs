using Common.Basic.Blocks;
using Mediator;

namespace Corelibs.BlazorShared
{
    public interface IQueryExecutor
    {
        Task<TResponse> Execute<TResponse>(IQuery<Result<TResponse>> query, CancellationToken cancellationToken = default);
        Task<TResponse> Execute<TQuery, TResponse>(CancellationToken cancellationToken = default) 
            where TQuery : IQuery<Result<TResponse>>, new() 
            => Execute(new TQuery(), cancellationToken);

        Task<TResponse> Execute<TQuery, TResponse>(string id, CancellationToken cancellationToken = default)
            where TQuery : IQuery<Result<TResponse>>
        {
            var query = (TQuery) Activator.CreateInstance(typeof(TQuery), id);
            return Execute(query, cancellationToken);
        }
    }
}
