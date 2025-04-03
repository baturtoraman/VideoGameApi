using FluentValidation;
using VideoGameApi.Entities;

namespace VideoGameApi.Validators
{
    public class DeveloperValidator : AbstractValidator<Developer>
    {
        public DeveloperValidator()
        {
            RuleFor(d => d.Name)
                .NotEmpty().WithMessage("Developer name cannot be empty.")
                .Length(5, 100).WithMessage("Developer name must be between 5 and 100 characters.");
        }
    }
}
