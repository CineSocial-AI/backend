using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.Movies.Queries.GetNewReleases;

public record GetNewReleasesQuery(int Page = 1, int PageSize = 20, int Days = 90) : IRequest<Result<object>>;
