using System.Linq.Expressions;
using CineSocial.Application.Common.Models;
using CineSocial.Domain.Common;

namespace CineSocial.Application.Common.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    IQueryable<T> GetQueryable();
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<PagedResult<T>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<PagedResult<T>> GetPagedAsync(Expression<Func<T, bool>> predicate, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
}
