using Microsoft.AspNetCore.Authorization;

namespace VoicePulse.API.Filters;

public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}