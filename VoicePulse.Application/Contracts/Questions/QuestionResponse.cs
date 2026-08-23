using VoicePulse.Application.Contracts.Answers;

namespace VoicePulse.Application.Contracts.Questions;

public record QuestionResponse(
    int Id, 
    string Content,
    IEnumerable<AnswerResponse> Answers
);
