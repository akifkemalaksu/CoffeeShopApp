using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoffeeShopController : ControllerBase
    {
        private readonly ICoffeeShopService _coffeeShopService;

        public CoffeeShopController(ICoffeeShopService coffeeShopService)
        {
            _coffeeShopService = coffeeShopService;
        }

        [HttpGet]
        [Authorize(Policy = "read")]
        public async Task<IActionResult> GetList()
        {
            var coffeeShops = await _coffeeShopService.List();
            return Ok(coffeeShops);
        }
    }
}
