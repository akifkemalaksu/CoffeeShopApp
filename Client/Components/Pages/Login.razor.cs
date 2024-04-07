using Azure;
using Azure.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

namespace Client.Components.Pages
{
    public partial class Login : ComponentBase
    {
        [Inject] private IHttpContextAccessor HttpContextAccessor { get; set; }
        public async Task<IActionResult> OnGetAsync(string redirectUrl)
        {
            if (string.IsNullOrWhiteSpace(redirectUrl))
                redirectUrl = HttpContextAccessor.HttpContext.Request.GetDisplayUrl();

            if (HttpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                HttpContextAccessor.HttpContext.Response.Redirect(redirectUrl);

            return new ChallengeResult(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties
            {
                RedirectUri = redirectUrl
            });
        }
    }
}
