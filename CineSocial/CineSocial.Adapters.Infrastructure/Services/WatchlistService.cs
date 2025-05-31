using Microsoft.EntityFrameworkCore;
using AutoMapper;
using CineSocial.Core.Application.Ports;
using CineSocial.Core.Application.DTOs.Common;
using CineSocial.Core.Application.DTOs.Watchlists;
using CineSocial.Core.Domain.Entities;
using CineSocial.Adapters.Infrastructure.Database;

namespace CineSocial.Adapters.Infrastructure.Services;

public class WatchlistService : IWatchlistService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public WatchlistService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<WatchlistDto>>> GetUserWatchlistAsync(Guid userId, int page = 1, int pageSize = 20, bool? isWatched = null)
    {
        try
        {
            var query = _context.Watchlists
                .Include(w => w.Movie)
                .ThenInclude(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
                .Where(w => w.UserId == userId);

            if (isWatched.HasValue)
                query = query.Where(w => w.IsWatched == isWatched.Value);

            query = query.OrderByDescending(w => w.CreatedAt);

            var totalCount = await query.CountAsync();
            var watchlistItems = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var watchlistDtos = _mapper.Map<List<WatchlistDto>>(watchlistItems);
            var result = new PagedResult<WatchlistDto>(watchlistDtos, totalCount, page, pageSize);
            return Result<PagedResult<WatchlistDto>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<PagedResult<WatchlistDto>>.Failure($"Error getting watchlist: {ex.Message}");
        }
    }

    public async Task<Result<WatchlistSummaryDto>> GetWatchlistSummaryAsync(Guid userId)
    {
        try
        {
            var watchlistItems = await _context.Watchlists
                .Include(w => w.Movie)
                .ThenInclude(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
                .Where(w => w.UserId == userId)
                .ToListAsync();

            var summary = new WatchlistSummaryDto
            {
                TotalMovies = watchlistItems.Count,
                WatchedMovies = watchlistItems.Count(w => w.IsWatched),
                UnwatchedMovies = watchlistItems.Count(w => !w.IsWatched),
                RecentlyAdded = _mapper.Map<List<WatchlistDto>>(
                    watchlistItems.OrderByDescending(w => w.CreatedAt).Take(5)
                ),
                RecentlyWatched = _mapper.Map<List<WatchlistDto>>(
                    watchlistItems.Where(w => w.IsWatched && w.WatchedDate.HasValue)
                                 .OrderByDescending(w => w.WatchedDate).Take(5)
                )
            };

            return Result<WatchlistSummaryDto>.Success(summary);
        }
        catch (Exception ex)
        {
            return Result<WatchlistSummaryDto>.Failure($"Error getting watchlist summary: {ex.Message}");
        }
    }

    public async Task<Result<WatchlistDto>> AddToWatchlistAsync(Guid userId, AddToWatchlistDto addDto)
    {
        try
        {
            var existingItem = await _context.Watchlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.MovieId == addDto.MovieId);

            if (existingItem != null)
                return Result<WatchlistDto>.Failure("Movie is already in watchlist");

            var watchlistItem = _mapper.Map<Watchlist>(addDto);
            watchlistItem.Id = Guid.NewGuid();
            watchlistItem.UserId = userId;
            watchlistItem.CreatedAt = DateTime.UtcNow;

            _context.Watchlists.Add(watchlistItem);
            await _context.SaveChangesAsync();

            var createdItem = await _context.Watchlists
                .Include(w => w.Movie)
                .ThenInclude(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
                .FirstOrDefaultAsync(w => w.Id == watchlistItem.Id);

            var watchlistDto = _mapper.Map<WatchlistDto>(createdItem);
            return Result<WatchlistDto>.Success(watchlistDto);
        }
        catch (Exception ex)
        {
            return Result<WatchlistDto>.Failure($"Error adding to watchlist: {ex.Message}");
        }
    }

    public async Task<Result<WatchlistDto>> UpdateWatchlistAsync(Guid userId, Guid movieId, UpdateWatchlistDto updateDto)
    {
        try
        {
            var watchlistItem = await _context.Watchlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.MovieId == movieId);

            if (watchlistItem == null)
                return Result<WatchlistDto>.Failure("Movie not found in watchlist");

            _mapper.Map(updateDto, watchlistItem);
            watchlistItem.UpdatedAt = DateTime.UtcNow;

            if (updateDto.IsWatched && !watchlistItem.WatchedDate.HasValue)
                watchlistItem.WatchedDate = DateTime.UtcNow;
            else if (!updateDto.IsWatched)
                watchlistItem.WatchedDate = null;

            await _context.SaveChangesAsync();

            var updatedItem = await _context.Watchlists
                .Include(w => w.Movie)
                .ThenInclude(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
                .FirstOrDefaultAsync(w => w.Id == watchlistItem.Id);

            var watchlistDto = _mapper.Map<WatchlistDto>(updatedItem);
            return Result<WatchlistDto>.Success(watchlistDto);
        }
        catch (Exception ex)
        {
            return Result<WatchlistDto>.Failure($"Error updating watchlist: {ex.Message}");
        }
    }

    public async Task<Result> RemoveFromWatchlistAsync(Guid userId, Guid movieId)
    {
        try
        {
            var watchlistItem = await _context.Watchlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.MovieId == movieId);

            if (watchlistItem == null)
                return Result.Failure("Movie not found in watchlist");

            _context.Watchlists.Remove(watchlistItem);
            await _context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error removing from watchlist: {ex.Message}");
        }
    }

    public async Task<Result<bool>> IsInWatchlistAsync(Guid userId, Guid movieId)
    {
        try
        {
            var exists = await _context.Watchlists
                .AnyAsync(w => w.UserId == userId && w.MovieId == movieId);

            return Result<bool>.Success(exists);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error checking watchlist: {ex.Message}");
        }
    }
}

