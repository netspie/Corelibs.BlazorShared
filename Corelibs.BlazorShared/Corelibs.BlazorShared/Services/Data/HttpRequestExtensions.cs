using Common.Basic.Blocks;
using Corelibs.Basic.Collections;
using Mediator;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net.Http.Formatting;
using System.Net.Http.Json;

namespace Corelibs.BlazorShared
{
    public static class HttpRequestExtensions
    {
        public static async Task<TResponse> GetResource<TResponse>(
            this IHttpClientFactory clientFactory, ISignInRedirector signInRedirector, string resourcePath, CancellationToken ct = default)
        {
            try
            {
                return await clientFactory.GetAsync<TResponse>(AuthUserTypes.Authorized, resourcePath, ct);
            }
            catch (AccessTokenNotAvailableException accessTokenNotAvailableException)
            {
                try
                {
                    return await clientFactory.GetAsync<TResponse>(AuthUserTypes.Anonymous, resourcePath, ct);
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

        record EmptyBody();

        public static Task<HttpResponseMessage> PostResource(
           this IHttpClientFactory clientFactory, ISignInRedirector signInRedirector, string resourcePath, CancellationToken ct = default)
        {
            return SendResource(signInRedirector, clientName => clientFactory.PostAsync(clientName, resourcePath, new EmptyBody(), ct));
        }

        public static Task<HttpResponseMessage> PostResource<TBody>(
           this IHttpClientFactory clientFactory, ISignInRedirector signInRedirector, string resourcePath, TBody body = default, CancellationToken ct = default)
           where TBody : new()
        {
            return SendResource(signInRedirector, clientName => clientFactory.PostAsync(clientName, resourcePath, body, ct));
        }

        public static Task<HttpResponseMessage> PutResource<TBody>(
           this IHttpClientFactory clientFactory, ISignInRedirector signInRedirector, string resourcePath, TBody body, CancellationToken ct)
        {
            return SendResource(signInRedirector, clientName => clientFactory.PutAsync(clientName, resourcePath, body, ct));
        }

        public static Task<HttpResponseMessage> PatchResource<TBody>(
           this IHttpClientFactory clientFactory, ISignInRedirector signInRedirector, string resourcePath, TBody body, CancellationToken ct)
        {
            return SendResource(signInRedirector, clientName => clientFactory.PatchAsync(clientName, resourcePath, body, ct));
        }

        public static Task<HttpResponseMessage> DeleteResource<TBody>(
          this IHttpClientFactory clientFactory, ISignInRedirector signInRedirector, string resourcePath, CancellationToken ct)
        {
            return SendResource(signInRedirector, clientName => clientFactory.DeleteAsync(clientName, resourcePath, ct));
        }

        private static async Task<HttpResponseMessage> SendResource(
            ISignInRedirector signInRedirector, Func<string, Task<HttpResponseMessage>> sendResourceFunc)
        {
            try
            {
                return await sendResourceFunc(AuthUserTypes.Authorized);
            }
            catch (AccessTokenNotAvailableException accessTokenNotAvailableException)
            {
                try
                {
                    return await sendResourceFunc(AuthUserTypes.Anonymous);
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

        private static Task<TResponse> GetAsync<TResponse>(this IHttpClientFactory clientFactory, string clientName, string resourcePath, CancellationToken ct)
        {
            var client = clientFactory.CreateClient(clientName);
            return HttpClientJsonExtensions.GetFromJsonAsync<TResponse>(client, resourcePath, ct);
        }

        private static Task<HttpResponseMessage> PostAsync<TBody>(this IHttpClientFactory clientFactory, string clientName, string resourcePath, TBody body, CancellationToken ct)
            where TBody : new()
        {
            body = body ?? new TBody();
            return clientFactory.CreateClientAndSendRequest(clientName, client => HttpClientJsonExtensions.PostAsJsonAsync(client, resourcePath, body, ct));
        }

        private static Task<HttpResponseMessage> PutAsync<TBody>(this IHttpClientFactory clientFactory, string clientName, string resourcePath, TBody body, CancellationToken ct) =>
            clientFactory.CreateClientAndSendRequest(clientName, client => HttpClientJsonExtensions.PutAsJsonAsync(client, resourcePath, body, ct));

        private static Task<HttpResponseMessage> PatchAsync<TBody>(this IHttpClientFactory clientFactory, string clientName, string resourcePath, TBody body, CancellationToken ct) =>
            clientFactory.CreateClientAndSendRequest(clientName, client => client.PatchAsync(resourcePath, new ObjectContent(typeof(TBody), body, new JsonMediaTypeFormatter(), "application/json"), ct));

        private static Task<HttpResponseMessage> DeleteAsync(this IHttpClientFactory clientFactory, string clientName, string resourcePath, CancellationToken ct) =>
            clientFactory.CreateClientAndSendRequest(clientName, client => client.DeleteAsync(resourcePath, ct));

        private static Task<HttpResponseMessage> CreateClientAndSendRequest(this IHttpClientFactory clientFactory, string clientName, Func<HttpClient, Task<HttpResponseMessage>> request)
        {
            var client = clientFactory.CreateClient(clientName);
            return request(client);
        }
    }
}
