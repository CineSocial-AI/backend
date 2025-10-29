using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Rates.Queries.GetUserRatings;

public record GetUserRatingsQuery(int UserId, int Page = 1, int PageSize = 20) : IRequest<Result<object>>;
