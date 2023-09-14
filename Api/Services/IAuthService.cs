using Api.Models;

namespace Api.Services
{
    public interface IAuthService
    {
        string GenerateTokenString(LoginModel model);
        Task<bool> Login(LoginModel model);
        Task<bool> RegisterUser(LoginModel model);
    }
}