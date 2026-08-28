using Microsoft.AspNetCore.Authorization;
using VoicePulse.Application.Common.Consts;

namespace VoicePulse.API.Filters;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {

        //يعني اليوزر ب null او لو كان مش null بس ال  IsAuthenticated بفولس 
        if (context.User.Identity is not { IsAuthenticated: true } ||
            !context.User.Claims.Any(x => x.Value == requirement.Permission && x.Type == Permissions.Type))
            return;

        context.Succeed(requirement);
        return;
    }
}