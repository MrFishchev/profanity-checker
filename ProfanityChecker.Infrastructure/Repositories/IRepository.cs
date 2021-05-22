using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProfanityChecker.Infrastructure
{
    public interface IRepository <TEntity> where TEntity : class, new()
    {
        Task<IEnumerable<TEntity>> GetAllAsync();

        Task<TEntity> AddAsync(TEntity entity);

        Task<TEntity> UpdateAsync(TEntity entity);

        Task DeleteAsync(TEntity entity);
    }
}