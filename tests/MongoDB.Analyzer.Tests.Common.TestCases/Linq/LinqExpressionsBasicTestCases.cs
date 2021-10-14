using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Linq
{
    public sealed class LinqExpressionsBasicTestCases : TestCasesBase
    {
        // MongoLinq2MQ
        // aggregate([{ "$match" : { "Name" : "Bob", "Age" : { "$gt" : 16, "$lte" : 21 } } }])
        public void SingleLine_expression()
        {
            _ = GetMongoCollection().AsQueryable().Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21);
        }

        // MongoLinq2MQ
        // aggregate([{ "$match" : { "Name" : "Bob", "Age" : { "$gt" : 16, "$lte" : 21 } } }, { "$match" : { "Height" : 180 } }])
        public void TwoLines_expression()
        {
            _ = GetMongoCollection().AsQueryable().
                Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21).
                Where(u => u.Height == 180);
        }

        // MongoLinq2MQ
        // aggregate([{ "$match" : { "Name" : "Bob", "Age" : { "$gt" : 16, "$lte" : 21 } } }, { "$match" : { "Height" : 180 } }, { "$project" : { "Scores" : "$Scores", "_id" : 0 } }])
        public void MultiLine_expression_1()
        {
            _ = GetMongoCollection().AsQueryable().
                Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21).
                Where(u => u.Height == 180).
                Select(u => u.Scores);
        }

        // MongoLinq2MQ
        // aggregate([{ "$match" : { "Name" : "Bob", "Age" : { "$gt" : 16, "$lte" : 21 } } }, { "$match" : { "Height" : { "$lt" : 180 } } }, { "$match" : { "LastName" : "Smith" } }, { "$project" : { "Height" : "$Height", "_id" : 0 } }])
        public void MultiLine_expression_2()
        {
            _ = GetMongoCollection().AsQueryable().
                Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21).
                Where(u => u.Height < 180).
                Where(u => u.LastName == "Smith").
                Select(u => u.Height);
        }
    }
}
