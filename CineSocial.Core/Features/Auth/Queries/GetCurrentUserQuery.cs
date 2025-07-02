using CineSocial.Core.Shared;
using CineSocial.Core.Shared.Interfaces;
using CineSocial.Domain.Entities;
using MediatR;

namespace CineSocial.Core.Features.Auth.Queries;

public record GetCurrentUserQuery(Guid UserId) : IRequest<Result<User>>;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, Result<User>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCurrentUserQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<User>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user == null)
        {
            return Result<User>.Failure("Kullanıcı bulunamadı.");
        }

        if (!user.IsActive)
        {
            return Result<User>.Failure("Hesap aktif değil.");
        }

        return Result<User>.Success(user);
    }
}