using FluentValidation;
using VideoGameApi.Entities;
using VideoGameApi.Models;
using VideoGameApi.Models.Converters;

namespace VideoGameApi.Validators
{
    public class VideoGameValidator : AbstractValidator<VideoGameDto>
    {
        public VideoGameValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title cannot be empty.")
                .Length(3, 100)
                .WithMessage("Title must be between 3 and 100 characters.");

            RuleFor(x => x.Platform)
                .MaximumLength(50)
                .WithMessage("Platform name cannot exceed 50 characters.");

            RuleFor(x => PublisherConverter.ToEntity(x.Publisher))
                .NotNull()
                .WithMessage("Publisher cannot be null.")
                .SetValidator(new PublisherValidator())
                .WithMessage("Invalid publisher data.");

            RuleFor(x => VideoGameDetailsConverter.ToEntity(x.VideoGameDetails))
                .NotNull()
                .WithMessage("Video game details cannot be null.")
                .SetValidator(new VideoGameDetailsValidator())
                .WithMessage("Invalid video game details data.");

            RuleFor(x => x.Genres)
                .Must(g => g == null || g.Count > 0)
                .WithMessage("At least one genre must be selected.");
        }
    }
}
