using API.Models;
using DataAccess.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class CoffeeShopService : ICoffeeShopService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public CoffeeShopService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public Task<List<CoffeeShopModel>> List() => _applicationDbContext
                                                        .CoffeeShops
                                                        .ProjectToType<CoffeeShopModel>()
                                                        .ToListAsync();
    }
}
