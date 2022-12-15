namespace Corelibs.BlazorShared
{
    public interface IDataService
    {
        Task<TResponse> Get<TResponse>(CancellationToken cancellationToken = default);
    }
}
