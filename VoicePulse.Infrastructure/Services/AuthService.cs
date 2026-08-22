using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using VoicePulse.Application.Contracts.Authentication;
using VoicePulse.Application.Interfaces;
using VoicePulse.Domain.Entities;

namespace VoicePulse.Infrastructure.Services;

public class AuthService(UserManager<ApplicationUser> userManager , IJwtProvider jwtProvider) : IAuthService
{
    private readonly UserManager<ApplicationUser> _usermanager = userManager;
    private readonly IJwtProvider _jwtprovider = jwtProvider;

    private readonly int _refreshTokenEpiryDays = 14;
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

        var (token, expiresIn) = _jwtprovider.GenerateToken(user);
        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenEpiryDays);

        //Return New AuthResponse() 
        return new AuthResponse(user.Id , user.Email , user.FristName ,user.LastName , token , expiresIn , refreshToken, refreshTokenExpiration);
    }

    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}
