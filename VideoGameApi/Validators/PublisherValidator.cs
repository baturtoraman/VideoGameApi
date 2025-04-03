using FluentValidation;
using VideoGameApi.Entities;

namespace VideoGameApi.Validators
{
    public class PublisherValidator : AbstractValidator<Publisher>
    {
        public PublisherValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Publisher Id must be greater than 0.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Publisher name cannot be empty.")
                .Length(5, 100)
                .WithMessage("Publisher name must be between 5 and 100 characters.");
        }
    }
}
