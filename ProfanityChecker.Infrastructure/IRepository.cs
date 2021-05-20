using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProfanityChecker.Infrastructure
{
    public interface IRepository <TEntity> where TEntity : class, new()
    {
        IAsyncEnumerable<TEntity> GetAll();

        Task<TEntity> AddAsync(TEntity entity);

        Task<TEntity> UpdateAsync(TEntity entity);
    }
}