using System.ComponentModel.DataAnnotations;

namespace VideoGameApi.Entities
{
    public class Developer
    {
        [Range(1, int.MaxValue, ErrorMessage = "Developer ID must be a positive integer.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Developer name is required.")]
        [StringLength(100, ErrorMessage = "Developer name can't be longer than 100 characters.")]
        public required string Name { get; set; }
    }
}