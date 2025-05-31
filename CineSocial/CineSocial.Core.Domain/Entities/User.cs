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

    // Navigation Properties - þimdilik yok
    // public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    // Domain Events
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

    // Domain Methods
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
}