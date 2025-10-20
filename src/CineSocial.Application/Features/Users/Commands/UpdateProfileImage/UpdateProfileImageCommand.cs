using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Users.Commands.UpdateProfileImage;

public record UpdateProfileImageCommand(
    int UserId,
    string FileName,
    string ContentType,
    byte[] Data
) : IRequest<Result<UpdateProfileImageResponse>>;
