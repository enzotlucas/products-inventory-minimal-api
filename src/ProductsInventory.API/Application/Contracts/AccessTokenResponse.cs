namespace ProductsInventory.API.Application.Contracts
{
    public record AccessTokenResponse(string TokenType, string AcecssToken, DateTime Expires, string UserId);
}
