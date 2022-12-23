namespace Corelibs.BlazorShared
{
    public class HttpDataService : IDataService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ISignInRedirector _signInRedirector;

        public HttpDataService(IHttpClientFactory clientFactory, ISignInRedirector signInRedirector)
        {
            _clientFactory = clientFactory;
            _signInRedirector = signInRedirector;
        }

        public async Task<TResponse> Get<TResponse>(CancellationToken cancellationToken = default)
        {
            var type = typeof(TResponse);
            var resourcePath = type.IsArray ? type.Name.Remove(type.Name.Length - 2, 2) : type.Name;
            return await _clientFactory.GetResource<TResponse>(_signInRedirector, resourcePath, cancellationToken);
        }

    }
}
