using CineSocial.Application.Features.Auth.Commands.Login;
using CineSocial.Application.Features.Auth.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CineSocial.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        _logger.LogInformation("User registration started for email: {Email}", command.Email);

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("User registration failed for email: {Email}. Error: {Error}",
                command.Email, result.Message);
            return BadRequest(result);
        }

        _logger.LogInformation("User registration completed successfully for email: {Email}", command.Email);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        _logger.LogInformation("User login attempt for email: {Email}", command.Email);

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("User login failed for email: {Email}. Error: {Error}",
                command.Email, result.Message);
            return Unauthorized(result);
        }

        _logger.LogInformation("User login successful for email: {Email}", command.Email);
        return Ok(result);
    }
}
