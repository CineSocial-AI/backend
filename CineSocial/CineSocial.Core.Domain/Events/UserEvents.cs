using CineSocial.Core.Domain.Common;
using MediatR;

namespace CineSocial.Core.Domain.Events;

public class UserRegisteredEvent : DomainEvent, INotification
{
    public Guid UserId { get; }
    public string Email { get; }
    public string FirstName { get; }
    public string LastName { get; }

    public UserRegisteredEvent(Guid userId, string email, string firstName, string lastName)
    {
        UserId = userId;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
    }
}

public class UserProfileUpdatedEvent : DomainEvent, INotification
{
    public Guid UserId { get; }
    public string FirstName { get; }
    public string LastName { get; }

    public UserProfileUpdatedEvent(Guid userId, string firstName, string lastName)
    {
        UserId = userId;
        FirstName = firstName;
        LastName = lastName;
    }
}

public class UserDeactivatedEvent : DomainEvent, INotification
{
    public Guid UserId { get; }
    public string Email { get; }

    public UserDeactivatedEvent(Guid userId, string email)
    {
        UserId = userId;
        Email = email;
    }
}

public class UserLoggedInEvent : DomainEvent, INotification
{
    public Guid UserId { get; }
    public string Email { get; }
    public DateTime LoginTime { get; }

    public UserLoggedInEvent(Guid userId, string email, DateTime loginTime)
    {
        UserId = userId;
        Email = email;
        LoginTime = loginTime;
    }
}