using Microsoft.AspNetCore.Identity;
using CineSocial.Core.Application.DTOs.Auth;
using CineSocial.Core.Application.DTOs.Common;
using CineSocial.Core.Application.Ports;
using CineSocial.Core.Application.Contracts.Services;
using CineSocial.Core.Domain.Entities;
using AutoMapper;

namespace CineSocial.Core.Application.UseCases.Auth;

public class RegisterUserUseCase : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;

    public RegisterUserUseCase(
        UserManager<User> userManager,
        ITokenService tokenService,
        IEmailService emailService,
        IMapper mapper)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _emailService = emailService;
        _mapper = mapper;
    }

    public async Task<Result<AuthTokenDto>> RegisterAsync(RegisterUserDto registerDto)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return Result<AuthTokenDto>.Failure("Bu email adresi zaten kullanýmda.");
            }

            // Check username availability
            var existingUsername = await _userManager.FindByNameAsync(registerDto.UserName);
            if (existingUsername != null)
            {
                return Result<AuthTokenDto>.Failure("Bu kullanýcý adý zaten kullanýmda.");
            }

            // Create user entity using domain method
            var user = User.CreateUser(
                registerDto.Email,
                registerDto.FirstName,
                registerDto.LastName,
                registerDto.UserName
            );

            // Create user with Identity
            var createResult = await _userManager.CreateAsync(user, registerDto.Password);
            if (!createResult.Succeeded)
            {
                var errors = createResult.Errors.Select(e => e.Description).ToList();
                return Result<AuthTokenDto>.Failure(errors);
            }

            // Generate email confirmation token
            var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // Send confirmation email (fire and forget)
            _ = Task.Run(async () =>
            {
                try
                {
                    var confirmationLink = $"https://yourdomain.com/confirm-email?email={user.Email}&token={emailToken}";
                    await _emailService.SendEmailConfirmationAsync(user.Email, confirmationLink);
                }
                catch
                {
                    // Log email send failure but don't fail registration
                }
            });

            // Generate tokens
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Map user to DTO
            var userProfileDto = _mapper.Map<UserProfileDto>(user);

            var authTokenDto = new AuthTokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = _tokenService.GetTokenExpiration(accessToken),
                User = userProfileDto
            };

            return Result<AuthTokenDto>.Success(authTokenDto);
        }
        catch (Exception ex)
        {
            return Result<AuthTokenDto>.Failure($"Kayýt sýrasýnda bir hata oluþtu: {ex.Message}");
        }
    }

    public async Task<Result<AuthTokenDto>> LoginAsync(LoginUserDto loginDto)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Result<AuthTokenDto>.Failure("Geçersiz email veya þifre.");
            }

            if (!user.IsActive)
            {
                return Result<AuthTokenDto>.Failure("Hesabýnýz deaktifleþtirilmiþ.");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isPasswordValid)
            {
                return Result<AuthTokenDto>.Failure("Geçersiz email veya þifre.");
            }

            // Generate tokens
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Map user to DTO
            var userProfileDto = _mapper.Map<UserProfileDto>(user);

            var authTokenDto = new AuthTokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = _tokenService.GetTokenExpiration(accessToken),
                User = userProfileDto
            };

            return Result<AuthTokenDto>.Success(authTokenDto);
        }
        catch (Exception ex)
        {
            return Result<AuthTokenDto>.Failure($"Giriþ sýrasýnda bir hata oluþtu: {ex.Message}");
        }
    }

    public async Task<Result<AuthTokenDto>> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            if (!_tokenService.ValidateToken(refreshToken))
            {
                return Result<AuthTokenDto>.Failure("Geçersiz refresh token.");
            }

            var userId = _tokenService.GetUserIdFromToken(refreshToken);
            if (!userId.HasValue)
            {
                return Result<AuthTokenDto>.Failure("Token'dan kullanýcý ID'si alýnamadý.");
            }

            var user = await _userManager.FindByIdAsync(userId.Value.ToString());
            if (user == null || !user.IsActive)
            {
                return Result<AuthTokenDto>.Failure("Kullanýcý bulunamadý veya deaktif.");
            }

            // Generate new tokens
            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            var userProfileDto = _mapper.Map<UserProfileDto>(user);

            var authTokenDto = new AuthTokenDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = _tokenService.GetTokenExpiration(newAccessToken),
                User = userProfileDto
            };

            return Result<AuthTokenDto>.Success(authTokenDto);
        }
        catch (Exception ex)
        {
            return Result<AuthTokenDto>.Failure($"Token yenileme sýrasýnda hata: {ex.Message}");
        }
    }

    public async Task<Result> LogoutAsync(Guid userId)
    {
        try
        {
            // In a more complex scenario, you might want to blacklist the token
            // For now, we just return success as client will discard the token
            await Task.CompletedTask;
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Çýkýþ sýrasýnda hata: {ex.Message}");
        }
    }

    public async Task<Result> ConfirmEmailAsync(string email, string token)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Result.Failure("Kullanýcý bulunamadý.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return Result.Failure("Email doðrulama baþarýsýz.");
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Email doðrulama sýrasýnda hata: {ex.Message}");
        }
    }

    public async Task<Result> ForgotPasswordAsync(string email)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Don't reveal that user doesn't exist
                return Result.Success();
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"https://yourdomain.com/reset-password?email={email}&token={resetToken}";

            await _emailService.SendPasswordResetAsync(email, resetLink);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Þifre sýfýrlama sýrasýnda hata: {ex.Message}");
        }
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                return Result.Failure("Kullanýcý bulunamadý.");
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
            return Result.Failure($"Þifre sýfýrlama sýrasýnda hata: {ex.Message}");
        }
    }

    public async Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return Result.Failure("Kullanýcý bulunamadý.");
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
            return Result.Failure($"Þifre deðiþtirme sýrasýnda hata: {ex.Message}");
        }
    }
}