using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
       private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if(ModelState.IsValid)
            {
                var result = await _authService.Login(model);
                if (result)
                {
                    var tokenString = _authService.GenerateTokenString(model);  
                    return Ok(tokenString);
                }
                return BadRequest("hata");
            }
            return BadRequest("Konrol ediniz");
          
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(LoginModel model)
        {
            if (ModelState.IsValid)
            {
               if( await _authService.RegisterUser(model))
                    return Ok("Successfuly");
                return BadRequest();

            }
            return BadRequest();
           
        }
    }
}
