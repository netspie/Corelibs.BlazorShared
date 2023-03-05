using Microsoft.JSInterop;

namespace Corelibs.BlazorShared.UI
{
    public static class KeyHandlersExtensions
    {
        public static ValueTask<BoundRect> AddKeyDownEventHandler<T>(this IJSRuntime jsRuntime, DotNetObjectReference<T> @objectReference, string methodName)
            where T : class
        {
            return jsRuntime.InvokeAsync<BoundRect>("addDocumentKeyDownHandler", @objectReference, methodName);
        }
    }
}
