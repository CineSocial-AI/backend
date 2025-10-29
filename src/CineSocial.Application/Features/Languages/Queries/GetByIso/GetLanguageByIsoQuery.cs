using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Movie;
using MediatR;

namespace CineSocial.Application.Features.Languages.Queries.GetByIso;

public record GetLanguageByIsoQuery(string Iso) : IRequest<Result<Language>>;
