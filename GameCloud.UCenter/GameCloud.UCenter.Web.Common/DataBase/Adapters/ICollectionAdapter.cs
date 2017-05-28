using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace GameCloud.Database.Adapters
{
    /// <summary>
    /// Provide an interface for MongoDB collection adapter.
    /// </summary>
    /// <typeparam name="TEntity">Indicating the document type.</typeparam>
    public interface ICollectionAdapter<TEntity> where TEntity : EntityBase
    {
        /// <summary>
        /// Gets the collection.
        /// </summary>
        IMongoCollection<TEntity> Collection { get; }

        /// <summary>
        /// Get document entity list.
        /// </summary>
        /// <param name="filter">Indicating the filter expression.</param>
        /// <param name="options">Indication the options.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async task</returns>
        Task<IReadOnlyList<TEntity>> GetListAsync(
            Expression<Func<TEntity, bool>> filter,
            FindOptions options,
            CancellationToken token);

        /// <summary>
        /// Get single document entity.
        /// </summary>
        /// <param name="filter">Indicating the filter expression.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async task</returns>
        Task<TEntity> GetSingleAsync(
            Expression<Func<TEntity, bool>> filter,
            CancellationToken token);

        /// <summary>
        /// Insert document entity.
        /// </summary>
        /// <param name="entity">Indicating the document entity.</param>
        /// <param name="options">Indication the options.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async task</returns>
        Task<TEntity> InsertAsync(
            TEntity entity,
            InsertOneOptions options,
            CancellationToken token);

        /// <summary>
        /// Insert document entity.
        /// </summary>
        /// <param name="entities">Indicating the document entities.</param>
        /// <param name="options">Indication the options.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async task</returns>
        Task<IReadOnlyList<TEntity>> InsertManyAsync(
            IReadOnlyList<TEntity> entities,
            InsertManyOptions options,
            CancellationToken token);

        /// <summary>
        /// Update document entity.
        /// </summary>
        /// <param name="entity">Indicating the document entity.</param>
        /// <param name="filter">Indication the filter.</param>
        /// <param name="update">Indication the update.</param>
        /// <param name="options">Indication the options.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async task</returns>
        Task<UpdateResult> UpdateOneAsync(
            TEntity entity,
            FilterDefinition<TEntity> filter,
            UpdateDefinition<TEntity> update,
            UpdateOptions options,
            CancellationToken token);

        Task<TEntity> ReplaceOneAsync(
            TEntity entity,
            UpdateOptions options,
            CancellationToken token);

        /// <summary>
        /// Delete document entity by filter expression.
        /// </summary>
        /// <param name="filter">Indicating the filter expression.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async task</returns>
        Task DeleteAsync(Expression<Func<TEntity, bool>> filter, CancellationToken token);

        /// <summary>
        /// Get document entity count by filter expression.
        /// </summary>
        /// <param name="filter">Indicating the filter expression.</param>
        /// <param name="options">Indication the options.</param>
        /// <param name="token">Indicating the cancellation token.</param>
        /// <returns>Async task</returns>
        Task<long> CountAsync(
            Expression<Func<TEntity, bool>> filter,
            CountOptions options,
            CancellationToken token);

        Task<string> CreateIndexIfNotExistAsync(
            IndexKeysDefinition<TEntity> keys,
            CreateIndexOptions options,
            CancellationToken token);
    }
}
