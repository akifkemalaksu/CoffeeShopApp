using API.Models;
using Client.Services;
using Client.Settings;
using IdentityModel.Client;
using Microsoft.AspNetCore.Components;


namespace Client.Components.Pages
{
    public partial class CoffeeShops : ComponentBase
    {
        private List<CoffeeShopModel> Shops { get; set; }
        [Inject] private HttpClient HttpClient { get; set; }
        [Inject] private ServiceUrlSettings ServiceUrls { get; set; }
        [Inject] private ITokenService TokenService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var tokenResponse = await TokenService.GetToken("CoffeeAPI.read");
            HttpClient.SetBearerToken(tokenResponse.AccessToken);

            var url = $"{ServiceUrls.ApiUrl}/api/coffeeshop";
            var result = await HttpClient.GetAsync(url);

            if (result.IsSuccessStatusCode)
                Shops = await result.Content.ReadFromJsonAsync<List<CoffeeShopModel>>();
        }
    }
}
