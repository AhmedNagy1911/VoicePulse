using VoicePulse.Domain.Entities;

namespace VoicePulse.Application.Interfaces;

public interface IJwtProvider
{
    (string token , int expiresIn) GenerateToken(ApplicationUser user) ;
    string? ValidateToken(string token) ;
}
