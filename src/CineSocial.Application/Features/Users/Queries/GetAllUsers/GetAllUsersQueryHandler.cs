using CineSocial.Application.Common.Interfaces;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.User;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CineSocial.Application.Features.Users.Queries.GetAllUsers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<object>>
{
    private readonly IRepository<AppUser> _userRepository;

    public GetAllUsersQueryHandler(IRepository<AppUser> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<object>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _userRepository.GetQueryable()
                .Where(u => !u.IsDeleted);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(u => u.Username.Contains(request.SearchTerm) || u.Email.Contains(request.SearchTerm));
            }

            var total = await query.CountAsync(cancellationToken);

            var users = await query
                .OrderBy(u => u.Username)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var result = new
            {
                data = users,
                total,
                page = request.Page,
                pageSize = request.PageSize
            };

            return Result<object>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<object>.Failure($"Failed to retrieve users: {ex.Message}");
        }
    }
}
