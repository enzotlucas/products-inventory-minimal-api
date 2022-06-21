using ProductsInventory.API.Core.ValueObjects;

namespace ProductsInventory.API.Application.ViewModels
{
    public record UserViewModel (string Email, string Password, string Name, UserType UserType);
}