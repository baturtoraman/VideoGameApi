using VideoGameApi.Entities;

namespace VideoGameApi.Models.Converters
{
    public static class VideoGameDetailsConverter
    {
        public static VideoGameDetailsDto ToDto(VideoGameDetails videoGameDetails)
        {
            if(videoGameDetails is null)
                return new VideoGameDetailsDto
                {
                    Id = 0,
                    Description = null,
                    ReleaseDate = DateTime.MinValue,
                    VideoGameId = 0,
                };
            return new VideoGameDetailsDto
            {
                Id = videoGameDetails.Id,
                Description = videoGameDetails.Description,
                ReleaseDate = videoGameDetails.ReleaseDate,
                VideoGameId = videoGameDetails.VideoGameId
            };
        }

        public static VideoGameDetails ToEntity(VideoGameDetailsDto? videoGameDetailsDto)
        {
            if (videoGameDetailsDto is null)
                return new VideoGameDetails
                {
                    Id = 0,
                    Description = null,
                    ReleaseDate = DateTime.MinValue,
                    VideoGameId = 0,
                };
            return new VideoGameDetails
            {
                Id = videoGameDetailsDto.Id,
                Description = videoGameDetailsDto.Description,
                ReleaseDate = videoGameDetailsDto.ReleaseDate,
                VideoGameId = videoGameDetailsDto.VideoGameId
            };
        }
    }
}