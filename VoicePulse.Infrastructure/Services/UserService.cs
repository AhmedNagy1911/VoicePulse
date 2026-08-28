using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VoicePulse.Application.Common.Consts;
using VoicePulse.Application.Common.Errors;
using VoicePulse.Application.Common.Results;
using VoicePulse.Application.Contracts.Users;
using VoicePulse.Application.Interfaces;
using VoicePulse.Domain.Entities;
using VoicePulse.Infrastructure.Persistence;

namespace VoicePulse.Infrastructure.Services;

public class UserService(UserManager<ApplicationUser> userManager, ApplicationDbContext context) : IUserService
{
    private readonly UserManager<ApplicationUser> _usermanager = userManager;
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await (from u in _context.Users
               join ur in _context.UserRoles
               on u.Id equals ur.UserId
               join r in _context.Roles
               on ur.RoleId equals r.Id into roles
               where !roles.Any(x => x.Name == DefaultRoles.Member)
               select new
               {
                       u.Id,
                       u.FristName,
                       u.LastName,
                       u.Email,
                       u.UserName,
                       u.IsDisabled,
                       Roles = roles.Select(x => x.Name!).ToList()
               }
                )
                .GroupBy(u => new { u.Id, u.FristName, u.LastName, u.Email,u.UserName, u.IsDisabled })
                .Select(u => new UserResponse
                (
                        u.Key.Id,
                        u.Key.FristName,
                        u.Key.LastName,
                        u.Key.Email,
                        u.Key.UserName,
                        u.Key.IsDisabled,
                        u.SelectMany(x => x.Roles)
                ))
               .ToListAsync(cancellationToken);

    public async Task<Result<UserResponse>> GetAsync(string id)
    {
        if (await _usermanager.FindByIdAsync(id) is not { } user)
            return Result.Failure<UserResponse>(UserErrors.UserNotFound);

        var userRoles = await _usermanager.GetRolesAsync(user);

        var response = (user, userRoles).Adapt<UserResponse>();

        return Result.Success(response);
    }


    public async Task<Result<UserProfileResponse>> GetProfileAsync(string userId)
    {
        var user = await _usermanager.Users
            .Where(x => x.Id == userId)
            .ProjectToType<UserProfileResponse>()
            .SingleAsync();

        return Result.Success(user);
    }

    public async Task<Result> UpdateProfileAsync(string userId, UpdateProfileRequest request)
    {
        //var user = await _userManager.FindByIdAsync(userId);
        //user = request.Adapt(user);
        //await _userManager.UpdateAsync(user!);

        await _usermanager.Users
            .Where(x => x.Id == userId)
            .ExecuteUpdateAsync(setters =>
                setters
                    .SetProperty(x => x.FristName, request.FristName)
                    .SetProperty(x => x.LastName, request.LastName)
            );

        return Result.Success();
    }

    public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request)
    {
        var user = await _usermanager.FindByIdAsync(userId);

        var result = await _usermanager.ChangePasswordAsync(user!, request.CurrentPassword, request.NewPassword);

        if (result.Succeeded)
            return Result.Success();

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }
}
