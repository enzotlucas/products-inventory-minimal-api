namespace ProductsInventory.API.Application.Contracts
{
    public record CreateAccountRequest(string Email, string Password, string Name, UserType UserType);
}