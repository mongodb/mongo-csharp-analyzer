using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Linq
{
    public sealed class LinqExpressionsWithCommentsTestCases : TestCasesBase
    {
        // MongoLinq2MQ
        // aggregate([{ "$match" : { "Name" : "Bob", "Age" : { "$gt" : 16, "$lte" : 21 } } }, { "$match" : { "Height" : 180 } }])
        public void SingleLine_comment_before_and_after_expression()
        {
            // single line comment
            _ = GetMongoCollection().AsQueryable().
                Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21).
                Where(u => u.Height == 180);
            // single line comment
        }

        // MongoLinq2MQ
        // aggregate([{ "$match" : { "Name" : "Bob", "Age" : { "$gt" : 16, "$lte" : 21 } } }, { "$match" : { "Height" : 180 } }])
        public void SingleLine_comment_between_linq_invocations()
        {
            _ = GetMongoCollection().
                // single line comment
                AsQueryable().
                // single line comment
                Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21).
                // single line comment
                Where(u => u.Height == 180);
        }

        // MongoLinq2MQ
        // aggregate([{ "$match" : { "Name" : "Bob", "Age" : { "$gt" : 16, "$lte" : 21 } } }, { "$match" : { "Height" : 180 } }])
        public void MultiLine_comment_before_and_after_expression()
        {
            /*
                multi line comment line 1
                multi line comment line 2
            */
            _ = GetMongoCollection().AsQueryable().
                Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21).
                Where(u => u.Height == 180);
            /*
               multi line comment line 1
               multi line comment line 2
            */
        }

        // MongoLinq2MQ
        // aggregate([{ "$match" : { "Name" : "Bob", "Age" : { "$gt" : 16, "$lte" : 21 } } }, { "$match" : { "Height" : 180 } }])
        public void MultiLine_comment_between_expressions()
        {
            _ = GetMongoCollection().
                /*
                    multi line comment line 1
                    multi line comment line 2
                */
                AsQueryable().
                /*
                    multi line comment line 1
                    multi line comment line 2
                */
                Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21).
                /*
                    multi line comment line 1
                    multi line comment line 2
                */
                Where(u => u.Height == 180);
        }

        // MongoLinq2MQ
        // aggregate([{ "$match" : { "Name" : "Bob", "Age" : { "$gt" : 16, "$lte" : 21 } } }, { "$match" : { "Height" : 180 } }])
        public void Multiple_multilines_comments_inside_expressions()
        {
            _ = GetMongoCollection().
                AsQueryable().
                Where(/* multi line comment */ u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21 /* multi line comment */).
                Where(/* multi line comment */ u => u.Height == 180 /* multi line comment */);
        }
    }
}
