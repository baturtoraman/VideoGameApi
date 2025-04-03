using VideoGameApi.Entities;

namespace VideoGameApi.Models.Converters
{
    public static class PublisherConverter
    {
        public static PublisherDto ToDto(Publisher? publisher)
        {
            if (publisher == null)
            {
                return new PublisherDto
                {
                    Id = 0,
                    Name = "Unknown Publisher"
                };
            }

            return new PublisherDto
            {
                Id = publisher.Id,
                Name = publisher.Name
            };
        }

        public static Publisher ToEntity(PublisherDto? publisherDto)
        {
            if (publisherDto == null)
            {
                return new Publisher
                {
                    Id = 0,
                    Name = "Unknown Publisher"
                };
            }

            return new Publisher
            {
                Id = publisherDto.Id,
                Name = publisherDto.Name
            };
        }
    }
}