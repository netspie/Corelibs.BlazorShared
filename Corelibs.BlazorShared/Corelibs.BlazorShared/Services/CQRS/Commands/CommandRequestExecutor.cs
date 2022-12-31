using AutoMapper;
using Common.Basic.Blocks;
using Mediator;

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

        async Task<Result> ICommandExecutor.Execute(ICommand<Result> command, CancellationToken ct = default)
        {
            var type = command.GetType();
            if (!_requestsPerType.TryGetValue(type, out var request))
                return null;

            var response = await request(request);
            if (response == null || !response.IsSuccessStatusCode)
                return Result.Failure();

            return Result.Success();
        }

        public void Add<TRequest>(Func<TRequest, Task<HttpResponseMessage>> request) =>
            _requestsPerType.Add(typeof(TRequest), requestObject => request((TRequest)requestObject));

        public void AddPost<TAppCommand, TApiCommand>(string resourceRoute)
        {
            var fullResourceRoute = $"{_baseRoute}/{resourceRoute}";
            Add<TAppCommand>(c => PostResource<TAppCommand, TApiCommand>(fullResourceRoute, c));
        }

        public void AddPut<TAppCommand, TApiCommand>(string resourceRoute)
        {
            var fullResourceRoute = $"{_baseRoute}/{resourceRoute}";
            Add<TAppCommand>(c => PutResource<TAppCommand, TApiCommand>(fullResourceRoute, c));
        }

        public void AddPost<TAppCommand>(string resourceRoute)
        {
            var fullResourceRoute = $"{_baseRoute}/{resourceRoute}";
            Add<TAppCommand>(c => PostResource(fullResourceRoute));
        }
    }
}
