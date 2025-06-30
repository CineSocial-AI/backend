using CineSocial.Core.Domain.Common;

namespace CineSocial.Core.Domain.Entities;

public class Group : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Rules { get; set; }
    public string? IconUrl { get; set; }
    public string? BannerUrl { get; set; }
    public bool IsPrivate { get; set; }
    public bool RequireApproval { get; set; }
    public bool IsNsfw { get; set; }
    public int MemberCount { get; set; }
    public int PostCount { get; set; } 
    public Guid CreatedById { get; set; }

    public new virtual User CreatedBy { get; set; } = null!;
    public virtual ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    public virtual ICollection<GroupBan> Bans { get; set; } = new List<GroupBan>();
}