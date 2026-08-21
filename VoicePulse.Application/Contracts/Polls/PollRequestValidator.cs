using FluentValidation;

namespace VoicePulse.Application.Contracts.Polls;

public class PollRequestValidator : AbstractValidator<PollRequest>
{
    public PollRequestValidator()
    {
        RuleFor(p => p.Title)
            .NotEmpty()
            .Length(3,100);

        RuleFor(p => p.Summary)
            .NotEmpty()
            .Length(3,1500);
    } 
}
