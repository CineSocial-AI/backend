using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CineSocial.Core.Application.Ports;
using CineSocial.Core.Application.DTOs.Auth;
using CineSocial.Adapters.WebAPI.DTOs.Responses;
using LoginRequest = CineSocial.Adapters.WebAPI.DTOs.Requests.LoginRequest;
using RegisterRequest = CineSocial.Adapters.WebAPI.DTOs.Requests.RegisterRequest;
using RefreshTokenRequest = CineSocial.Adapters.WebAPI.DTOs.Requests.RefreshTokenRequest;
using ForgotPasswordRequest = CineSocial.Adapters.WebAPI.DTOs.Requests.ForgotPasswordRequest;
using ResetPasswordRequest = CineSocial.Adapters.WebAPI.DTOs.Requests.ResetPasswordRequest;
using ChangePasswordRequest = CineSocial.Adapters.WebAPI.DTOs.Requests.ChangePasswordRequest;

namespace CineSocial.Adapters.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Kullanıcı kaydı
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var registerDto = new RegisterUserDto
            {
                Email = request.Email,
                Password = request.Password,
                ConfirmPassword = request.ConfirmPassword,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName
            };

            var result = await _authService.RegisterAsync(registerDto);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            var response = new AuthTokenResponse
            {
                AccessToken = result.Value!.AccessToken,
                RefreshToken = result.Value.RefreshToken,
                ExpiresAt = result.Value.ExpiresAt,
                User = new UserResponse
                {
                    Id = result.Value.User.Id,
                    Email = result.Value.User.Email,
                    UserName = result.Value.User.UserName,
                    FirstName = result.Value.User.FirstName,
                    LastName = result.Value.User.LastName,
                    FullName = result.Value.User.FullName,
                    ProfileImageUrl = result.Value.User.ProfileImageUrl,
                    EmailConfirmed = result.Value.User.EmailConfirmed
                }
            };

            return Ok(ApiResponse<AuthTokenResponse>.CreateSuccess(response, "Kayıt başarılı"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Register endpoint error");
            return StatusCode(500, ApiResponse.CreateFailure("Bir hata oluştu"));
        }
    }

    /// <summary>
    /// Kullanıcı girişi
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var loginDto = new LoginUserDto
            {
                Email = request.Email,
                Password = request.Password,
                RememberMe = request.RememberMe
            };

            var result = await _authService.LoginAsync(loginDto);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            var response = new AuthTokenResponse
            {
                AccessToken = result.Value!.AccessToken,
                RefreshToken = result.Value.RefreshToken,
                ExpiresAt = result.Value.ExpiresAt,
                User = new UserResponse
                {
                    Id = result.Value.User.Id,
                    Email = result.Value.User.Email,
                    UserName = result.Value.User.UserName,
                    FirstName = result.Value.User.FirstName,
                    LastName = result.Value.User.LastName,
                    FullName = result.Value.User.FullName,
                    ProfileImageUrl = result.Value.User.ProfileImageUrl,
                    EmailConfirmed = result.Value.User.EmailConfirmed
                }
            };

            return Ok(ApiResponse<AuthTokenResponse>.CreateSuccess(response, "Giriş başarılı"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login endpoint error");
            return StatusCode(500, ApiResponse.CreateFailure("Bir hata oluştu"));
        }
    }

    /// <summary>
    /// Token yenileme
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var result = await _authService.RefreshTokenAsync(request.RefreshToken);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            var response = new AuthTokenResponse
            {
                AccessToken = result.Value!.AccessToken,
                RefreshToken = result.Value.RefreshToken,
                ExpiresAt = result.Value.ExpiresAt,
                User = new UserResponse
                {
                    Id = result.Value.User.Id,
                    Email = result.Value.User.Email,
                    UserName = result.Value.User.UserName,
                    FirstName = result.Value.User.FirstName,
                    LastName = result.Value.User.LastName,
                    FullName = result.Value.User.FullName,
                    ProfileImageUrl = result.Value.User.ProfileImageUrl,
                    EmailConfirmed = result.Value.User.EmailConfirmed
                }
            };

            return Ok(ApiResponse<AuthTokenResponse>.CreateSuccess(response, "Token yenilendi"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RefreshToken endpoint error");
            return StatusCode(500, ApiResponse.CreateFailure("Bir hata oluştu"));
        }
    }

    /// <summary>
    /// Kullanıcı çıkışı
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            {
                await _authService.LogoutAsync(userId);
            }

            return Ok(ApiResponse.CreateSuccess("Çıkış başarılı"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Logout endpoint error");
            return StatusCode(500, ApiResponse.CreateFailure("Bir hata oluştu"));
        }
    }

    /// <summary>
    /// Email doğrulama
    /// </summary>
    [HttpGet("confirm-email")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
    {
        try
        {
            var result = await _authService.ConfirmEmailAsync(email, token);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            return Ok(ApiResponse.CreateSuccess("Email doğrulandı"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ConfirmEmail endpoint error");
            return StatusCode(500, ApiResponse.CreateFailure("Bir hata oluştu"));
        }
    }

    /// <summary>
    /// Şifre sıfırlama isteği
    /// </summary>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        try
        {
            await _authService.ForgotPasswordAsync(request.Email);
            return Ok(ApiResponse.CreateSuccess("Şifre sıfırlama linki email adresinize gönderildi"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ForgotPassword endpoint error");
            return StatusCode(500, ApiResponse.CreateFailure("Bir hata oluştu"));
        }
    }

    /// <summary>
    /// Şifre sıfırlama
    /// </summary>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        try
        {
            var resetDto = new ResetPasswordDto
            {
                Email = request.Email,
                Token = request.Token,
                NewPassword = request.NewPassword,
                ConfirmPassword = request.ConfirmPassword
            };

            var result = await _authService.ResetPasswordAsync(resetDto);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            return Ok(ApiResponse.CreateSuccess("Şifre başarıyla sıfırlandı"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ResetPassword endpoint error");
            return StatusCode(500, ApiResponse.CreateFailure("Bir hata oluştu"));
        }
    }

    /// <summary>
    /// Şifre değiştirme
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return BadRequest(ApiResponse.CreateFailure("Kullanıcı kimliği bulunamadı"));
            }

            var changeDto = new ChangePasswordDto
            {
                CurrentPassword = request.CurrentPassword,
                NewPassword = request.NewPassword,
                ConfirmPassword = request.ConfirmPassword
            };

            var result = await _authService.ChangePasswordAsync(userId, changeDto);

            if (!result.IsSuccess)
            {
                return BadRequest(ApiResponse.CreateFailure(result.ErrorMessage));
            }

            return Ok(ApiResponse.CreateSuccess("Şifre başarıyla değiştirildi"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ChangePassword endpoint error");
            return StatusCode(500, ApiResponse.CreateFailure("Bir hata oluştu"));
        }
    }
}