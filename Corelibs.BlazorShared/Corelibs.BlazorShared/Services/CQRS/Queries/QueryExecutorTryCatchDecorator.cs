using Common.Basic.Blocks;
using Mediator;

namespace Corelibs.BlazorShared
{
    public sealed class QueryExecutorTryCatchDecorator<TException> : IQueryExecutor
        where TException : Exception
    {
        private readonly IQueryExecutor _decorated;
        private readonly Action<TException> _onCatch;

        public QueryExecutorTryCatchDecorator(IQueryExecutor decorated, Action<TException> onCatch)
        {
            _decorated = decorated;
            _onCatch = onCatch;
        }

        public async Task<TResponse> Execute<TResponse>(IQuery<Result<TResponse>> query, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _decorated.Execute(query, cancellationToken);
            }
            catch (TException exception)
            {
                _onCatch.Invoke(exception);
                return default;
            }
        }
    }
}
