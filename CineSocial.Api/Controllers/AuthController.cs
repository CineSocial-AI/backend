using System.Security.Claims;
using CineSocial.Api.DTOs;
using CineSocial.Core.Features.Auth.Commands;
using CineSocial.Core.Features.Auth.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace CineSocial.Api.Controllers;

/// <summary>
/// Authentication and user management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }
    /// <summary>
    /// User login
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>JWT token and user information</returns>
    /// <response code="200">Login successful</response>
    /// <response code="400">Invalid credentials</response>
    /// <response code="401">Authentication failed</response>
    [HttpPost("login")]
    [SwaggerRequestExample(typeof(LoginRequest), typeof(Swagger.Examples.LoginRequestExample))]
    [SwaggerResponseExample(200, typeof(Swagger.Examples.AuthResponseExample))]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var command = new LoginCommand(request.Email, request.Password);
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Error });
        }

        var response = new AuthResponse
        {
            Token = result.Data!.Token,
            RefreshToken = result.Data.RefreshToken,
            ExpiresAt = result.Data.ExpiresAt,
            User = new UserDto
            {
                Id = result.Data.User.Id,
                Username = result.Data.User.Username,
                Email = result.Data.User.Email,
                FirstName = result.Data.User.FirstName ?? "",
                LastName = result.Data.User.LastName ?? "",
                Bio = result.Data.User.Bio,
                CreatedAt = result.Data.User.CreatedAt
            }
        };

        return Ok(response);
    }

    /// <summary>
    /// User registration
    /// </summary>
    /// <param name="request">Registration information</param>
    /// <returns>JWT token and user information</returns>
    /// <response code="201">Registration successful</response>
    /// <response code="400">Invalid registration data</response>
    /// <response code="409">User already exists</response>
    [HttpPost("register")]
    [SwaggerRequestExample(typeof(RegisterRequest), typeof(Swagger.Examples.RegisterRequestExample))]
    [SwaggerResponseExample(201, typeof(Swagger.Examples.AuthResponseExample))]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var command = new RegisterCommand(
            request.Username,
            request.Email, 
            request.Password,
            request.FirstName,
            request.LastName,
            request.Bio
        );
        
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Error });
        }

        var response = new AuthResponse
        {
            Token = result.Data!.Token,
            RefreshToken = result.Data.RefreshToken,
            ExpiresAt = result.Data.ExpiresAt,
            User = new UserDto
            {
                Id = result.Data.User.Id,
                Username = result.Data.User.Username,
                Email = result.Data.User.Email,
                FirstName = result.Data.User.FirstName ?? "",
                LastName = result.Data.User.LastName ?? "",
                Bio = result.Data.User.Bio,
                CreatedAt = result.Data.User.CreatedAt
            }
        };

        return CreatedAtAction(nameof(GetProfile), response);
    }

    /// <summary>
    /// Get current user profile
    /// </summary>
    /// <returns>Current user information</returns>
    /// <response code="200">Profile retrieved successfully</response>
    /// <response code="401">User not authenticated</response>
    [HttpGet("profile")]
    [Authorize]
    [SwaggerResponseExample(200, typeof(Swagger.Examples.UserDtoExample))]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetProfile()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Geçersiz token." });
        }

        var query = new GetCurrentUserQuery(userId);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
        {
            return NotFound(new { message = result.Error });
        }

        var userDto = new UserDto
        {
            Id = result.Data!.Id,
            Username = result.Data.Username,
            Email = result.Data.Email,
            FirstName = result.Data.FirstName ?? "",
            LastName = result.Data.LastName ?? "",
            Bio = result.Data.Bio,
            CreatedAt = result.Data.CreatedAt
        };

        return Ok(userDto);
    }

    /// <summary>
    /// Refresh JWT token
    /// </summary>
    /// <param name="refreshToken">Refresh token</param>
    /// <returns>New JWT token</returns>
    /// <response code="200">Token refreshed successfully</response>
    /// <response code="400">Invalid refresh token</response>
    /// <response code="401">Refresh token expired</response>
    [HttpPost("refresh")]
    [SwaggerResponseExample(200, typeof(Swagger.Examples.AuthResponseExample))]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
    {
        var command = new RefreshTokenCommand(refreshToken);
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.Error });
        }

        var response = new AuthResponse
        {
            Token = result.Data!.Token,
            RefreshToken = result.Data.RefreshToken,
            ExpiresAt = result.Data.ExpiresAt,
            User = new UserDto
            {
                Id = result.Data.User.Id,
                Username = result.Data.User.Username,
                Email = result.Data.User.Email,
                FirstName = result.Data.User.FirstName ?? "",
                LastName = result.Data.User.LastName ?? "",
                Bio = result.Data.User.Bio,
                CreatedAt = result.Data.User.CreatedAt
            }
        };

        return Ok(response);
    }

    /// <summary>
    /// User logout
    /// </summary>
    /// <returns>Confirmation of logout</returns>
    /// <response code="200">Logout successful</response>
    /// <response code="401">User not authenticated</response>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Geçersiz token." });
        }

        // Clear refresh token
        var user = await _mediator.Send(new GetCurrentUserQuery(userId));
        if (user.IsSuccess && user.Data != null)
        {
            user.Data.RefreshToken = null;
            user.Data.RefreshTokenExpiresAt = null;
        }

        return Ok(new { message = "Başarıyla çıkış yapıldı." });
    }
}