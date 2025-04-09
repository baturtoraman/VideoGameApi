using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoGameApi.Services;
using VideoGameApi.Models;
namespace VideoGameApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserBalanceService _userBalanceService;

        public UserController(UserBalanceService userBalanceService)
        {
            _userBalanceService = userBalanceService;
        }

        [HttpGet("{userId}/balance")]
        public async Task<IActionResult> GetBalance(Guid userId)
        {
            var response = await _userBalanceService.GetBalanceAsync(userId);
            if (!response.Success)
            {
                return NotFound(response.Message);
            }

            return Ok(new UpdateBalanceDto { Amount = response.Data });
        }

        [HttpPost("{userId}/balance")]
        public async Task<IActionResult> UpdateBalance(Guid userId, [FromBody] UpdateBalanceDto request)
        {
            var response = await _userBalanceService.UpdateBalanceAsync(userId, request.Amount);
            if (!response.Success)
            {
                return NotFound(response.Message);
            }

            return Ok(new UpdateBalanceDto { Amount = response.Data });
        }
    }
}