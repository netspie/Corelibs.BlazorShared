using Common.Basic.Blocks;
using Mediator;

namespace Corelibs.BlazorShared
{
    public sealed class MediatorQueryExecutor : IQueryExecutor
    {
        private IMediator _mediator;

        public MediatorQueryExecutor(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<TResponse> Execute<TResponse>(IQuery<Result<TResponse>> query, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(query, cancellationToken);
            var value = result.Get();

            return value;
        }
    }
}
