using JwtAuthDotNet9.Models;
using JwtAuthDotNet9.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoGameApi.Entities;

namespace JwtAuthDotNet9.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDto request)
        {
            var response = await authService.RegisterAsync(request);
            if (response == null)
                return BadRequest("Username or email already exists.");

            return Ok(new
            {
                Message = "User registered successfully. Use the token to verify your email.",
                VerificationToken = response.Data!.VerificationToken
            });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserDto request)
        {
            var result = await authService.LoginAsync(request);
            if (result == null)
                return BadRequest("Invalid username or password.");

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await authService.RefreshTokensAsync(request);
            if (result == null)
                return Unauthorized("Invalid refresh token.");

            return Ok(result);
        }

        [HttpGet]
        public IActionResult AuthenticatedOnlyEndpoint()
        {
            return Ok("You are authenticated!");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnlyEndpoint()
        {
            return Ok("You are an admin!");
        }
    }
}