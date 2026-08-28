using Mapster;
using VoicePulse.Application.Contracts.Questions;
using VoicePulse.Application.Contracts.Users;
using VoicePulse.Domain.Entities;

namespace VoicePulse.Application.Mapping;

public class MappingConfiguration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<QuestionRequest, Question>()
            .Map(dest => dest.Answers, src => src.Answers.Select(answer => new Answer { Content = answer }));

        config.NewConfig<(ApplicationUser user, IList<string> roles), UserResponse>()
            .Map(dest => dest, src => src.user)
            .Map(dest => dest.Roles, src => src.roles);

        config.NewConfig<CreateUserRequest, ApplicationUser>()
            .Map(dest => dest.EmailConfirmed, src => true);

    }
}

