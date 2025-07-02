using CineSocial.Domain.Entities;

namespace CineSocial.Core.Shared.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByRefreshTokenAsync(string refreshToken);
    Task<bool> IsEmailExistsAsync(string email);
    Task<bool> IsUsernameExistsAsync(string username);
}