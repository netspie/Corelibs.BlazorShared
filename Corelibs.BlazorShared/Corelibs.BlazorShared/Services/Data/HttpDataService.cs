using System.Net.Http.Json;

namespace Corelibs.BlazorShared
{
    public class HttpDataService<TAccessTokenNotAvailableException> : IDataService
        where TAccessTokenNotAvailableException : Exception
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
            var resourceName = type.IsArray ? type.Name.Remove(type.Name.Length - 2, 2) : type.Name;

            try
            {
                return await GetFromJsonAsync(AuthUserTypes.Authorized, resourceName);
            }
            catch (TAccessTokenNotAvailableException accessTokenNotAvailableException)
            {
                try
                {
                    return await GetFromJsonAsync(AuthUserTypes.Anonymous, resourceName);
                }
                catch (HttpRequestException ex)
                {
                    if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        _signInRedirector.Redirect(accessTokenNotAvailableException);
                        return default;
                    }

                    throw ex;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return default;
                }
            }

            Task<TResponse> GetFromJsonAsync(string clientName, string resourceName)
            {
                var client = _clientFactory.CreateClient(clientName);
                return client.GetFromJsonAsync<TResponse>($"{resourceName}", cancellationToken);
            }
        }
    }
}
