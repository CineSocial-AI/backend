using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Movie;
using MediatR;

namespace CineSocial.Application.Features.Keywords.Queries.GetById;

public record GetKeywordByIdQuery(int Id) : IRequest<Result<Keyword>>;
