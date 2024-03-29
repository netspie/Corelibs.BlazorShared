﻿using AutoMapper;
using Common.Basic.Blocks;
using Corelibs.Basic.Architecture.CQRS.Query.Types;
using Corelibs.Basic.Net;
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

        async Task<TResponse> IQueryExecutor.Execute<TQuery, TResponse>(TQuery query, CancellationToken ct = default)
        {
            var type = typeof(TQuery);
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

        public void Add<TApiQuery, TAppQuery, TAppQueryOut>(Func<TAppQuery, string> getResourceRoute)
            where TAppQuery : IQuery<Result<TAppQueryOut>>
        {
            Add<TAppQuery, TAppQueryOut>(q =>
            {
                var resourceRoute = getResourceRoute(q);
                return GetResource<TAppQueryOut, TApiQuery>(q, $"{_baseRoute}/{resourceRoute}", typeof(FromRouteAttribute));
            });
        }
    }
}
