using VideoGameApi.Entities;
using VideoGameApi.Models.Converters;

namespace VideoGameApi.Models.Converters
{
    public static class VideoGameConverter
    {
        public static VideoGameDto ToDto(VideoGame videoGame)
        {
            return new VideoGameDto
            {
                Id = videoGame.Id,
                Title = videoGame.Title,
                Platform = videoGame.Platform,
                PublisherId = videoGame.PublisherId,
                Publisher = PublisherConverter.ToDto(videoGame.Publisher),
                VideoGameDetails = videoGame.VideoGameDetails != null ? VideoGameDetailsConverter.ToDto(videoGame.VideoGameDetails) : null,
                Genres = videoGame.Genres!.Select(g => g.ToDto()).ToList(),
                ImageUrl = videoGame.ImageUrl,
                Stock = videoGame.Stock,
                Price = videoGame.Price
            };
        }

        public static VideoGame ToEntity(VideoGameDto videoGameDto)
        {
            var videogame = new VideoGame
            {
                Id = videoGameDto.Id,
                Title = videoGameDto.Title,
                Platform = videoGameDto.Platform,
                PublisherId = videoGameDto.PublisherId,
                Publisher = PublisherConverter.ToEntity(videoGameDto.Publisher),
                VideoGameDetails = videoGameDto.VideoGameDetails != null ? VideoGameDetailsConverter.ToEntity(videoGameDto.VideoGameDetails) : null,
                Genres = videoGameDto.Genres?.Select(g => g.ToEntity()).ToList() ?? new List<Genre>(),
                ImageUrl = videoGameDto.ImageUrl,
                Stock = videoGameDto.Stock,
                Price = videoGameDto.Price
            };

            return videogame;
        }
    }
}