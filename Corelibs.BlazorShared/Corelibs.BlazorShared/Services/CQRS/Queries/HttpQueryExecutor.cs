using Common.Basic.Blocks;
using Mediator;

namespace Corelibs.BlazorShared
{
    public sealed class HttpQueryExecutor : IQueryExecutor
    {
        private IMediator _mediator;

        public HttpQueryExecutor(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<TResponse> Execute<TResponse>(IQuery<Result<TResponse>> query, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(query, cancellationToken);
            if (result == null)
                return default;

            var value = result.Get();
            return value;
        }
    }
}
