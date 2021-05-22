using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Serilog;

// ReSharper disable once CheckNamespace
namespace ProfanityChecker.Infrastructure
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, new()
    {
        protected readonly DataContext DataContext;

        protected Repository(DataContext dataContext)
        {
            DataContext = dataContext;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken ct)
        {
            try
            {
                return await DataContext.Set<TEntity>().AsNoTracking().ToListAsync(ct);
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw new InvalidOperationException($"Unable to retrieve entities: {e.Message}");
            }
        }

        public async Task<TEntity> AddAsync(TEntity entity, CancellationToken ct)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "Parameter must be not null");

            try
            {
                await DataContext.AddAsync(entity, ct);
                return entity;
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw new InvalidOperationException($"Unable to save {nameof(entity)}: {e.Message}");
            }
        }

        public Task<TEntity> UpdateAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "Parameter must be not null");
            
            try
            {
                DataContext.Update(entity);
                return Task.FromResult(entity);
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw new InvalidOperationException($"Unable to update {nameof(entity)}: {e.Message}");
            }
        }

        public Task DeleteAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "Parameter must be not null");

            try
            {
                DataContext.Remove(entity);
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
                throw new InvalidOperationException($"Unable to delete {nameof(entity)}: {e.Message}");
            }
        }
    }
}