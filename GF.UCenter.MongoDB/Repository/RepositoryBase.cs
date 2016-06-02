
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace GF.UCenter.MongoDB.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Database;
    using Entity;
    

    public abstract class RepositoryBase<T> : IRepository<T>
        where T : EntityBase
    {
        private MongoCollection<T> collection;
        private static bool initialized;
        private object locker = new object();

        protected RepositoryBase(IMongoContext mongoContext)
        {
            collection = mongoContext.GetDatabase().GetCollection<T>(GetCollectionName());

            if (!initialized)
            {
                lock (locker)
                {
                    if (!initialized)
                    {
                        CreateIndexes(collection);
                        initialized = true;
                    }
                }
            }
        }

        protected virtual void CreateIndexes(MongoCollection<T> collection)
        {
        }

        protected abstract string GetCollectionName();

        private MongoCollection<T> Collection => collection;

        public virtual IQueryable<T> Context => Collection.AsQueryable<T>();

        public virtual T GetById(string id)
        {
            return Collection.FindOneByIdAs<T>(BsonValue.Create(id));
        }

        public virtual T Add(T entity)
        {
            try
            {
                Collection.Insert<T>(entity);
            }
            catch (Exception ex)  // TODO: Should catch mongodb exception
            {
                throw new DatabaseException(ex.Message);
            }

            return entity;
        }

        public virtual T Update(T entity)
        {
            IMongoQuery query = Query.And(Query.EQ("_id", entity.Id));
            var wrap = new BsonDocumentWrapper(entity);
            var document = new UpdateDocument(wrap.ToBsonDocument());
            WriteConcernResult res = Collection.Update(query, document);

            if (res.DocumentsAffected == 1)
            {
                return entity;
            }

            if (res.HasLastErrorMessage)
            {
                throw new Exception(res.LastErrorMessage);
            }

            bool isConcurrencyError = Collection.Find(Query.EQ("_id", entity.Id)).Any();
            if (isConcurrencyError)
            {
                throw new ConcurrencyException();
            }
            else
            {
                throw new ModelNotFoundException();
            }
        }

        public virtual void Delete(string id)
        {
            Collection.Remove(Query.And(Query.EQ("_id", BsonValue.Create(id))));
        }

        public virtual void Delete(ObjectId id)
        {
            Collection.Remove(Query.And(Query.EQ("_id", id)));
        }

        public void Delete(T entity)
        {
            Delete(((dynamic)entity).Id);
        }

        public virtual long Count()
        {
            return Collection.Count();
        }

        #region IQueryable<T>

        public IEnumerator<T> GetEnumerator()
        {
            return Context.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Context.GetEnumerator();
        }

        public Type ElementType
        {
            get { return Context.ElementType; }
        }

        public Expression Expression
        {
            get { return Context.Expression; }
        }

        public IQueryProvider Provider
        {
            get { return Context.Provider; }
        }

        #endregion
    }
}