using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace GameCloud.Database.Adapters
{
    /// <summary>
    /// Provide a class for collection adapter extension functions.
    /// </summary>
    public static class CollectionAdapterExtensions
    {
        /// <summary>
        /// Delete document.
        /// </summary>
        /// <typeparam name="TEntity">Indicating the document type.</typeparam>
        /// <param name="adapter">Indicating the document adapter.</param>
        /// <param name="entity">Indicating the document entity.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async task.</returns>
        public static Task DeleteAsync<TEntity>(this ICollectionAdapter<TEntity> adapter, TEntity entity, CancellationToken token)
            where TEntity : EntityBase
        {
            return adapter.DeleteAsync(entity.Id, token);
        }

        /// <summary>
        /// Delete document by id.
        /// </summary>
        /// <typeparam name="TEntity">Indicating the document type.</typeparam>
        /// <param name="adapter">Indicating the document adapter.</param>
        /// <param name="id">Indicating the document id.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async task.</returns>
        /// <returns></returns>
        public static Task DeleteAsync<TEntity>(this ICollectionAdapter<TEntity> adapter, string id, CancellationToken token)
            where TEntity : EntityBase
        {
            return adapter.DeleteAsync(e => e.Id == id, token);
        }

        /// <summary>
        /// Get single document by id.
        /// </summary>
        /// <typeparam name="TEntity">Indicating the document type.</typeparam>
        /// <param name="adapter">Indicating the document adapter.</param>
        /// <param name="id">Indicating the document id.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async return document entity.</returns>
        public static Task<TEntity> GetSingleAsync<TEntity>(this ICollectionAdapter<TEntity> adapter, string id, CancellationToken token)
            where TEntity : EntityBase
        {
            return adapter.GetSingleAsync(e => e.Id == id, token);
        }

        /// <summary>
        /// Get document count by filter.
        /// </summary>
        /// <typeparam name="TEntity">Indicating the document type.</typeparam>
        /// <param name="adapter">Indicating the document adapter.</param>
        /// <param name="filter">Indicating the filter expression.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async return count.</returns>
        public static Task<long> CountAsync<TEntity>(this ICollectionAdapter<TEntity> adapter, Expression<Func<TEntity, bool>> filter, CancellationToken token)
            where TEntity : EntityBase
        {
            return adapter.CountAsync(filter, null, token);
        }

        /// <summary>
        /// Insert document entity.
        /// </summary>
        /// <typeparam name="TEntity">Indicating the document type.</typeparam>
        /// <param name="adapter">Indicating the document adapter.</param>
        /// <param name="entity">Indicating the document entity.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async task.</returns>
        public static Task<TEntity> InsertAsync<TEntity>(this ICollectionAdapter<TEntity> adapter, TEntity entity, CancellationToken token)
            where TEntity : EntityBase
        {
            return adapter.InsertAsync(entity, null, token);
        }

        /// <summary>
        /// Update document entity.
        /// </summary>
        /// <typeparam name="TEntity">Indicating the document type.</typeparam>
        /// <param name="adapter">Indicating the document adapter.</param>
        /// <param name="entity">Indicating the document entity.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async task.</returns>
        public static Task<TEntity> UpdateAsync<TEntity>(this ICollectionAdapter<TEntity> adapter, TEntity entity, CancellationToken token)
           where TEntity : EntityBase
        {
            return adapter.UpdateAsync(entity, null, token);
        }

        /// <summary>
        /// Update or insert document entity.
        /// </summary>
        /// <typeparam name="TEntity">Indicating the document type.</typeparam>
        /// <param name="adapter">Indicating the document adapter.</param>
        /// <param name="entity">Indicating the document entity.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async task.</returns>
        public static Task<TEntity> UpsertAsync<TEntity>(this ICollectionAdapter<TEntity> adapter, TEntity entity, CancellationToken token)
           where TEntity : EntityBase
        {
            return adapter.UpdateAsync(
                entity,
                new UpdateOptions { IsUpsert = true },
                token);
        }

        /// <summary>
        /// Get document entity list by filter.
        /// </summary>
        /// <typeparam name="TEntity">Indicating the document type.</typeparam>
        /// <param name="adapter">Indicating the document adapter.</param>
        /// <param name="filter">Indicating the filter expression.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async return list.</returns>
        public static Task<IReadOnlyList<TEntity>> GetListAsync<TEntity>(this ICollectionAdapter<TEntity> adapter, Expression<Func<TEntity, bool>> filter, CancellationToken token)
           where TEntity : EntityBase
        {
            return adapter.GetListAsync(filter, null, token);
        }
    }
}
