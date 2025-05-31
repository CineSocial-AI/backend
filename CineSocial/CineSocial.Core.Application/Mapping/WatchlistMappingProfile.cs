using AutoMapper;
using CineSocial.Core.Domain.Entities;
using CineSocial.Core.Application.DTOs.Watchlists;

namespace CineSocial.Core.Application.Mapping;

public class WatchlistMappingProfile : Profile
{
    public WatchlistMappingProfile()
    {
        CreateMap<Watchlist, WatchlistDto>()
            .ForMember(dest => dest.MovieTitle, opt => opt.MapFrom(src => src.Movie.Title))
            .ForMember(dest => dest.MoviePoster, opt => opt.MapFrom(src => src.Movie.PosterPath))
            .ForMember(dest => dest.MovieReleaseDate, opt => opt.MapFrom(src => src.Movie.ReleaseDate))
            .ForMember(dest => dest.MovieRating, opt => opt.MapFrom(src => src.Movie.VoteAverage))
            .ForMember(dest => dest.MovieGenres, opt => opt.MapFrom(src => src.Movie.MovieGenres.Select(mg => mg.Genre.Name)));

        CreateMap<AddToWatchlistDto, Watchlist>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsWatched, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.WatchedDate, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Movie, opt => opt.Ignore());

        CreateMap<UpdateWatchlistDto, Watchlist>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.MovieId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Movie, opt => opt.Ignore());
    }
}
