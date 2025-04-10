using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VideoGameApi.Entities
{
    public class VideoGame
    {
[Range(1, int.MaxValue, ErrorMessage = "Publisher ID must be a positive integer.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(5, ErrorMessage = "Title must be at most 5 characters.")]
        [Column(TypeName = "varchar(5)")]
        public string? Title { get; set; }

        [StringLength(50, ErrorMessage = "Platform must be at most 50 characters.")]
        [Column(TypeName = "varchar(50)")]
        public string? Platform { get; set; }

        public int? PublisherId { get; set; }
        public Publisher? Publisher { get; set; }

        public VideoGameDetails? VideoGameDetails { get; set; }

        public List<Genre>? Genres { get; set; } = new List<Genre>();

        // Image URL
        [StringLength(50, ErrorMessage = "ImageUrl must be at most 50 characters.")]
        [Column(TypeName = "varchar(50)")]
        public string? ImageUrl { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock must be a non-negative integer.")]
        public int Stock { get; set; } = 0;

        [Range(0, double.MaxValue, ErrorMessage = "Price must be a non-negative value.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } = 0;
    }
}
