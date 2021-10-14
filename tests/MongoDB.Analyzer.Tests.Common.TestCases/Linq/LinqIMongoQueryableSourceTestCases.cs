using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Linq
{
    public sealed class LinqIMongoQueryableSourceTestCases : TestCasesBase
    {
        // MongoLinq2MQ
        // aggregate([{ "$match" : { "Name" : "Bob", "Age" : { "$gt" : 18, "$lte" : 21 } } }, { "$match" : { "LastName" : /^.{0,9}$/s } }, { "$project" : { "Name" : "$Name", "_id" : 0 } }])
        public void Queryable_from_collection_variable()
        {
            var collection = GetMongoCollection();

            var queryable = collection.AsQueryable().
                Where(u => u.Name == "Bob" && u.Age > 18 && u.Age <= 21).
                Where(u => u.LastName.Length < 10).
                Select(u => u.Name);
        }

        // MongoLinq2MQL
        // aggregate([{ "$match" : { "Name" : "Bob", "Age" : { "$gt" : 18, "$lte" : 21 } } }, { "$match" : { "LastName" : /^.{0,9}$/s } }, { "$project" : { "Name" : "$Name", "_id" : 0 } }])
        public void Queryable_from_collection_method()
        {
            var queryable = GetMongoCollection().AsQueryable().
                Where(u => u.Name == "Bob" && u.Age > 18 && u.Age <= 21).
                Where(u => u.LastName.Length < 10).
                Select(u => u.Name);
        }

        // MongoLinq2MQL
        // aggregate([{ "$match" : { "Name" : "Bob", "Age" : { "$gt" : 18, "$lte" : 21 } } }, { "$match" : { "LastName" : /^.{0,9}$/s } }, { "$project" : { "Name" : "$Name", "_id" : 0 } }])
        public void Queryable_from_method()
        {
            var queryable = GetMongoQueryable().
                Where(u => u.Name == "Bob" && u.Age > 18 && u.Age <= 21).
                Where(u => u.LastName.Length < 10).
                Select(u => u.Name);
        }

        // MongoLinq2MQL
        // aggregate([{ "$match" : { "Name" : "Bob", "Age" : { "$gt" : 18, "$lte" : 21 } } }, { "$match" : { "LastName" : /^.{0,9}$/s } }, { "$project" : { "Name" : "$Name", "_id" : 0 } }])
        public void Queryable_from_method_nested_1()
        {
            var queryable = GetThis().
                GetMongoQueryable().
                Where(u => u.Name == "Bob" && u.Age > 18 && u.Age <= 21).
                Where(u => u.LastName.Length < 10).
                Select(u => u.Name);
        }

        // MongoLinq2MQL
        // aggregate([{ "$match" : { "Name" : "Bob1", "Age" : { "$gt" : 18, "$lte" : 21 } } }, { "$match" : { "LastName" : /^.{0,11}$/s } }, { "$project" : { "Name" : "$Name", "_id" : 0 } }])
        public void Queryable_from_method_nested_2()
        {
            var queryable = GetThis().
                ReturnArgument(GetMongoCollection()).
                AsQueryable().
                Where(u => u.Name == "Bob1" && u.Age > 18 && u.Age <= 21).
                Where(u => u.LastName.Length < 12).
                Select(u => u.Name);
        }

        // MongoLinq2MQL
        // aggregate([{ "$match" : { "Name" : "Bob2", "Age" : { "$gt" : 18, "$lte" : 21 } } }, { "$match" : { "LastName" : /^.{0,10}$/s } }, { "$project" : { "Name" : "$Name", "_id" : 0 } }])
        public void Queryable_from_method_nested_3()
        {
            var queryable = ReturnArgument(ReturnArgument(GetThis().
                GetThis().
                ReturnArgument(GetMongoCollection())).
                AsQueryable()).
                Where(u => u.Name == "Bob2" && u.Age > 18 && u.Age <= 21).
                Where(u => u.LastName.Length < 11).
                Select(u => u.Name);
        }

        // MongoLinq2MQL
        // aggregate([{ "$match" : { "Name" : "Bob", "Age" : { "$gt" : 18, "$lte" : 21 } } }, { "$match" : { "LastName" : /^.{0,9}$/s } }, { "$project" : { "Name" : "$Name", "_id" : 0 } }])
        public void DirectQueryableAccess()
        {
            var collection = GetMongoCollection();
            var queryable = collection.AsQueryable();

            _ = queryable.
                Where(u => u.Name == "Bob" && u.Age > 18 && u.Age <= 21).
                Where(u => u.LastName.Length < 10).
                Select(u => u.Name);
        }

        private LinqIMongoQueryableSourceTestCases GetThis() => this;
    }
}
