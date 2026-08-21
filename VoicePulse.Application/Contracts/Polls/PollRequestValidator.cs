using FluentValidation;

namespace VoicePulse.Application.Contracts.Polls;

public class PollRequestValidator : AbstractValidator<PollRequest>
{
    public PollRequestValidator()
    {
        RuleFor(p => p.Title)
            .NotEmpty()
            .Length(3,100);

        RuleFor(p => p.Description)
            .NotEmpty()
            .Length(3,1000);
    } 
}
