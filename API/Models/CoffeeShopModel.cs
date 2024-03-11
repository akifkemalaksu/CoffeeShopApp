using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public record CoffeeShopModel(int Id, string Name, string OpeningHours, string Address);
}
