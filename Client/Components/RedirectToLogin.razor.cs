using Microsoft.AspNetCore.Components;

namespace Client.Components
{
    public partial class RedirectToLogin
    {
        [Inject] public NavigationManager NavigationManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            NavigationManager.NavigateTo($"/login?redirectUrl={Uri.EscapeUriString(NavigationManager.Uri)}", true);
        }
    }
}
