using System.ComponentModel.DataAnnotations;

namespace VideoGameApi.Entities
{
    public class Publisher
    {
        [Range(1, int.MaxValue, ErrorMessage = "Publisher ID must be a positive integer.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Publisher name is required.")]
        [StringLength(100, ErrorMessage = "Publisher name can't be longer than 100 characters.")]
        public required string Name { get; set; }
        public List<VideoGame> VideoGames { get; set; } = new List<VideoGame>();
    }
}