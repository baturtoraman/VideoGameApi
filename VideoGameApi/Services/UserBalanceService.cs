using VideoGameApi.Data;
using VideoGameApi.Entities;
using VideoGameApi.Responses;

namespace VideoGameApi.Services
{
    public class UserBalanceService
    {
        private readonly VideoGameDbContext _context;

        public UserBalanceService(VideoGameDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel<decimal>> GetBalanceAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return new ResponseModel<decimal>(false, "User not found.", 0);
            }

            return new ResponseModel<decimal>(true, "Balance retrieved successfully.", user.Balance);
        }

        public async Task<ResponseModel<decimal>> UpdateBalanceAsync(Guid userId, decimal amount)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return new ResponseModel<decimal>(false, "User not found.", 0);
            }

            // Negatif bakiye kontrolü
            if (user.Balance + amount < 0)
            {
                return new ResponseModel<decimal>(false, "Insufficient balance.", user.Balance);
            }

            user.Balance += amount;
            await _context.SaveChangesAsync();

            return new ResponseModel<decimal>(true, "Balance updated successfully.", user.Balance);
        }
    }
}