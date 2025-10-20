using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Users.Commands.UpdateProfile;

public record UpdateProfileCommand(
    int UserId,
    string? Username,
    string? Bio,
    int? ProfileImageId,
    int? BackgroundImageId
) : IRequest<Result<UpdateProfileResponse>>;
