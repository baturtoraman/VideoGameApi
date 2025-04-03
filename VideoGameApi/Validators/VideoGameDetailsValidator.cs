using FluentValidation;
using VideoGameApi.Entities;

namespace VideoGameApi.Validators
{
    public class VideoGameDetailsValidator : AbstractValidator<VideoGameDetails>
    {
        public VideoGameDetailsValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("VideoGameDetails ID must be greater than 0.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description cannot be empty.")
                .Length(3, 500).WithMessage("Description must be between 3 and 500 characters.");

            RuleFor(x => x.ReleaseDate)
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Release date cannot be in the future.");

            RuleFor(x => x.VideoGameId)
                .GreaterThan(0).WithMessage("VideoGameId must be greater than 0.");
        }
    }
}
