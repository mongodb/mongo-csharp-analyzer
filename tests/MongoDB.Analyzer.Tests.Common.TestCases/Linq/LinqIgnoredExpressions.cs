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
using MongoDB.Analyzer.Tests.Common.DataModel;
using MongoDB.Driver.Linq;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Linq
{
    public sealed class LinqIgnoredExpressions : TestCasesBase
    {
        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\" } }])")]
        [MQL("aggregate([{ \"$match\" : { \"Name\" : \"Bob\" } }])")]
        public void Simple_valid_expression_as_baseline()
        {
            _ = GetMongoQueryable().Where(u => u.Name == "Bob");

            _ = from user in GetMongoQueryable()
                where user.Name == "Bob"
                select user;
        }

        [NoDiagnostics]
        public void Objects_comparison_expression_should_be_ignored()
        {
            var person = new Person();
            _ = GetMongoQueryable<Person>().Where(p => p == person);
        }

        [NoDiagnostics]
        public void Arrays_reference_should_be_ignored()
        {
            var names = new string[] { "Alice", "Bob" };
            _ = GetMongoQueryable<Person>().Where(p => p.Name == names[2]);
        }

        [NoDiagnostics]
        public void IMongoQueryable_with_generic_type_should_be_ignored<T>()
        {
            _ = GetMongoQueryable<T>().Where(t => t.GetHashCode() == 1);
        }

        [NoDiagnostics]
        public void Internal_members_should_be_ignored()
        {
            _ = GetMongoQueryable<MixedDataMembers>().Where(u => u.InternalPropertyInt == 1);
            _ = GetMongoQueryable<MixedDataMembers>().Where(u => u.ProtectedInternalPropertyString == "str");
        }

        [NoDiagnostics]
        public void Query_syntax<T>()
        {
            var person = new Person();
            var names = new string[] { "Alice", "Bob" };

            _ = from personObj in GetMongoQueryable<Person>()
                where personObj == person
                select personObj;

            _ = from personObj in GetMongoQueryable<Person>()
                where personObj.Name == names[2]
                select personObj;

            _ = from item in GetMongoQueryable<T>()
                where item.GetHashCode() == 1
                select item;

            _ = from mixedDataMember in GetMongoQueryable<MixedDataMembers>()
                where mixedDataMember.InternalPropertyInt == 1
                select mixedDataMember;

            _ = from mixedDataMember in GetMongoQueryable<MixedDataMembers>()
                where mixedDataMember.ProtectedInternalPropertyString == "str"
                select mixedDataMember;
        }

        [NoDiagnostics]
        public void IQueryable_extensions_in_middle_or_prefix_should_be_ignored()
        {
            _ = GetMongoQueryable()
                .Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21)
                .Where(u => u.LastName.Length < 10)
                .ApplyPagingIQueryable(1, 0)
                .Where(u => u.LastName == "Smith");

            _ = GetMongoQueryable()
                .Where(u => u.Name == "Bob" && u.Age > 16 && u.Age <= 21)
                .Where(u => u.LastName.Length < 10)
                .ApplyPaging(1, 0)
                .ApplyPagingIQueryable(1, 0)
                .Where(u => u.LastName == "Smith");

            _ = GetMongoQueryable()
                .ApplyPagingIQueryable(1, 0)
                .Where(u => u.LastName == "Smith");

            _ = GetMongoQueryable()
                .ApplyPaging(1, 0)
                .ApplyPagingIQueryable(1, 0)
                .Where(u => u.LastName == "Smith");
        }
    }
}
