using Common.Basic.Blocks;
using Mediator;

namespace Corelibs.BlazorShared
{
    public interface ICommandExecutor
    {
        Task<Result> Execute<TCommand>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : ICommand<Result>;

        Task<Result> Execute<TCommand>(CancellationToken cancellationToken = default)
            where TCommand : ICommand<Result>, new()
            => Execute(new TCommand(), cancellationToken);

        Task<Result> Execute<TCommand>(string id, CancellationToken cancellationToken = default)
            where TCommand : ICommand<Result>
        {
            var command = (TCommand) Activator.CreateInstance(typeof(TCommand), id);
            return Execute(command, cancellationToken);
        }
    }
}
