namespace VoicePulse.Application.Contracts.Roles;

public record RoleRequest(
    string Name,
    IList<string> Permissions
);