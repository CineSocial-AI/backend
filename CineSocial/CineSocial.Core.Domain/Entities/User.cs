using Microsoft.AspNetCore.Identity;
using CineSocial.Core.Domain.Common;
using CineSocial.Core.Domain.Events;

namespace CineSocial.Core.Domain.Entities;

public class User : IdentityUser<Guid>, IAuditableEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Bio { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public static User CreateUser(string email, string firstName, string lastName, string userName)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            UserName = userName,
            FirstName = firstName,
            LastName = lastName,
            EmailConfirmed = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        user.AddDomainEvent(new UserRegisteredEvent(user.Id, user.Email, user.FirstName, user.LastName));
        return user;
    }

    public void UpdateProfile(string firstName, string lastName, string? bio, DateTime? dateOfBirth)
    {
        FirstName = firstName;
        LastName = lastName;
        Bio = bio;
        DateOfBirth = dateOfBirth;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new UserProfileUpdatedEvent(Id, firstName, lastName));
    }

    public void UpdateProfileImage(string imageUrl)
    {
        ProfileImageUrl = imageUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void DeactivateAccount()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new UserDeactivatedEvent(Id, Email ?? string.Empty));
    }

    public string GetFullName() => $"{FirstName} {LastName}".Trim();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    public virtual ICollection<Watchlist> Watchlists { get; set; } = new List<Watchlist>();
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public virtual ICollection<ReviewLike> ReviewLikes { get; set; } = new List<ReviewLike>();
    public virtual ICollection<CommentLike> CommentLikes { get; set; } = new List<CommentLike>();
    public virtual ICollection<Group> CreatedGroups { get; set; } = new List<Group>();
    public virtual ICollection<GroupMember> GroupMemberships { get; set; } = new List<GroupMember>();
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    public virtual ICollection<PostComment> PostComments { get; set; } = new List<PostComment>();
    public virtual ICollection<PostReaction> PostReactions { get; set; } = new List<PostReaction>();
    public virtual ICollection<CommentReaction> CommentReactions { get; set; } = new List<CommentReaction>();
    public virtual ICollection<GroupBan> GroupBans { get; set; } = new List<GroupBan>();
    public virtual ICollection<GroupBan> IssuedBans { get; set; } = new List<GroupBan>();
    public virtual ICollection<UserBlock> BlockedUsers { get; set; } = new List<UserBlock>();
    public virtual ICollection<UserBlock> BlockedByUsers { get; set; } = new List<UserBlock>();
    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
    public virtual ICollection<Report> ReviewedReports { get; set; } = new List<Report>();
    public virtual ICollection<Following> Followers { get; set; } = new List<Following>();
    public virtual ICollection<Following> Followings { get; set; } = new List<Following>();
}