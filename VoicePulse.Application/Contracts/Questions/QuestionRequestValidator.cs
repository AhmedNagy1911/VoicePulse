using FluentValidation;

namespace VoicePulse.Application.Contracts.Questions;

public class QuestionRequestValidator : AbstractValidator<QuestionRequest>
{
    public QuestionRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .Length(3, 1000);

        RuleFor(x => x.Answers)
            .NotNull();

        RuleFor(x => x.Answers)
            .Must(x => x.Count > 1)
            .WithMessage("Question should has at least 2 answers")
            .When(x => x.Answers != null); 

        RuleFor(x => x.Answers)
            .Must(x => x.Distinct().Count() == x.Count)
             .WithMessage("You cannot add duplicated answers for the same question")
            .When(x => x.Answers != null);

    }
}
