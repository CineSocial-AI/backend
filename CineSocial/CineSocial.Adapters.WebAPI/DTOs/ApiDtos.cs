using System.ComponentModel.DataAnnotations;

namespace CineSocial.Adapters.WebAPI.DTOs.Requests;

public class RegisterRequest
{
    [Required(ErrorMessage = "Email gerekli")]
    [EmailAddress(ErrorMessage = "Geçersiz email formatý")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Þifre gerekli")]
    [MinLength(6, ErrorMessage = "Þifre en az 6 karakter olmalý")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Þifre tekrarý gerekli")]
    [Compare("Password", ErrorMessage = "Þifreler eþleþmiyor")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ad gerekli")]
    [MinLength(2, ErrorMessage = "Ad en az 2 karakter olmalý")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Soyad gerekli")]
    [MinLength(2, ErrorMessage = "Soyad en az 2 karakter olmalý")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kullanýcý adý gerekli")]
    [MinLength(3, ErrorMessage = "Kullanýcý adý en az 3 karakter olmalý")]
    public string UserName { get; set; } = string.Empty;
}

public class LoginRequest
{
    [Required(ErrorMessage = "Email gerekli")]
    [EmailAddress(ErrorMessage = "Geçersiz email formatý")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Þifre gerekli")]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; } = false;
}

public class RefreshTokenRequest
{
    [Required(ErrorMessage = "Refresh token gerekli")]
    public string RefreshToken { get; set; } = string.Empty;
}

public class ForgotPasswordRequest
{
    [Required(ErrorMessage = "Email gerekli")]
    [EmailAddress(ErrorMessage = "Geçersiz email formatý")]
    public string Email { get; set; } = string.Empty;
}

public class ResetPasswordRequest
{
    [Required(ErrorMessage = "Email gerekli")]
    [EmailAddress(ErrorMessage = "Geçersiz email formatý")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Token gerekli")]
    public string Token { get; set; } = string.Empty;

    [Required(ErrorMessage = "Yeni þifre gerekli")]
    [MinLength(6, ErrorMessage = "Þifre en az 6 karakter olmalý")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Þifre tekrarý gerekli")]
    [Compare("NewPassword", ErrorMessage = "Þifreler eþleþmiyor")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class ChangePasswordRequest
{
    [Required(ErrorMessage = "Mevcut þifre gerekli")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Yeni þifre gerekli")]
    [MinLength(6, ErrorMessage = "Þifre en az 6 karakter olmalý")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Þifre tekrarý gerekli")]
    [Compare("NewPassword", ErrorMessage = "Þifreler eþleþmiyor")]
    public string ConfirmPassword { get; set; } = string.Empty;
}