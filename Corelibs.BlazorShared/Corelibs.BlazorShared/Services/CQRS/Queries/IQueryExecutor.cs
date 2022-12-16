using Common.Basic.Blocks;
using Mediator;

namespace Corelibs.BlazorShared
{
    public interface IQueryExecutor
    {
        Task<TResponse> Execute<TResponse>(IQuery<Result<TResponse>> query, CancellationToken cancellationToken = default);
    }
}
