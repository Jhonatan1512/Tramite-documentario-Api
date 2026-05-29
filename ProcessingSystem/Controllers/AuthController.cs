using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProcessingSystem.Application.DTOs;
using ProcessingSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authentication;

namespace ProcessingSystem.Api.Controllers
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

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _authService.AuthenticateAsync(dto);
            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult> Logout()
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            if(string.IsNullOrEmpty(token)) 
                token = Request.Headers["Authorization"].ToString().Replace("Bearer", "").Trim();

            await _authService.LogOutAsync(token);
            return Ok(new {mensaje = "Sesión cerrada"});
        }
    }
}
