using VideoGameApi.Entities;

namespace VideoGameApi.Models.Converters
{
    public static class DeveloperConverter
    {
        public static DeveloperDto ToDto(Developer developer)
        {
            if (developer == null)
            {
                return new DeveloperDto
                {
                    Id = 0,
                    Name = "Unknown Developer"
                };
            }

            return new DeveloperDto
            {
                Id = developer.Id,
                Name = developer.Name,
            };
        }

        public static Developer ToEntity(DeveloperDto developerDto)
        {
            if (developerDto == null)
            {
                return new Developer
                {
                    Id = 0,
                    Name = "Unknown Developer"
                };
            }
            return new Developer
            {
                Id = developerDto.Id,
                Name = developerDto.Name,
            };
        }
    }
}