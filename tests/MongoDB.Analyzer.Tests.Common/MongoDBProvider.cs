using MongoDB.Driver;

namespace MongoDB.Analyzer.Tests.Common
{
    public static class MongoDBProvider
    {
        private static MongoClient s_mongoClient;

        public static IMongoDatabase MongoDatabase
        {
            get
            {
                if (s_mongoClient == null)
                {
                    s_mongoClient = new MongoClient(@"mongodb://localhost:27017");
                }

                return s_mongoClient.GetDatabase("TestDatabase");
            }
        }
    }
}
