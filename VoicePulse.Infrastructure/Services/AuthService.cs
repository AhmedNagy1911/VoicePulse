using Microsoft.AspNetCore.Identity;
using VoicePulse.Application.Contracts.Authentication;
using VoicePulse.Application.Interfaces;
using VoicePulse.Domain.Entities;

namespace VoicePulse.Infrastructure.Services;

public class AuthService(UserManager<ApplicationUser> userManager) : IAuthService
{
    private readonly UserManager<ApplicationUser> _usermanager = userManager;

    public async Task<AuthResponse?> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        //chech user?
        var user = await _usermanager.FindByEmailAsync(email);

        if (user is null)
            return null;

        //chech password
        var isValidPassword =await _usermanager.CheckPasswordAsync(user, password);

        if(!isValidPassword)
            return null;

        //generate JWT token

        //Return New AuthResponse() 

        return new AuthResponse(user.Id , user.Email , user.FristName ,user.LastName , "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiYWRtaW4iOnRydWUsImlhdCI6MTUxNjIzOTAyMn0.KMUFsIDTnFmyG3nMiGM6H9FNFUROf3wh7SmqJp-QV30" ,3600);
    }
}
