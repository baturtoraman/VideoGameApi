using JwtAuthDotNet9.Models;
using VideoGameApi.Entities;
using VideoGameApi.Responses;

namespace JwtAuthDotNet9.Services
{
    public interface IAuthService
    {
        Task<ResponseModel<CustomResponse>> RegisterAsync(UserDto request);
        Task<TokenResponseDto?> LoginAsync(UserDto request);
        Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request);
    }
}
