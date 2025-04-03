using System.Text.Json.Serialization;

namespace VideoGameApi.Models
{
    public class GenreDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public List<VideoGameDto>? VideoGames { get; set; }
    }
}
