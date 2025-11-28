using Microsoft.AspNetCore.Mvc;
using StudentEvents.Application.Services;

namespace StudentEvents.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var token = await _authService.AuthenticateAsync(req.Email, req.Password);
            if (token == null) return Unauthorized();
            return Ok(new { token });
        }

    }

    public record LoginRequest(string Email, string Password);
}
