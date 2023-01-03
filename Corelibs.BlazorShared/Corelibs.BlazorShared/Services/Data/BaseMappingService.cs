using AutoMapper;
using Common.Basic.Blocks;
using Mediator;

namespace Corelibs.BlazorShared
{
    public abstract class BaseMappingService : BaseService
    {
        protected readonly IMapper _mapper;

        public BaseMappingService(
            IMapper mapper, IHttpClientFactory clientFactory, ISignInRedirector signInRedirector) 
            : base(clientFactory, signInRedirector)
        {
            _mapper = mapper;
        }

        protected Task<TResponse> GetResource<TResponse, TApiQuery>(IQuery<Result<TResponse>> query, string resourcePath, CancellationToken ct = default)
        {
            var apiQuery = _mapper.Map<TApiQuery>(query);
            return GetResource<TApiQuery, TResponse>(apiQuery, resourcePath, ct);
        }

        protected Task<TResponse> GetResource<TResponse, TApiQuery>(IQuery<Result<TResponse>> query, string resourcePath, Type routeAttrType, CancellationToken ct = default)
        {
            var apiQuery = _mapper.Map<TApiQuery>(query);
            return GetResource<TApiQuery, TResponse>(apiQuery, resourcePath, routeAttrType, ct);
        }

        protected Task<HttpResponseMessage> PostResource<TBody, TApiCommand>(string resourcePath, TBody body, CancellationToken ct = default)
        {
            var apiCommand = _mapper.Map<TApiCommand>(body);
            return PostResource(resourcePath, apiCommand, ct);
        }

        protected Task<HttpResponseMessage> PutResource<TBody, TApiCommand>(string resourcePath, TBody body, CancellationToken ct = default)
        {
            var apiCommand = _mapper.Map<TApiCommand>(body);
            return PutResource(resourcePath, apiCommand, ct);
        }

        protected Task<HttpResponseMessage> PatchResource<TBody, TApiCommand>(string resourcePath, TBody body, CancellationToken ct = default)
        {
            var apiCommand = _mapper.Map<TApiCommand>(body);
            return PatchResource(resourcePath, apiCommand, ct);
        }
    }
}
