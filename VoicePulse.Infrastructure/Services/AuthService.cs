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

        // Add RefreshToken
        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenEpiryDays);

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            ExpiresOn = refreshTokenExpiration
        });

        await _usermanager.UpdateAsync(user);

        //Return New AuthResponse() 
        return new AuthResponse(user.Id , user.Email , user.FristName ,user.LastName , token , expiresIn , refreshToken, refreshTokenExpiration);
    }


    public async Task<AuthResponse?> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        //chech userid? 
        var userId =_jwtprovider.ValidateToken(token);

        if (userId is null)
            return null;

        ////chech user?
        var user =await _usermanager.FindByIdAsync(userId);

        if (user is null)
            return null;

        //chech token?
        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

        if (userRefreshToken is null)
            return null;

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        //generate JWT NewToken
        var (newToken, expiresIn) = _jwtprovider.GenerateToken(user);

        // Add NewRefreshToken
        var newRefreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenEpiryDays);

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = newRefreshToken,
            ExpiresOn = refreshTokenExpiration
        });

        await _usermanager.UpdateAsync(user);

        //Return New AuthResponse() 
        return new AuthResponse(user.Id, user.Email, user.FristName, user.LastName, newToken, expiresIn, newRefreshToken, refreshTokenExpiration);

    }


    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}
