using System.ComponentModel.DataAnnotations;

namespace VideoGameApi.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password hash is required.")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required.")]
        [StringLength(50, ErrorMessage = "Role must be at most 50 characters.")]
        public string Role { get; set; } = string.Empty;

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Balance must be a positive value.")]
        public decimal Balance { get; set; } = 0; 
    }
}