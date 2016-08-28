using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using GameCloud.Database.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GameCloud.Database.Adapters
{
    /// <summary>
    /// Provide an adapter for MongoDBA collection.
    /// </summary>
    /// <typeparam name="TEntity">Indicating the document type.</typeparam>
    [Export(typeof(ICollectionAdapter<>))]
    public class CollectionAdapter<TEntity> : ICollectionAdapter<TEntity>
        where TEntity : EntityBase
    {
        private readonly IMongoCollection<TEntity> collection;
        private readonly string collectionName;
        private readonly DatabaseContext context;

        public CollectionAdapter(DatabaseContext context, string collectionName)
        {
            this.context = context;
            this.collectionName = collectionName;
            this.collection = this.context.Database.GetCollection<TEntity>(this.collectionName, this.context.Settings.CollectionSettings);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionAdapter{TEntity}" /> class.
        /// </summary>
        /// <param name="context">Indicating the database context.</param>
        /// <param name="collectionName">Indicating the database collection</param>
        [ImportingConstructor]
        private CollectionAdapter(DatabaseContext context)
            : this(context, typeof(TEntity).GetCustomAttribute<CollectionNameAttribute>().CollectionName)
        {
        }

        IMongoCollection<TEntity> ICollectionAdapter<TEntity>.Collection
        {
            get
            {
                return this.collection;
            }
        }

        Task<long> ICollectionAdapter<TEntity>.CountAsync(
            Expression<Func<TEntity, bool>> filter,
            CountOptions options,
            CancellationToken token)
        {
            if (filter == null)
            {
                return this.collection.CountAsync(new BsonDocument(), options, token);
            }

            return this.collection.CountAsync(filter, options, token);
        }

        async Task ICollectionAdapter<TEntity>.DeleteAsync(
            Expression<Func<TEntity, bool>> filter,
            CancellationToken token)
        {
            var result = await this.collection.DeleteOneAsync(filter, token);
            //// todo: check the delete result.
        }

        async Task<IReadOnlyList<TEntity>> ICollectionAdapter<TEntity>.GetListAsync(
            Expression<Func<TEntity, bool>> filter,
            FindOptions options,
            CancellationToken token)
        {
            return await this.collection.Find(filter, options).ToListAsync(token);
        }

        Task<TEntity> ICollectionAdapter<TEntity>.GetSingleAsync(
            Expression<Func<TEntity, bool>> filter,
            CancellationToken token)
        {
            return this.collection.Find(filter, null).FirstOrDefaultAsync(token);
        }

        async Task<TEntity> ICollectionAdapter<TEntity>.InsertAsync(
            TEntity entity,
            InsertOneOptions options,
            CancellationToken token)
        {
            await this.collection.InsertOneAsync(entity, options, token);

            // todo: need retrieve the entity from server side?
            return entity;
        }

        async Task<IReadOnlyList<TEntity>> ICollectionAdapter<TEntity>.InsertManyAsync(
            IReadOnlyList<TEntity> entities,
            InsertManyOptions options,
            CancellationToken token)
        {
            await this.collection.InsertManyAsync(entities, options, token);

            return entities;
        }

        async Task<TEntity> ICollectionAdapter<TEntity>.ReplaceOneAsync(
            TEntity entity,
            UpdateOptions options,
            CancellationToken token)
        {
            var replaceResult = await this.collection.ReplaceOneAsync(
                e => e.Id == entity.Id,
                entity,
                options,
                token);

            // todo: add some check logic here.
            // todo: reterive the entity from server side??
            return entity;
        }

        Task<UpdateResult> ICollectionAdapter<TEntity>.UpdateOneAsync(
            TEntity entity,
            FilterDefinition<TEntity> filter,
            UpdateDefinition<TEntity> update,
            UpdateOptions options,
            CancellationToken token)
        {
            return this.collection.UpdateOneAsync(filter, update, options, token);
        }

        async Task<string> ICollectionAdapter<TEntity>.CreateIndexIfNotExistAsync(
            IndexKeysDefinition<TEntity> keys,
            CreateIndexOptions options,
            CancellationToken token)
        {
            using (var cursor = await this.collection.Indexes.ListAsync(token))
            {
                var indexes = await cursor.ToListAsync(token);
                var index = indexes.FirstOrDefault(i => i["name"] == options.Name);
                if (index == null)
                {
                    return await this.collection.Indexes.CreateOneAsync(keys, options, token);
                }
            }

            return null;
        }
    }
}
