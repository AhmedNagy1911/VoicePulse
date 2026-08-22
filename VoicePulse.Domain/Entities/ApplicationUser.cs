using Microsoft.AspNetCore.Identity;

namespace VoicePulse.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string FristName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
