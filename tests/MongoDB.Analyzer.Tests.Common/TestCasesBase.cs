using MongoDB.Analyzer.Tests.Common.DataModel;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq;

namespace MongoDB.Analyzer.Tests.Common
{
    public abstract class TestCasesBase
    {
        protected IMongoCollection<User> GetMongoCollection() =>
            MongoDBProvider.MongoDatabase.GetCollection<User>("TestCollection");

        protected IMongoQueryable<User> GetMongoQueryable() => GetMongoCollection().AsQueryable();

        protected T ReturnArgument<T>(T arg) => arg;
    }
}
