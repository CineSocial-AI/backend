using Microsoft.Extensions.Logging;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using CineSocial.Core.Application.DTOs.Auth;
using CineSocial.Core.Application.DTOs.Common;
using CineSocial.Core.Application.Ports.Repositories;
using CineSocial.Core.Application.Contracts.Services;
using CineSocial.Core.Domain.Entities;

namespace CineSocial.Core.Application.Services;

/// <summary>
/// Authentication service implementation containing business logic
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ITokenService tokenService,
        IEmailService emailService,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<AuthTokenDto>> RegisterAsync(RegisterUserDto registerDto, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate business rules
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return Result<AuthTokenDto>.Failure("Bu email adresi zaten kayıtlı");
            }

            var existingUsername = await _userManager.FindByNameAsync(registerDto.UserName);
            if (existingUsername != null)
            {
                return Result<AuthTokenDto>.Failure("Bu kullanıcı adı zaten alınmış");
            }

            // Create domain entity
            var user = User.CreateUser(registerDto.Email, registerDto.FirstName, registerDto.LastName, registerDto.UserName);

            // Use Identity for password management
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return Result<AuthTokenDto>.Failure(errors);
            }

            // Add default role
            await _userManager.AddToRoleAsync(user, "User");

            // Generate tokens
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var userProfileDto = _mapper.Map<UserProfileDto>(user);
            var tokenDto = new AuthTokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = _tokenService.GetTokenExpiration(accessToken),
                User = userProfileDto
            };

            _logger.LogInformation("User registered successfully: {Email}", user.Email);
            return Result<AuthTokenDto>.Success(tokenDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration");
            return Result<AuthTokenDto>.Failure("Kayıt sırasında bir hata oluştu");
        }
    }

    public async Task<Result<AuthTokenDto>> LoginAsync(LoginUserDto loginDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !user.IsActive)
            {
                return Result<AuthTokenDto>.Failure("Geçersiz email veya şifre");
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, true);
            if (!signInResult.Succeeded)
            {
                if (signInResult.IsLockedOut)
                {
                    return Result<AuthTokenDto>.Failure("Hesap geçici olarak kilitlendi");
                }
                return Result<AuthTokenDto>.Failure("Geçersiz email veya şifre");
            }

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var userProfileDto = _mapper.Map<UserProfileDto>(user);
            var tokenDto = new AuthTokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = _tokenService.GetTokenExpiration(accessToken),
                User = userProfileDto
            };

            return Result<AuthTokenDto>.Success(tokenDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return Result<AuthTokenDto>.Failure("Giriş sırasında bir hata oluştu");
        }
    }

    public async Task<Result<AuthTokenDto>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        try
        {
            // TODO: Implement proper refresh token validation with database storage
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Result<AuthTokenDto>.Failure("Geçersiz refresh token");
            }

            // This is a placeholder - implement proper refresh token validation
            return Result<AuthTokenDto>.Failure("Refresh token implementasyonu tamamlanacak");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return Result<AuthTokenDto>.Failure("Token yenileme sırasında hata oluştu");
        }
    }

    public async Task<Result> LogoutAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            // TODO: Invalidate refresh tokens in database
            await Task.CompletedTask;
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return Result.Failure("Çıkış sırasında hata oluştu");
        }
    }

    public async Task<Result> ConfirmEmailAsync(string email, string token, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Result.Failure("Kullanıcı bulunamadı");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return Result.Failure("Email doğrulama başarısız");
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during email confirmation");
            return Result.Failure("Email doğrulama sırasında hata oluştu");
        }
    }

    public async Task<Result> ForgotPasswordAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Don't reveal if user exists or not
                return Result.Success();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"https://yourdomain.com/reset-password?email={email}&token={Uri.EscapeDataString(token)}";

            await _emailService.SendPasswordResetAsync(email, resetLink);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during forgot password");
            return Result.Failure("Şifre sıfırlama sırasında hata oluştu");
        }
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordDto resetPasswordDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                return Result.Failure("Kullanıcı bulunamadı");
            }

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return Result.Failure(errors);
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password reset");
            return Result.Failure("Şifre sıfırlama sırasında hata oluştu");
        }
    }

    public async Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return Result.Failure("Kullanıcı bulunamadı");
            }

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return Result.Failure(errors);
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password change");
            return Result.Failure("Şifre değiştirme sırasında hata oluştu");
        }
    }
}