namespace VoicePulse.Application.Contracts.Questions;

public record QuestionRequest(
    string Content,
    List<string> Answers
);
