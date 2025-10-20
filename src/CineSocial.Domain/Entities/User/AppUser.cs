using CineSocial.Domain.Common;
using CineSocial.Domain.Enums;

namespace CineSocial.Domain.Entities.User;

public class AppUser : BaseAuditableEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;

    public int? ProfileImageId { get; set; }
    public Image? ProfileImage { get; set; }

    public int? BackgroundImageId { get; set; }
    public Image? BackgroundImage { get; set; }

    public string? Bio { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;
}
