using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Users.Commands.UpdateBackgroundImage;

public record UpdateBackgroundImageCommand(
    int UserId,
    string FileName,
    string ContentType,
    byte[] Data
) : IRequest<Result<UpdateBackgroundImageResponse>>;
