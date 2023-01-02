using Microsoft.AspNetCore.Components;

namespace Corelibs.BlazorShared
{
    public static class NavigationExtensions
    {
        public static void NavigateToBase(this NavigationManager navigation, bool forceLoad = true) =>
            navigation.NavigateTo(navigation.BaseUri, forceLoad);
    }
}
