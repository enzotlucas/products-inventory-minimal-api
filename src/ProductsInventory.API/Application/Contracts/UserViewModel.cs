namespace ProductsInventory.API.Application.Contracts
{
    public record UserViewModel (string Email, string Password, string Name, UserType UserType);
}