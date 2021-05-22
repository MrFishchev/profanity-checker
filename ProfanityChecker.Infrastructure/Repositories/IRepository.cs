using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ProfanityChecker.Infrastructure
{
    public interface IRepository <TEntity> where TEntity : class, new()
    {
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken ct = default);

        Task<TEntity> AddAsync(TEntity entity, CancellationToken ct = default);

        Task<TEntity> UpdateAsync(TEntity entity);

        Task DeleteAsync(TEntity entity);
    }
}