using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Rates.Queries.GetMyRatings;

public record GetMyRatingsQuery(int UserId, int Page = 1, int PageSize = 20) : IRequest<Result<object>>;
