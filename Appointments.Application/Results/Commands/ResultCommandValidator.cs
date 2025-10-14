using FluentValidation;

namespace Appointments.Application.Results.Commands;

public abstract class ResultCommandValidator<T> : AbstractValidator<T> where T : IResultCommand
{
    protected ResultCommandValidator()
    {
        RuleFor(x => x.Complaints)
            .NotEmpty().WithMessage("The 'Complaints' field must not be empty.");

        RuleFor(x => x.Conclusion)
            .NotEmpty().WithMessage("The 'Conclusion' field must not be empty.");

        RuleFor(x => x.Recommendations)
            .NotEmpty().WithMessage("The 'Recommendations' field must not be empty.");
    }
}
