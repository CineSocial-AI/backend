using CineSocial.Application.Common.Models;
using CineSocial.Domain.Entities.Movie;
using MediatR;

namespace CineSocial.Application.Features.Languages.Queries.GetAll;

public record GetAllLanguagesQuery : IRequest<Result<List<Language>>>;
