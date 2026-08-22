using System;
using System.Collections.Generic;
using System.Text;

namespace VoicePulse.Domain.Entities;

public class AuditableEntity
{
    public string CreatedById { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    public string? UpdatedById { get; set; }
    public DateTime? UpdatedOn { get; set; }

    public ApplicationUser CreatedBy { get; set; } = default!;
    public ApplicationUser? UpdatedBy { get; set; }
}