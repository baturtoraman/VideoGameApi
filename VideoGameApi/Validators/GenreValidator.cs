using FluentValidation;

namespace VideoGameApi.Entities
{
    public class GenreValidator : AbstractValidator<Genre>
    {
        public GenreValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Genre Id must be greater than or equal to 0.");

            RuleFor(x => x.Name)
                .NotNull()
                .WithMessage("Genre name cannot be null.")
                .NotEmpty()
                .WithMessage("Genre name cannot be empty.");
        }
    }
}
