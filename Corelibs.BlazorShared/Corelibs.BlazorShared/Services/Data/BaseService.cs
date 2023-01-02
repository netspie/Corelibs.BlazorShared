using Common.Basic.Blocks;
using Common.Basic.Collections;
using Corelibs.Basic.Collections;
using Corelibs.Basic.Net;
using Corelibs.Basic.Reflection;
using Mediator;
using System.Reflection;

namespace Corelibs.BlazorShared
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
            var resourcePathWithQuery = queryString.IsNullOrEmpty() ? resourcePath : $"{resourcePath}?{queryString}";
            resourcePathWithQuery = resourcePathWithQuery.ReplaceParametersWithValues(apiQuery);

            return _clientFactory.GetResource<TResponse>(_signInRedirector, resourcePathWithQuery, ct);
        }

        protected Task<TResponse> GetResource<TApiQuery, TResponse>(TApiQuery apiQuery, string resourcePath, Type routeAttrType, CancellationToken ct = default)
        {
            var queryString = apiQuery.GetQueryString(routeAttrType);
            var resourcePathWithQuery = queryString.IsNullOrEmpty() ? resourcePath : $"{resourcePath}?{queryString}";
            resourcePathWithQuery = resourcePathWithQuery.ReplaceParametersWithValues(apiQuery);

            return _clientFactory.GetResource<TResponse>(_signInRedirector, resourcePathWithQuery, ct);
        }

        protected Task<HttpResponseMessage> PostResource(string resourcePath, CancellationToken ct = default)
        {
            return _clientFactory.PostResource(_signInRedirector, resourcePath, ct);
        }

        protected Task<HttpResponseMessage> PostResource<TBody>(string resourcePath, TBody body, CancellationToken ct = default)
        {
            resourcePath = resourcePath.ReplaceParametersWithValues(body);
            return _clientFactory.PostResource(_signInRedirector, resourcePath, body, ct);
        }

        protected Task<HttpResponseMessage> PutResource<TBody>(string resourcePath, TBody body, CancellationToken ct = default)
        {
            resourcePath = resourcePath.ReplaceParametersWithValues(body);
            return _clientFactory.PutResource(_signInRedirector, resourcePath, body, ct);
        }

        protected Task<HttpResponseMessage> PatchResource<TBody>(string resourcePath, TBody body, CancellationToken ct = default)
        {
            resourcePath = resourcePath.ReplaceParametersWithValues(body);
            return _clientFactory.PatchResource(_signInRedirector, resourcePath, body, ct);
        }
    }
}
