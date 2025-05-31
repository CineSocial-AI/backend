using CineSocial.Core.Domain.Common;

namespace CineSocial.Core.Domain.Entities;

public class Report : BaseEntity
{
    public Guid ReporterId { get; set; }
    public ReportTargetType TargetType { get; set; }
    public Guid TargetId { get; set; }
    public ReportReason Reason { get; set; }
    public string? Details { get; set; }
    public ReportStatus Status { get; set; }
    public Guid? ReviewedById { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewNotes { get; set; }

    public virtual User Reporter { get; set; } = null!;
    public virtual User? ReviewedBy { get; set; }
}

public enum ReportTargetType
{
    Post = 1,
    Comment = 2,
    User = 3,
    Group = 4
}

public enum ReportReason
{
    Spam = 1,
    Harassment = 2,
    HateSpeech = 3,
    Violence = 4,
    Inappropriate = 5,
    Copyright = 6,
    Other = 7
}

public enum ReportStatus
{
    Pending = 1,
    Reviewed = 2,
    Resolved = 3,
    Dismissed = 4
}
