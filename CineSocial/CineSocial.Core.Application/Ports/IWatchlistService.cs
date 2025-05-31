using CineSocial.Core.Application.DTOs.Common;
using CineSocial.Core.Application.DTOs.Watchlists;

namespace CineSocial.Core.Application.Ports;

public interface IWatchlistService
{
    Task<Result<PagedResult<WatchlistDto>>> GetUserWatchlistAsync(Guid userId, int page = 1, int pageSize = 20, bool? isWatched = null);
    Task<Result<WatchlistSummaryDto>> GetWatchlistSummaryAsync(Guid userId);
    Task<Result<WatchlistDto>> AddToWatchlistAsync(Guid userId, AddToWatchlistDto addDto);
    Task<Result<WatchlistDto>> UpdateWatchlistAsync(Guid userId, Guid movieId, UpdateWatchlistDto updateDto);
    Task<Result> RemoveFromWatchlistAsync(Guid userId, Guid movieId);
    Task<Result<bool>> IsInWatchlistAsync(Guid userId, Guid movieId);
}

