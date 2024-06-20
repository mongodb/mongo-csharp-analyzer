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

using System;
using MongoDB.Analyzer.Tests.Common.DataModel;
using MongoDB.Driver;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Builders
{
    public sealed class BuildersComplexExpressions : TestCasesBase
    {
        [NoDiagnostics]
        public void SortByFluentFunction()
        {
            Func<IFindFluent<User, User>, IOrderedFindFluent<User, User>> fluentFunction = x => x.SortBy(x => x.Age);
        }

        [NoDiagnostics]
        public void ThenByFluentFunction()
        {
            Func<IFindFluent<User, User>, IOrderedFindFluent<User, User>> fluentFunction = x => x.SortBy(x => x.Age).ThenBy(x => x.Height);
        }

        [NoDiagnostics]
        public void SortByAndThenByFluentFunctions()
        {
            Func<IFindFluent<User, User>, IFindFluent<User, User>> fluentFunction = x => x.Skip(10).Limit(12).SortBy(x => x.Age).ThenBy(x => x.Height);
        }

        [BuildersMQL("{ \"Age\" : -1, \"Name\" : 1, \"Address\" : 1 }")]
        public void SortFluentFunction()
        {
            Func<IFindFluent<User, User>, IFindFluent<User, User>> func = x => x.Sort(Builders<User>.Sort.Combine(
                    Builders<User>.Sort.Descending(u => u.Age),
                    Builders<User>.Sort.Ascending(u => u.Name),
                    Builders<User>.Sort.Ascending(u => u.Address)));
        }

        [BuildersMQL("{ \"Age\" : 1, \"Address\" : 0, \"Height\" : 1 }")]
        public void ProjectFluentFunction()
        {
            Func<IFindFluent<User, User>, IFindFluent<User, Bson.BsonDocument>> func = x => x.Project(Builders<User>.Projection.Include(u => u.Age).Exclude(u => u.Address).Include(u => u.Height));
        }

        [BuildersMQL("{ \"Age\" : -1, \"Name\" : 1, \"Address\" : 1 }")]
        [BuildersMQL("{ \"Age\" : 1, \"Address\" : 0, \"Height\" : 1 }")]
        public void SortAndProjectFluentFunction()
        {
            Func<IFindFluent<User, User>, IFindFluent<User, Bson.BsonDocument>> func = x => x.Sort(Builders<User>.Sort.Combine(
                    Builders<User>.Sort.Descending(u => u.Age),
                    Builders<User>.Sort.Ascending(u => u.Name),
                    Builders<User>.Sort.Ascending(u => u.Address)))
                .Project(Builders<User>.Projection.Include(u => u.Age)
                .Exclude(u => u.Address).Include(u => u.Height));
        }
    }
}
