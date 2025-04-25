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
    public sealed class NotSupportedLinqExpressions : TestCasesBase
    {
        [InvalidLinq("Method referencing lambda parameter is not supported LINQ expression.")]
        public void Query_syntax()
        {
            _ = from person in GetMongoQueryable<Person>()
                where ReturnArgument(ReturnArgument(ReturnArgument(this).ReturnArgument(person.Vehicle.VehicleType))).Type == VehicleTypeEnum.Bus
                select person;
        }

        [InvalidLinq("Expression not supported: 10 in (o.BsonDocument.ElementCount == 10) because it was not possible to determine how to serialize the constant.", DriverVersions.V3_1_AndLower)]
        [InvalidLinq("Expression not supported: o.BsonDocument.ElementCount.", DriverVersions.V3_2_OrGreater)]
        public void Unsupported_bson_types()
        {
            _ = GetMongoQueryable<ClassWithBsonTypes>().Where(o => o.BsonDocument.ElementCount == 10);
        }

        [InvalidLinq("MongoDB.Bson.Serialization.Serializers.ByteArraySerializer must implement IBsonArraySerializer to be used with LINQ.")]
        public void Unsupported_byte_array_item_access()
        {
            _ = GetMongoQueryable<SimpleTypesArraysHolder>()
                .Where(u => u.ByteArray[0] == 12);
        }

        [InvalidLinq("Method referencing lambda parameter is not supported LINQ expression.")]
        public void Unsupported_external_method_referencing_lambda_variable()
        {
            _ = GetMongoQueryable()
                .Where(u => ReturnArgument(u).Name == "Alice");
        }

        [InvalidLinq("Method referencing lambda parameter is not supported LINQ expression.")]
        public void Unsupported_external_method_referencing_lambda_variable_2()
        {
            _ = GetMongoQueryable()
                .Where(u => ReturnArgument(u.Address) == "Alice");
        }

        [InvalidLinq("Method referencing lambda parameter is not supported LINQ expression.")]
        public void Unsupported_external_method_referencing_lambda_variable_3()
        {
            _ = GetMongoQueryable()
                .Where(u => u.Address == ReturnArgument(u).Address);
        }

        [InvalidLinq("Expression not supported: u.Matrix2.Get(1, 1).")]
        public void Unsupported_multidimensional_array_dimension1()
        {
            _ = GetMongoQueryable<MultiDimentionalArrayHolder>()
                .Where(u => u.Matrix2[1, 1] == 1);
        }

        [InvalidLinq("Expression not supported: u.Matrix3.Get(1, 1, 1).")]
        public void Unsupported_multidimensional_array_dimension2()
        {
            _ = GetMongoQueryable<MultiDimentionalArrayHolder>()
                .Where(u => u.Matrix3[1, 1, 1] == 1);
        }

        [InvalidLinq("Method referencing lambda parameter is not supported LINQ expression.")]
        public void Unsupported_nested_external_method_referencing_lambda_variable()
        {
            _ = GetMongoQueryable<Person>()
                .Where(u => ReturnArgument(this).ReturnArgument(u.Vehicle.VehicleType.Type) == VehicleTypeEnum.Bus);
        }

        [InvalidLinq("Method referencing lambda parameter is not supported LINQ expression.")]
        public void Unsupported_nested_external_method_referencing_lambda_variable_2()
        {
            _ = GetMongoQueryable<Person>()
                .Where(u => ReturnArgument(ReturnArgument(ReturnArgument(this).ReturnArgument(u.Vehicle.VehicleType))).Type == VehicleTypeEnum.Bus);
        }

        [InvalidLinq("Expression not supported: u.GetHashCode().")]
        public void Unsupported_object_method_invocation()
        {
            _ = GetMongoQueryable()
                .Where(u => u.GetHashCode() == 1);
        }

        [InvalidLinq("Expression not supported: u.Name.LastIndexOf(1).")]
        public void Unsupported_string_method_LastIndexOf()
        {
            _ = GetMongoQueryable()
               .Where(u => u.Name.LastIndexOf('1') == 1);
        }

#if NET472
        [InvalidLinq("Class System.Int32 cannot be assigned to Class MongoDB.Analyzer.Tests.Common.DataModel.GoldenApple.  Ensure that known types are derived from the mapped class.\r\nParameter name: type")]
        [InvalidLinq("Class System.Type cannot be assigned to Class MongoDB.Analyzer.Tests.Common.DataModel.FujiApple.  Ensure that known types are derived from the mapped class.\r\nParameter name: type")]
        [InvalidLinq("Class System.TimeSpan cannot be assigned to Class MongoDB.Analyzer.Tests.Common.DataModel.YellowApple.  Ensure that known types are derived from the mapped class.\r\nParameter name: type")]
#else
        [InvalidLinq("Class System.Int32 cannot be assigned to Class MongoDB.Analyzer.Tests.Common.DataModel.GoldenApple.  Ensure that known types are derived from the mapped class. (Parameter 'type')")]
        [InvalidLinq("Class System.Type cannot be assigned to Class MongoDB.Analyzer.Tests.Common.DataModel.FujiApple.  Ensure that known types are derived from the mapped class. (Parameter 'type')")]
        [InvalidLinq("Class System.TimeSpan cannot be assigned to Class MongoDB.Analyzer.Tests.Common.DataModel.YellowApple.  Ensure that known types are derived from the mapped class. (Parameter 'type')")]
#endif
        public void Unsupported_type_for_bson_attribute_argument()
        {
            _ = GetMongoQueryable<GoldenApple>()
                .Where(g => g.GoldenAppleCost == 22)
                .Select(g => g);

            _ = GetMongoQueryable<FujiApple>()
                .Where(f => f.FujiAppleCost == 22)
                .Select(f => f);

            _ = GetMongoQueryable<YellowApple>()
                .Where(y => y.YellowAppleCost == 22)
                .Select(y => y);
        }

        [InvalidLinq("Serializer for MongoDB.Analyzer.Tests.Common.DataModel.Fruit does not have a member named Quantity.")]
        [InvalidLinq("Serializer for MongoDB.Analyzer.Tests.Common.DataModel.Apple does not have a member named _AppleID.")]
        public void Warnings_due_to_bson_ignore()
        {
            _ = GetMongoQueryable<Fruit>().Where(f => f.Quantity == 22);
            _ = GetMongoQueryable<Apple>().Where(a => a._AppleID == "Apple ID");
        }
    }
}
