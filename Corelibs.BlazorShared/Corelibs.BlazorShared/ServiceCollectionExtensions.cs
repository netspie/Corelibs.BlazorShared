using Microsoft.Extensions.DependencyInjection;

namespace Corelibs.BlazorShared
{
    public static class ServiceCollectionExtensions
    {
        public static IHttpClientBuilder AddHttpClient(this IServiceCollection services, string name, string adress)
            => services.AddHttpClient(name, client => client.BaseAddress = new Uri(adress));

        public static T Get<T>(this IServiceProvider sp)
            => sp.GetRequiredService<T>();

        public static void AddAuthorizationAndSignInRedirection<TAuthUser, TSignInRedirector, TAuthorizationMessageHandler>(
            this IServiceCollection services, string baseAddress)
            where TAuthUser : class, IAuthUser
            where TSignInRedirector : class, ISignInRedirector
            where TAuthorizationMessageHandler : DelegatingHandler
        {
            services.AddHttpClient(AuthUserTypes.Authorized, baseAddress).AddHttpMessageHandler<TAuthorizationMessageHandler>();
            services.AddHttpClient(AuthUserTypes.Anonymous, baseAddress);

            services.AddTransient<IAuthUser, TAuthUser>();
            services.AddTransient<ISignInRedirector, TSignInRedirector>();
            services
                .AddHttpClient<IDataService, HttpDataService>(
                (client, sp) =>
                {
                    client.BaseAddress = new Uri(baseAddress);
                    return new HttpDataService(sp.Get<IHttpClientFactory>(), sp.Get<ISignInRedirector>());
                })
                .AddHttpMessageHandler<TAuthorizationMessageHandler>();
        }
    }
}
