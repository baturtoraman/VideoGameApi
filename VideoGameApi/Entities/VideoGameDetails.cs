using System.ComponentModel.DataAnnotations;

namespace VideoGameApi.Entities
{
    public class VideoGameDetails
    {
        [Range(1, int.MaxValue, ErrorMessage = "Publisher ID must be a positive integer.")]
        public int Id { get; set; }

        [StringLength(100, ErrorMessage = "Description must be at most 100 characters.")]
        public string? Description { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Release date is required.")]
        public DateTime ReleaseDate { get; set; }

        [Required]
        public int VideoGameId { get; set; }
        public VideoGame VideoGame { get; set; } = null!;
    }
}