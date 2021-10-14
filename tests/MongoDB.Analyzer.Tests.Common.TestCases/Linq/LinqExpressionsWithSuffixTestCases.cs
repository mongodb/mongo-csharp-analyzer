using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Linq
{
    public sealed class LinqExpressionsWithSuffixTestCases : TestCasesBase
    {
        // MongoLinq2MQ
        // aggregate([{ "$match" : { "Name" : "Bob", "Age" : { "$gt" : 16, "$lte" : 21 } } }, { "$match" : { "LastName" : /^.{0,9}$/s } }, { "$project" : { "Name" : "$Name", "_id" : 0 } }])
        public void SingleMethod_suffix()
        {
            var collection = GetMongoCollection();

            _ = collection.AsQueryable().
                Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21).
                Where(u => u.LastName.Length < 10).
                Select(u => u.Name).
                ToCursor();
        }

        // MongoLinq2MQ
        // aggregate([{ "$match" : { "Name" : "Bob", "Age" : { "$gt" : 16, "$lte" : 21 } } }, { "$match" : { "LastName" : /^.{0,9}$/s } }, { "$project" : { "Name" : "$Name", "_id" : 0 } }])
        public void TwoMethods_suffix()
        {
            var collection = GetMongoCollection();

            _ = collection.AsQueryable().
                Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21).
                Where(u => u.LastName.Length < 10).
                Select(u => u.Name).
                ToCursor().
                ToList();
        }

        // MongoLinq2MQ
        // aggregate([{ "$match" : { "Name" : "Bob", "Age" : { "$gt" : 16, "$lte" : 21 } } }, { "$match" : { "LastName" : /^.{0,9}$/s } }, { "$project" : { "Name" : "$Name", "_id" : 0 } }])
        public void MultipleMethods_suffix()
        {
            var collection = GetMongoCollection();

            _ = collection.AsQueryable().
                Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21).
                Where(u => u.LastName.Length < 10).
                Select(u => u.Name).
                ToCursor().
                ToList().
                ToArray().
                ToList();
        }
    }
}
