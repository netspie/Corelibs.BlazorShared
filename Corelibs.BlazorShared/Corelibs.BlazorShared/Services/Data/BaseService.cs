using Common.Basic.Blocks;
using Corelibs.Basic.Collections;
using Corelibs.BlazorShared;
using Mediator;

namespace PageTree.Client.Shared.Services
{
    public abstract class BaseService
    {
        protected readonly IHttpClientFactory _clientFactory;
        protected readonly ISignInRedirector _signInRedirector;

        public BaseService(IHttpClientFactory clientFactory, ISignInRedirector signInRedirector)
        {
            _clientFactory = clientFactory;
            _signInRedirector = signInRedirector;
        }

        protected Task<TResponse> GetResource<TResponse>(IQuery<Result<TResponse>> query, string resourcePath, CancellationToken ct = default)
        {
            var queryString = query.GetQueryString();
            var resourcePathWithQuery = $"{resourcePath}?{queryString}";

            return _clientFactory.GetResource<TResponse>(_signInRedirector, resourcePathWithQuery, ct);
        }

        protected Task<TResponse> GetResource<TApiQuery, TResponse>(TApiQuery apiQuery, string resourcePath, CancellationToken ct = default)
        {
            var queryString = apiQuery.GetQueryString();
            var resourcePathWithQuery = $"{resourcePath}?{queryString}";

            return _clientFactory.GetResource<TResponse>(_signInRedirector, resourcePathWithQuery, ct);
        }

        protected Task<HttpResponseMessage> PostResource(string resourcePath, CancellationToken ct = default)
        {
            return _clientFactory.PostResource(_signInRedirector, resourcePath, ct);
        }
    }
}
