using Microsoft.Extensions.Logging;
using AutoMapper;
using CineSocial.Core.Application.Ports;
using CineSocial.Core.Application.DTOs.Auth;
using CineSocial.Core.Application.DTOs.Common;
using CineSocial.Core.Application.Contracts.Services;
using CineSocial.Core.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CineSocial.Core.Application.UseCases.Auth;

public class RegisterUserUseCase : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;
    private readonly ILogger<RegisterUserUseCase> _logger;

    public RegisterUserUseCase(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ITokenService tokenService,
        IEmailService emailService,
        IMapper mapper,
        ILogger<RegisterUserUseCase> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _emailService = emailService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<AuthTokenDto>> RegisterAsync(RegisterUserDto registerDto)
    {
        try
        {
            // Check if user exists
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return Result<AuthTokenDto>.Failure("Bu email adresi zaten kayýtlý");
            }

            // Check if username exists
            var existingUsername = await _userManager.FindByNameAsync(registerDto.UserName);
            if (existingUsername != null)
            {
                return Result<AuthTokenDto>.Failure("Bu kullanýcý adý zaten alýnmýþ");
            }

            // Create user
            var user = User.CreateUser(registerDto.Email, registerDto.FirstName, registerDto.LastName, registerDto.UserName);

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
            return Result<AuthTokenDto>.Failure("Kayýt sýrasýnda bir hata oluþtu");
        }
    }

    public async Task<Result<AuthTokenDto>> LoginAsync(LoginUserDto loginDto)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !user.IsActive)
            {
                return Result<AuthTokenDto>.Failure("Geçersiz email veya þifre");
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, true);
            if (!signInResult.Succeeded)
            {
                if (signInResult.IsLockedOut)
                {
                    return Result<AuthTokenDto>.Failure("Hesap geçici olarak kilitlendi");
                }
                return Result<AuthTokenDto>.Failure("Geçersiz email veya þifre");
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
            return Result<AuthTokenDto>.Failure("Giriþ sýrasýnda bir hata oluþtu");
        }
    }

    public async Task<Result<AuthTokenDto>> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            // Basic refresh token validation (you might want to store these in database)
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Result<AuthTokenDto>.Failure("Geçersiz refresh token");
            }

            // For now, we'll just generate new tokens
            // In production, you should validate the refresh token from database

            return Result<AuthTokenDto>.Failure("Refresh token implementasyonu tamamlanacak");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return Result<AuthTokenDto>.Failure("Token yenileme sýrasýnda hata oluþtu");
        }
    }

    public async Task<Result> LogoutAsync(Guid userId)
    {
        try
        {
            // Here you would invalidate the refresh token in database
            await Task.CompletedTask;
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return Result.Failure("Çýkýþ sýrasýnda hata oluþtu");
        }
    }

    public async Task<Result> ConfirmEmailAsync(string email, string token)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Result.Failure("Kullanýcý bulunamadý");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return Result.Failure("Email doðrulama baþarýsýz");
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during email confirmation");
            return Result.Failure("Email doðrulama sýrasýnda hata oluþtu");
        }
    }

    public async Task<Result> ForgotPasswordAsync(string email)
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
            return Result.Failure("Þifre sýfýrlama sýrasýnda hata oluþtu");
        }
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                return Result.Failure("Kullanýcý bulunamadý");
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
            return Result.Failure("Þifre sýfýrlama sýrasýnda hata oluþtu");
        }
    }

    public async Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return Result.Failure("Kullanýcý bulunamadý");
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
            return Result.Failure("Þifre deðiþtirme sýrasýnda hata oluþtu");
        }
    }
}