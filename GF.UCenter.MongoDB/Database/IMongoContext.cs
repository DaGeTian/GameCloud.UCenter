namespace GF.UCenter.MongoDB.Database
{
    using global::MongoDB.Driver;

    public interface IMongoContext
    {
        IMongoDatabase GetDatabase();
    }
}
