using AutoMapper;
using CineSocial.Core.Domain.Entities;
using CineSocial.Core.Application.DTOs.Movies;

namespace CineSocial.Core.Application.Mapping;

public class MovieMappingProfile : Profile
{
    public MovieMappingProfile()
    {
        CreateMap<Movie, MovieDto>()
            .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => src.GetAverageRating()))
            .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => src.GetReviewCount()))
            .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.MovieGenres.Select(mg => mg.Genre)))
            .ForMember(dest => dest.Cast, opt => opt.MapFrom(src => src.MovieCasts.OrderBy(mc => mc.Order)))
            .ForMember(dest => dest.Crew, opt => opt.MapFrom(src => src.MovieCrews));

        CreateMap<Movie, MovieSummaryDto>()
            .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => src.GetAverageRating()))
            .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => src.GetReviewCount()))
            .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.MovieGenres.Select(mg => mg.Genre.Name)));

        CreateMap<CreateMovieDto, Movie>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.MovieGenres, opt => opt.Ignore())
            .ForMember(dest => dest.Reviews, opt => opt.Ignore())
            .ForMember(dest => dest.MovieCasts, opt => opt.Ignore())
            .ForMember(dest => dest.MovieCrews, opt => opt.Ignore())
            .ForMember(dest => dest.Watchlists, opt => opt.Ignore())
            .ForMember(dest => dest.Ratings, opt => opt.Ignore());

        CreateMap<UpdateMovieDto, Movie>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.MovieGenres, opt => opt.Ignore())
            .ForMember(dest => dest.Reviews, opt => opt.Ignore())
            .ForMember(dest => dest.MovieCasts, opt => opt.Ignore())
            .ForMember(dest => dest.MovieCrews, opt => opt.Ignore())
            .ForMember(dest => dest.Watchlists, opt => opt.Ignore())
            .ForMember(dest => dest.Ratings, opt => opt.Ignore());

        CreateMap<Genre, GenreDto>();
        CreateMap<CreateGenreDto, Genre>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.MovieGenres, opt => opt.Ignore());

        CreateMap<Person, PersonDto>();
        CreateMap<CreatePersonDto, Person>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.MovieCasts, opt => opt.Ignore())
            .ForMember(dest => dest.MovieCrews, opt => opt.Ignore());

        CreateMap<MovieCast, CastMemberDto>()
            .ForMember(dest => dest.PersonId, opt => opt.MapFrom(src => src.PersonId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Person.Name))
            .ForMember(dest => dest.ProfilePath, opt => opt.MapFrom(src => src.Person.ProfilePath));

        CreateMap<MovieCrew, CrewMemberDto>()
            .ForMember(dest => dest.PersonId, opt => opt.MapFrom(src => src.PersonId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Person.Name))
            .ForMember(dest => dest.ProfilePath, opt => opt.MapFrom(src => src.Person.ProfilePath));
    }
}
