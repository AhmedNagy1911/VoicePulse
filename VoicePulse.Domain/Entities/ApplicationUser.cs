using Microsoft.AspNetCore.Identity;

namespace VoicePulse.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string FristName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsDisabled { get; set; }
    public List<RefreshToken> RefreshTokens { get; set; } = [];
}
