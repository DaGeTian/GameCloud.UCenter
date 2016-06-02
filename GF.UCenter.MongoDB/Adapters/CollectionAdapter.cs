using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GF.UCenter.MongoDB.Attributes;
using GF.UCenter.MongoDB.Entity;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GF.UCenter.MongoDB.Adapters
{
    [Export(typeof(ICollectionAdapter<>))]
    public class CollectionAdapter<TEntity> : ICollectionAdapter<TEntity>
        where TEntity : EntityBase
    {
        private readonly DatabaseContext context;
        private readonly IMongoCollection<TEntity> collection;
        private readonly string collectionName;

        [ImportingConstructor]
        private CollectionAdapter(DatabaseContext context)
        {
            this.context = context;
            this.collectionName = typeof(TEntity)
                .GetCustomAttribute<CollectionNameAttribute>()
                .CollectionName;

            this.collection = this.context.Database.GetCollection<TEntity>(this.collectionName, this.context.Settings.CollectionSettings);
        }

        IMongoCollection<TEntity> ICollectionAdapter<TEntity>.Collection
        {
            get
            {
                return this.collection;
            }
        }

        async Task ICollectionAdapter<TEntity>.CreateIfNotExistsAsync(CancellationToken token)
        {
            var filter = new BsonDocument("name", this.collectionName);
            var collections = await this.context.Database.ListCollectionsAsync(
                new ListCollectionsOptions()
                {
                    Filter = filter
                });

            if (!collections.Any())
            {
                await this.context.Database.CreateCollectionAsync(this.collectionName, null, token);
            }
        }

        Task<long> ICollectionAdapter<TEntity>.CountAsync(Expression<Func<TEntity, bool>> filter, CancellationToken token)
        {
            return ((ICollectionAdapter<TEntity>)this).CountAsync(filter, null, token);
        }

        Task<long> ICollectionAdapter<TEntity>.CountAsync(Expression<Func<TEntity, bool>> filter, CountOptions options, CancellationToken token)
        {
            return this.collection.CountAsync(filter, options, token);
        }

        Task ICollectionAdapter<TEntity>.DeleteAsync(string id, CancellationToken token)
        {
            return this.collection.DeleteOneAsync(e => e.Id == id, token);
        }

        Task ICollectionAdapter<TEntity>.DeleteAsync(TEntity entity, CancellationToken token)
        {
            return ((ICollectionAdapter<TEntity>)this).DeleteAsync(entity.Id, token);
        }

        async Task<IReadOnlyList<TEntity>> ICollectionAdapter<TEntity>.GetListAsync(Expression<Func<TEntity, bool>> filter, CancellationToken token)
        {
            return await this.collection.Find(filter).ToListAsync(token);
        }

        Task<TEntity> ICollectionAdapter<TEntity>.GetSingleAsync(Expression<Func<TEntity, bool>> filter, CancellationToken token)
        {
            return this.collection.Find(filter, null).FirstOrDefaultAsync(token);
        }

        Task<TEntity> ICollectionAdapter<TEntity>.GetSingleAsync(string id, CancellationToken token)
        {
            return ((ICollectionAdapter<TEntity>)this).GetSingleAsync(e => e.Id == id, token);
        }

        async Task<TEntity> ICollectionAdapter<TEntity>.InsertAsync(TEntity entity, CancellationToken token)
        {
            await this.collection.InsertOneAsync(entity, null, token);

            // todo: need retrieve the entity from server side?
            return entity;
        }

        async Task<TEntity> ICollectionAdapter<TEntity>.UpdateAsync(TEntity entity, CancellationToken token)
        {
            var replaceResult = await this.collection.ReplaceOneAsync(
                e => e.Id == entity.Id,
                entity,
                null,
                token);

            // todo: add some check logic here.
            // todo: reterive the entity from server side??
            return entity;
        }
    }
}
