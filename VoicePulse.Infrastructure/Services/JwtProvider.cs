using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VoicePulse.Application.Interfaces;
using VoicePulse.Domain.Entities;

namespace VoicePulse.Infrastructure.Services;

public class JwtProvider : IJwtProvider
{
    public (string token, int expiresIn) GenerateToken(ApplicationUser user)
    {
        Claim[] claims = [
            new(JwtRegisteredClaimNames.Sub , user.Id),
            new(JwtRegisteredClaimNames.Email , user.Email!),
            new(JwtRegisteredClaimNames.GivenName , user.FristName),
            new(JwtRegisteredClaimNames.FamilyName , user.LastName),
            new(JwtRegisteredClaimNames.Jti , Guid.NewGuid().ToString())
        ];

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("e8bcbf9ab03c6a9a59713c068f5be7e13c29c2a6"));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var expiresIn = 30;

        var token = new JwtSecurityToken(
            issuer: "VoicePulseApp",
            audience: "VoicePulseApp Users",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresIn),
            signingCredentials: signingCredentials
        );

        return(token : new JwtSecurityTokenHandler().WriteToken(token), expiresIn: expiresIn*60);
    }
}
