using AutoMapper;
using Common.Basic.Blocks;
using Corelibs.Basic.Architecture.CQRS.Command.Types;
using Mediator;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Corelibs.BlazorShared
{
    public class CommandRequestExecutor : BaseMappingService, ICommandExecutor
    {
        protected readonly Dictionary<Type, Func<object, Task<HttpResponseMessage>>> _requestsPerType =
            new Dictionary<Type, Func<object, Task<HttpResponseMessage>>>();

        protected readonly string _baseRoute;

        public CommandRequestExecutor(string baseRoute, IMapper mapper, IHttpClientFactory clientFactory, ISignInRedirector signInRedirector)
            : base(mapper, clientFactory, signInRedirector)
        {
            _baseRoute = baseRoute;
        }

        async Task<Result> ICommandExecutor.Execute<TCommand>(TCommand command, CancellationToken ct = default)
        {
            var type = typeof(TCommand);
            if (!_requestsPerType.TryGetValue(type, out var request))
                return null;

            var response = await request(command);
            if (response == null || !response.IsSuccessStatusCode)
                return Result.Failure();

            return Result.Success();
        }

        public void Add<TRequest>(Func<TRequest, Task<HttpResponseMessage>> request)
        {
            _requestsPerType.Add(typeof(TRequest), SendRequestLocal);

            Task<HttpResponseMessage> SendRequestLocal(object requestObject)
            {
                var requestObjectTyped = (TRequest) requestObject;
                return request(requestObjectTyped);
            }
        }

        public void AddPost<TAppCommand, TApiCommand>(string resourceRoute)
        {
            var fullResourceRoute = $"{_baseRoute}/{resourceRoute}";
            Add<TAppCommand>(c => PostResource<TAppCommand, TApiCommand>(fullResourceRoute, c));
        }

        public void AddPut<TAppCommand, TApiCommand>(string resourceRoute)
            where TAppCommand : IReplaceCommand
        {
            var fullResourceRoute = $"{_baseRoute}/{resourceRoute}";
            Add<TAppCommand>(c => PutResource<TAppCommand, TApiCommand>($"{fullResourceRoute}/{c.ID}", c));
        }

        public void AddPost<TAppCommand>(string resourceRoute)
        {
            var fullResourceRoute = $"{_baseRoute}/{resourceRoute}";
            Add<TAppCommand>(c => PostResource(fullResourceRoute));
        }
    }
}
