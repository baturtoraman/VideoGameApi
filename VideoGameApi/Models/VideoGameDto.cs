using System.ComponentModel.DataAnnotations.Schema;

namespace VideoGameApi.Models
{
    public class VideoGameDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Platform { get; set; }

        public int? DeveloperId { get; set; }
        public required DeveloperDto Developer { get; set; }

        public int? PublisherId { get; set; }
        public PublisherDto? Publisher { get; set; }

        public VideoGameDetailsDto? VideoGameDetails { get; set; }

        public List<GenreDto>? Genres { get; set; }

        public string? ImageUrl { get; set; }
    }
}
