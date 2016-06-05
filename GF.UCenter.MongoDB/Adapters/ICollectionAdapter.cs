namespace GF.UCenter.MongoDB.Adapters
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Entity;
    using global::MongoDB.Driver;

    public interface ICollectionAdapter<TEntity> where TEntity : EntityBase
    {
        IMongoCollection<TEntity> Collection { get; }

        Task CreateIfNotExistsAsync(CancellationToken token);

        Task<IReadOnlyList<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> filter, FindOptions options, CancellationToken token);

        Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> filter, CancellationToken token);

        Task<TEntity> InsertAsync(TEntity entity, InsertOneOptions options, CancellationToken token);

        Task<TEntity> UpdateAsync(TEntity entity, UpdateOptions options, CancellationToken token);

        Task DeleteAsync(Expression<Func<TEntity, bool>> filter, CancellationToken token);

        Task<long> CountAsync(Expression<Func<TEntity, bool>> filter, CountOptions options, CancellationToken token);
    }
}
