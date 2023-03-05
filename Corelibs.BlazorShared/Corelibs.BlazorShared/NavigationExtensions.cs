using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Corelibs.BlazorShared
{
    public static class NavigationExtensions
    {
        public static void NavigateToBase(this NavigationManager navigation, bool forceLoad = true) =>
            navigation.NavigateTo(navigation.BaseUri, forceLoad);

        public static async Task NavigateBack(this IJSRuntime jsRuntime) =>
            await jsRuntime.InvokeVoidAsync("history.back");
    }
}
