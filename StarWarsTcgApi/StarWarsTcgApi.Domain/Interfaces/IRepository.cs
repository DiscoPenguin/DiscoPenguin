using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StarWarsTcgApi.Domain.Interfaces
{
    public interface IRepository<TEntity, TId> where TEntity : class
    {
        Task<TEntity?> GetByIdAsync(TId id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TId> AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TId id);
    }
}