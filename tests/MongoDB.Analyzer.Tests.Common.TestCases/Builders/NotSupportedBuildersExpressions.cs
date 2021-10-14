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

using MongoDB.Analyzer.Tests.Common.DataModel;
using MongoDB.Driver;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Builders
{
    public sealed class NotSupportedBuildersExpressions : TestCasesBase
    {
        [NotSupportedBuilders("Unable to determine the serialization information for u => ArrayLength(u.IntArray).")]
        public void Array_of_predefined_type_array_members_access()
        {
            _ = Builders<SimpleTypesArraysHolder>.Filter.Gt(u => u.IntArray.Length, 1);
        }

        [NotSupportedBuilders("Unable to determine the serialization information for u => (u.SiblingsCount + 2).")]
        public void Binary_expression_in_field_getter()
        {
            _ = Builders<Person>.Filter.Eq(u => u.SiblingsCount + 2, 1);
        }

        [NotSupportedBuilders("Object reference not set to an instance of an object.")]
        public void Filter_AnyNin_null_argument()
        {
            _ = Builders<SimpleTypesArraysHolder>.Filter.AnyNin(t => t.ObjectArray, null);
        }
    }
}
