namespace GF.UCenter.MongoDB.Repository
{
    using System.Linq;
    using Entity;

    public interface IRepository<T> : IQueryable<T>
        where T : EntityBase
    {
        IQueryable<T> Context { get; }

        T GetById(string id);

        T Add(T entity);

        T Update(T entity);

        void Delete(string id);

        void Delete(T entity);

        long Count();
    }
}