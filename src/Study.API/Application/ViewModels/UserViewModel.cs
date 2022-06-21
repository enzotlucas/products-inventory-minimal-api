using Study.API.Core.ValueObjects;

namespace Study.API.Application.ViewModels
{
    public record UserViewModel (string Email, string Password, string Name, UserType UserType);
}