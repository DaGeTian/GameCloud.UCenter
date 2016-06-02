using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GF.UCenter.MongoDB.Entity;
using MongoDB.Driver;

namespace GF.UCenter.MongoDB.Adapters
{
    public interface ICollectionAdapter<TEntity> where TEntity : EntityBase
    {
        IMongoCollection<TEntity> Collection { get; }

        Task CreateIfNotExistsAsync(CancellationToken token);

        Task<IReadOnlyList<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> filter, CancellationToken token);

        Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> filter, CancellationToken token);

        Task<TEntity> GetSingleAsync(string id, CancellationToken token);

        Task<TEntity> InsertAsync(TEntity entity, CancellationToken token);

        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken token);

        Task DeleteAsync(TEntity entity, CancellationToken token);

        Task DeleteAsync(string id, CancellationToken token);

        Task<long> CountAsync(Expression<Func<TEntity, bool>> filter, CancellationToken token);

        Task<long> CountAsync(Expression<Func<TEntity, bool>> filter, CountOptions options, CancellationToken token);
    }
}
