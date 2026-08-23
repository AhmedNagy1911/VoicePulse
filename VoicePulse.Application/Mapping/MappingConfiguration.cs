using Mapster;
using VoicePulse.Application.Contracts.Questions;
using VoicePulse.Domain.Entities;

namespace VoicePulse.Application.Mapping;

public class MappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<QuestionRequest, Question>()
            .Map(dest => dest.Answers, src => src.Answers.Select(answer => new Answer { Content = answer }));
    }
}

