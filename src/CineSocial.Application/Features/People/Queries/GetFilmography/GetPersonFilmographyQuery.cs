using CineSocial.Application.Common.Models;
using MediatR;

namespace CineSocial.Application.Features.People.Queries.GetFilmography;

public record GetPersonFilmographyQuery(int PersonId, int Page = 1, int PageSize = 20) : IRequest<Result<object>>;
