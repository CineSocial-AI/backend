using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Movie;
using MediatR;

namespace CineSocial.Application.Features.Languages.Queries.GetById;

public record GetLanguageByIdQuery(int Id) : IRequest<Result<Language>>;
