using AutoMapper;
using Common.Basic.Blocks;
using Common.Basic.CQRS.Query;
using Mediator;

namespace Corelibs.BlazorShared
{

    public class QueryRequestExecutor : BaseMappingService, IQueryExecutor
    {
        private readonly Dictionary<Type, Func<object, Task<object>>> _requestsPerType =
            new Dictionary<Type, Func<object, Task<object>>>();
        
        protected const string _baseRoute = "/api/v1";

        public QueryRequestExecutor(IMapper mapper, IHttpClientFactory clientFactory, ISignInRedirector signInRedirector)
            : base(mapper, clientFactory, signInRedirector)
        {}

        async Task<TResponse> IQueryExecutor.Execute<TResponse>(IQuery<Result<TResponse>> query, CancellationToken ct = default)
        {
            var type = query.GetType();
            if (!_requestsPerType.TryGetValue(type, out var request))
                return default;

            var response = await request(query);
            return (TResponse) response;
        }

        public void Add<TRequest, TResponse>(Func<TRequest, Task<TResponse>> request) =>
            _requestsPerType.Add(typeof(TRequest), async requestObject => await request((TRequest) requestObject));

        public void Add<TApiQuery, TAppQuery, TAppQueryOut>(string resourceRoute)
            where TApiQuery : IApiQuery
            where TAppQuery : IQuery<Result<TAppQueryOut>>, IGetQuery
        {
            var fullResourceRoute = $"{_baseRoute}/{resourceRoute}";
            Add<TAppQuery, TAppQueryOut>(q => GetResource<TAppQueryOut, TApiQuery>(q, $"{fullResourceRoute}/{q.ID}", typeof(FromRouteAttribute)));
        }
    }
}
