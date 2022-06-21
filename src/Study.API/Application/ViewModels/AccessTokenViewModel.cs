namespace Study.API.Application.ViewModels
{
    public record AccessTokenViewModel(string TokenType, string AcecssToken, DateTime Expires, string UserId);
}
