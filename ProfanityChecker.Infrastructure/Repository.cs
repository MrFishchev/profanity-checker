using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ProfanityChecker.Infrastructure
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, new()
    {
        protected readonly DataContext DataContext;

        public Repository(DataContext dataContext)
        {
            DataContext = dataContext;
        }

        public IAsyncEnumerable<TEntity> GetAll()
        {
            try
            {
                return DataContext.Set<TEntity>().AsNoTracking().AsAsyncEnumerable();
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw new InvalidOperationException($"Unable to retrieve entities: {e.Message}");
            }
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "Parameter must be not null");

            try
            {
                await DataContext.AddAsync(entity);
                await DataContext.SaveChangesAsync();

                return entity;
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw new InvalidOperationException($"Unable to save {nameof(entity)}: {e.Message}");
            }
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "Parameter must be not null");
            
            try
            {
                DataContext.Update(entity);
                await DataContext.SaveChangesAsync();

                return entity;
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw new InvalidOperationException($"Unable to update {nameof(entity)}: {e.Message}");
            }
        }
    }
}