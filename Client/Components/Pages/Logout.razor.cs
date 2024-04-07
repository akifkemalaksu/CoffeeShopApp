using Client.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Client.Components.Pages
{
    public partial class Logout : ComponentBase
    {
        private readonly ServiceUrlSettings serviceUrlSettings;

        public Logout(ServiceUrlSettings serviceUrlSettings)
        {
            this.serviceUrlSettings = serviceUrlSettings;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            return new SignOutResult([OpenIdConnectDefaults.AuthenticationScheme, CookieAuthenticationDefaults.AuthenticationScheme],
                new AuthenticationProperties
                {
                    RedirectUri = serviceUrlSettings.ApplicationUrl
                }
                );
        }
    }
}
