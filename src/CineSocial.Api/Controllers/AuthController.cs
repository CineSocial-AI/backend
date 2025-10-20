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

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return Unauthorized(result);

        return Ok(result);
    }
}
