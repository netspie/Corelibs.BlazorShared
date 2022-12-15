using Common.Basic.Blocks;
using Mediator;

namespace Corelibs.BlazorShared
{
    public interface IQueryExecutor
    {
        Task<Result<TResponse>> Execute<TResponse>(IQuery<Result<TResponse>> query, CancellationToken cancellationToken = default);
        Task<TResponse> ExecuteForDTO<TResponse>(IQuery<Result<TResponse>> query, CancellationToken cancellationToken = default);
    }
}
