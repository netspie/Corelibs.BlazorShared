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

        public async Task<Result<TResponse>> Execute<TResponse>(IQuery<Result<TResponse>> query, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(query, cancellationToken);
            if (result == null)
                return new Result<TResponse>("Result is null. Something went wrong");

            return result;
        }

        public async Task<TResponse> ExecuteForDTO<TResponse>(IQuery<Result<TResponse>> query, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(query, cancellationToken);
            if (result == null)
                return default;

            if (!result.ValidateSuccessAndValues())
                return default;

            return result.Get();
        }
    }
}
