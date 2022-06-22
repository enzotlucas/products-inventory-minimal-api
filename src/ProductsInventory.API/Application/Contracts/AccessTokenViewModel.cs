namespace ProductsInventory.API.Application.Contracts
{
    public record AccessTokenViewModel(string TokenType, string AcecssToken, DateTime Expires, string UserId);
}
