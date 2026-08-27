using VoicePulse.Application.Common.Results;
using VoicePulse.Application.Contracts.Users;

namespace VoicePulse.Application.Interfaces;

public interface IUserService
{
    Task<Result<UserProfileResponse>> GetProfileAsync(string userId);
}
