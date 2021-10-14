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
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Builders
{
    public sealed class BuildersIgnoredExpressions : TestCasesBase
    {
        [BuildersMQL("{ \"Address.City\" : \"Vienna\" }")]
        public void Simple_valid_expression_as_baseline()
        {
            var person = new Person();
            _ = Builders<Person>.Filter.Eq(t => t.Address.City, "Vienna");
        }

        [NoDiagnostics]
        public void Objects_comparison_expression_should_be_ignored()
        {
            var person = new Person();
            _ = Builders<Person>.Filter.Eq(t => t.Address, person.Address);
        }

        [NoDiagnostics]
        public void Arrays_reference_should_be_ignored()
        {
            var names = new string[] { "Alice", "Bob" };
            _ = Builders<Person>.Filter.Eq(t => t.Name, names[2]);
        }

        [NoDiagnostics]
        public void Bson_binary_data_param_should_be_ignored()
        {
            _ = Builders<BsonDocument>.Filter.Eq("_id", new BsonBinaryData(Guid.Empty, GuidRepresentation.Standard));
        }

        [NoDiagnostics]
        public void In_should_be_ignored()
        {
            var arr = new[] { "Lion", "Erich" };
            _ = Builders<Person>.Filter.In(c => c.Name, arr);
        }

        [NoDiagnostics]
        public void Generic_builder_expressions_should_be_ignored<T>()
        {
            _ = Builders<T>.Filter.Eq("field", "value");
        }

        [NoDiagnostics]
        public void Internal_members_should_be_ignored()
        {
            _ = Builders<MixedDataMembers>.Filter.Eq(u => u.InternalPropertyInt, 1);
            _ = Builders<MixedDataMembers>.Filter.Eq(u => u.ProtectedInternalPropertyString, "str");
        }
    }
}
