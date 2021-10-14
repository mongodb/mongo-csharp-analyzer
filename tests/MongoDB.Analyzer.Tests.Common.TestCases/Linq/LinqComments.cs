// Copyright 2021-present MongoDB Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Linq
{
    public sealed class LinqComments : TestCasesBase
    {
        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }, { \"$match\" : { \"Height\" : 180 } }])")]
        public void SingleLine_comment_before_and_after_expression()
        {
            // single line comment
            _ = GetMongoCollection().AsQueryable()
                .Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21)
                .Where(u => u.Height == 180);
            // single line comment
        }

        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }, { \"$match\" : { \"Height\" : 180 } }])")]
        public void SingleLine_comment_between_linq_invocations()
        {
            _ = GetMongoCollection()
                // single line comment
                .AsQueryable()
                // single line comment
                .Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21)
                // single line comment
                .Where(u => u.Height == 180);
        }

        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }, { \"$match\" : { \"Height\" : 180 } }])")]
        public void MultiLine_comment_before_and_after_expression()
        {
            /*
                multi line comment line 1
                multi line comment line 2
            */
            _ = GetMongoCollection().AsQueryable()
                .Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21)
                .Where(u => u.Height == 180);
            /*
               multi line comment line 1
               multi line comment line 2
            */
        }

        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }, { \"$match\" : { \"Height\" : 180 } }])")]
        public void MultiLine_comment_between_expressions()
        {
            _ = GetMongoCollection()
                /*
                    multi line comment line 1
                    multi line comment line 2
                */
                .AsQueryable()
                /*
                    multi line comment line 1
                    multi line comment line 2
                */
                .Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21)
                /*
                    multi line comment line 1
                    multi line comment line 2
                */
                .Where(u => u.Height == 180);
        }

        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\", \"Age\" : { \"$gt\" : 16, \"$lte\" : 21 } } }, { \"$match\" : { \"Height\" : 180 } }])")]
        public void Multiple_multilines_comments_inside_expressions()
        {
            _ = GetMongoCollection().AsQueryable()
                .Where(/* multi line comment */ u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21 /* multi line comment */)
                .Where(/* multi line comment */ u => u.Height == 180 /* multi line comment */);
        }
    }
}
