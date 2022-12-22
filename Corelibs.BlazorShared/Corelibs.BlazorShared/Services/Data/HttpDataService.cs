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
            var resourcePath = type.IsArray ? type.Name.Remove(type.Name.Length - 2, 2) : type.Name;
            return await _clientFactory.GetResource<TResponse, TAccessTokenNotAvailableException>(_signInRedirector, resourcePath, cancellationToken);
        }

    }

    public static class Extes
    {
        public static async Task<TResponse> GetResource<TResponse, TAccessTokenNotAvailableException>(
            this IHttpClientFactory clientFactory, ISignInRedirector signInRedirector, string resourcePath, CancellationToken cancellationToken)
            where TAccessTokenNotAvailableException : Exception
        {
            try
            {
                return await clientFactory.GetFromJsonAsync<TResponse>(AuthUserTypes.Authorized, resourcePath, cancellationToken);
            }
            catch (TAccessTokenNotAvailableException accessTokenNotAvailableException)
            {
                try
                {
                    return await clientFactory.GetFromJsonAsync<TResponse>(AuthUserTypes.Anonymous, resourcePath, cancellationToken);
                }
                catch (HttpRequestException ex)
                {
                    if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        signInRedirector.Redirect(accessTokenNotAvailableException);
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
        }

        private static Task<TResponse> GetFromJsonAsync<TResponse>(this IHttpClientFactory clientFactory, string clientName, string resourcePath, CancellationToken cancellationToken)
        {
            var client = clientFactory.CreateClient(clientName);
            return client.GetFromJsonAsync<TResponse>($"{resourcePath}", cancellationToken);
        }
    }
}
