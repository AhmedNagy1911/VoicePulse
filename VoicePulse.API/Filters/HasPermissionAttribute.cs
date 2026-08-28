using Microsoft.AspNetCore.Authorization;

namespace VoicePulse.API.Filters;

public class HasPermissionAttribute(string permission) : AuthorizeAttribute(permission)
{
}