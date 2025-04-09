using JwtAuthDotNet9.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using VideoGameApi.Data;
using VideoGameApi.Entities;
using VideoGameApi.Responses;

namespace JwtAuthDotNet9.Services
{
    public class AuthService(VideoGameDbContext context, IConfiguration configuration) : IAuthService
    {
        public async Task<TokenResponseDto?> LoginAsync(UserDto request)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user is null)
            {
                return null;
            }
            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password)
                == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return await CreateTokenResponse(user);
        }

        private async Task<TokenResponseDto> CreateTokenResponse(User? user)
        {
            return new TokenResponseDto
            {
                AccessToken = CreateToken(user!),
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user!)
            };
        }

        public async Task<ResponseModel<CustomResponse>> RegisterAsync(UserDto request)
        {
            try
            {
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Username = request.Username,
                    Email = request.Email,
                    PasswordHash = new PasswordHasher<User>().HashPassword(null, request.Password),
                    Role = request.Role
                };

                context.Users.Add(user);
                await context.SaveChangesAsync();

                return new ResponseModel<CustomResponse>(true, "User registered successfully.", new CustomResponse
                {
                    Message = "User registered successfully.",
                    VerificationToken = "GeneratedTokenHere"
                });
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException?.Message.Contains("UNIQUE") == true)
                {
                    return new ResponseModel<CustomResponse>(false, "Email is already in use.", null);
                }
                return new ResponseModel<CustomResponse>(false, "An error occurred while registering the user.", null);
            }
        }

        public async Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request)
        {
            //bool değer ya da int değer dönebilirsin
            var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);
            if (user is null)
                return null;

            return await CreateTokenResponse(user);
        }

        private async Task<User?> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
        {
            //ihtiyacın olan alanı getir sadece tüm user datasını getirme
            var user = await context.Users.FindAsync(userId);
            if (user is null || user.RefreshToken != refreshToken
                || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return null;
            }

            return user;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
        {
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await context.SaveChangesAsync();
            return refreshToken;
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
