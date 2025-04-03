using VideoGameApi.Entities;

namespace VideoGameApi.Models.Converters
{
    public static class GenreConverter
    {
        public static GenreDto ToDto(this Genre genre)
        {
            if (genre == null)
            {
                return new GenreDto
                {
                    Id = 0,
                    Name = "Unknown Genre"
                };
            }

            return new GenreDto
            {
                Id = genre.Id,
                Name = genre.Name,
            };
        }

        public static Genre ToEntity(this GenreDto genreDto)
        {
            if (genreDto == null)
            {
                return new Genre
                {
                    Id = 0,
                    Name = "Unknown Genredto"
                };
            }

            return new Genre
            {
                Id = genreDto.Id,
                Name = genreDto.Name,
            };
        }
    }
}