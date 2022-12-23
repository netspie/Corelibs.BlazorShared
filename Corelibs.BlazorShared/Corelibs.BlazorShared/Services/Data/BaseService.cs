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
    }
}
