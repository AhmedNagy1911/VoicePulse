using VoicePulse.Application.Common.Results;
using VoicePulse.Application.Contracts.Roles;

namespace VoicePulse.Application.Interfaces;

public interface IRoleService
{
    Task<IEnumerable<RoleResponse>> GetAllAsync(bool? includeDisabled = false, CancellationToken cancellationToken = default);
    Task<Result<RoleDetailResponse>> GetAsync(string id);
}
