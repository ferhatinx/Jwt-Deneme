using Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api.Services;
public class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthService(UserManager<IdentityUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public string GenerateTokenString(LoginModel model)
    {
        IEnumerable<Claim> userClaims = new List<Claim>()
        {
            new Claim(ClaimTypes.Email,model.UserName),
            new Claim(ClaimTypes.Role,"Admin"),

        };
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value));
        var signinCredential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature); 
        var securityToken = new JwtSecurityToken(
        claims :userClaims,
        expires: DateTime.Now.AddMinutes(60),
        issuer:_configuration.GetSection("Jwt:Issuer").Value,
        audience: _configuration.GetSection("Jwt:Audience").Value,
        signingCredentials:signinCredential
            
        );
        var tokenString = new JwtSecurityTokenHandler().WriteToken(securityToken);
        return tokenString;
    }

    public async Task<bool> Login(LoginModel model)
    {
        var identityUser = await _userManager.FindByNameAsync(model.UserName);
        if(identityUser is null) 
        {
            return false;
        }
       return await _userManager.CheckPasswordAsync(identityUser, model.Password);
    }

    public async Task<bool> RegisterUser(LoginModel model)
    {
        var IdentityUser = new IdentityUser()
        {
            UserName = model.UserName,
            Email = model.UserName
        };
        var result = await _userManager.CreateAsync(IdentityUser, model.Password);
        return result.Succeeded;
    }
}
