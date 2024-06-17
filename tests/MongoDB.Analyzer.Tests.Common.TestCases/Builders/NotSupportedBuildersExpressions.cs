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
        [NotSupportedBuilders("Unable to determine the serialization information for u => ArrayLength(u.IntArray).", DriverVersions.V2_18_OrLower)]
        [NotSupportedBuilders("Expression not supported: ArrayLength(u.IntArray).", DriverVersions.V2_19_OrGreater)]
        public void Array_of_predefined_type_array_members_access()
        {
            _ = Builders<SimpleTypesArraysHolder>.Filter.Gt(u => u.IntArray.Length, 1);
        }

        [NotSupportedBuilders("Unable to determine the serialization information for u => (u.SiblingsCount + 2).", DriverVersions.V2_18_OrLower)]
        [NotSupportedBuilders("Expression not supported: (u.SiblingsCount + 2).", DriverVersions.V2_19_OrGreater)]
        public void Binary_expression_in_field_getter()
        {
            _ = Builders<Person>.Filter.Eq(u => u.SiblingsCount + 2, 1);
        }

        [NotSupportedBuilders("Object reference not set to an instance of an object.")]
        public void Filter_AnyNin_null_argument()
        {
            _ = Builders<SimpleTypesArraysHolder>.Filter.AnyNin(t => t.ObjectArray, null);
        }

#if NET472
        [NotSupportedBuilders("Class System.Int32 cannot be assigned to Class MongoDB.Analyzer.Tests.Common.DataModel.GoldenApple.  Ensure that known types are derived from the mapped class.\r\nParameter name: type")]
        [NotSupportedBuilders("Class System.Type cannot be assigned to Class MongoDB.Analyzer.Tests.Common.DataModel.FujiApple.  Ensure that known types are derived from the mapped class.\r\nParameter name: type")]
        [NotSupportedBuilders("Class System.TimeSpan cannot be assigned to Class MongoDB.Analyzer.Tests.Common.DataModel.YellowApple.  Ensure that known types are derived from the mapped class.\r\nParameter name: type")]
#else
        [NotSupportedBuilders("Class System.Int32 cannot be assigned to Class MongoDB.Analyzer.Tests.Common.DataModel.GoldenApple.  Ensure that known types are derived from the mapped class. (Parameter 'type')")]
        [NotSupportedBuilders("Class System.Type cannot be assigned to Class MongoDB.Analyzer.Tests.Common.DataModel.FujiApple.  Ensure that known types are derived from the mapped class. (Parameter 'type')")]
        [NotSupportedBuilders("Class System.TimeSpan cannot be assigned to Class MongoDB.Analyzer.Tests.Common.DataModel.YellowApple.  Ensure that known types are derived from the mapped class. (Parameter 'type')")]
#endif
        public void Unsupported_type_for_bson_attribute_argument()
        {
            _ = Builders<GoldenApple>.Filter.Exists(g => g.GoldenAppleCost, false);
            _ = Builders<FujiApple>.Filter.Exists(f => f.FujiAppleCost, false);
            _ = Builders<YellowApple>.Filter.Exists(y => y.YellowAppleCost, false);
        }

        [NotSupportedBuilders("Unable to determine the serialization information for f => f.Quantity.", version: DriverVersions.V2_18_OrLower)]
        [NotSupportedBuilders("Expression not supported: f.Quantity.", version: DriverVersions.V2_19_OrGreater)]
        [NotSupportedBuilders("Unable to determine the serialization information for a => a._AppleID.", version: DriverVersions.V2_18_OrLower)]
        [NotSupportedBuilders("Expression not supported: a._AppleID.", version: DriverVersions.V2_19_OrGreater)]
        public void Warnings_due_to_bson_ignore()
        {
            _ = Builders<Fruit>.Update.Set(f => f.Quantity, 22);
            _ = Builders<Apple>.Update.Set(a => a._AppleID, "Apple ID");
        }
    }
}
