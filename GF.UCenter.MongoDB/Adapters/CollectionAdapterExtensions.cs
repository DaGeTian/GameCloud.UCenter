namespace GF.UCenter.MongoDB.Adapters
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Entity;
    using global::MongoDB.Driver;

    public static class CollectionAdapterExtensions
    {
        public static Task DeleteAsync<TEntity>(this ICollectionAdapter<TEntity> adapter, TEntity entity, CancellationToken token)
            where TEntity : EntityBase
        {
            return adapter.DeleteAsync(entity.Id, token);
        }

        public static Task DeleteAsync<TEntity>(this ICollectionAdapter<TEntity> adapter, string id, CancellationToken token)
            where TEntity : EntityBase
        {
            return adapter.DeleteAsync(e => e.Id == id, token);
        }

        public static Task<TEntity> GetSingleAsync<TEntity>(this ICollectionAdapter<TEntity> adapter, string id, CancellationToken token)
            where TEntity : EntityBase
        {
            return adapter.GetSingleAsync(e => e.Id == id, token);
        }

        public static Task<long> CountAsync<TEntity>(this ICollectionAdapter<TEntity> adapter, Expression<Func<TEntity, bool>> filter, CancellationToken token)
            where TEntity : EntityBase
        {
            return adapter.CountAsync(filter, null, token);
        }

        public static Task<TEntity> InsertAsync<TEntity>(this ICollectionAdapter<TEntity> adapter, TEntity entity, CancellationToken token)
            where TEntity : EntityBase
        {
            return adapter.InsertAsync(entity, null, token);
        }

        public static Task<TEntity> UpdateAsync<TEntity>(this ICollectionAdapter<TEntity> adapter, TEntity entity, CancellationToken token)
           where TEntity : EntityBase
        {
            return adapter.UpdateAsync(entity, null, token);
        }

        public static Task<TEntity> UpsertAsync<TEntity>(this ICollectionAdapter<TEntity> adapter, TEntity entity, CancellationToken token)
           where TEntity : EntityBase
        {
            return adapter.UpdateAsync(
                entity,
                new UpdateOptions { IsUpsert = true },
                token);
        }

        public static Task<IReadOnlyList<TEntity>> GetListAsync<TEntity>(this ICollectionAdapter<TEntity> adapter, Expression<Func<TEntity, bool>> filter, CancellationToken token)
           where TEntity : EntityBase
        {
            return adapter.GetListAsync(filter, null, token);
        }
    }
}
