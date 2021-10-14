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
using MongoDB.Driver;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Builders
{
    public sealed class BuildersArrays : TestCasesBase
    {
        [BuildersMQL("{ \"IntArray.0\" : { \"$in\" : [11, 22, 33] } }")]
        public void Filter_in()
        {
            _ = Builders<SimpleTypesArraysHolder>.Filter.In(t => t.IntArray[0], new[] { 11, 22, 33 });
        }

        [BuildersMQL("{ \"$or\" : [{ \"IntArray\" : { \"$gt\" : 123 } }, { \"ObjectArray\" : { \"$ne\" : null } }] }")]
        public void Filter_any()
        {
            _ = Builders<SimpleTypesArraysHolder>.Filter.AnyGt(t => t.IntArray, 123) |
                Builders<SimpleTypesArraysHolder>.Filter.AnyNe(t => t.ObjectArray, null);
        }

        [BuildersMQL("{ \"$or\" : [{ \"IntArray.12\" : 1 }, { \"IntArray.0\" : 2147483647 }, { \"ObjectArray.0\" : null }, { \"ObjectArray.123\" : { \"$ne\" : null } }] }")]
        public void Array_of_predefined_type_item_access()
        {
            _ = Builders<SimpleTypesArraysHolder>.Filter.Eq(u => u.IntArray[12], 1) |
                Builders<SimpleTypesArraysHolder>.Filter.Eq(u => u.IntArray.ElementAt(0), int.MaxValue) |
                Builders<SimpleTypesArraysHolder>.Filter.Eq(u => u.ObjectArray[0], null) |
                Builders<SimpleTypesArraysHolder>.Filter.Ne(u => u.ObjectArray.ElementAt(123), null);
        }

        [BuildersMQL("{ \"$or\" : [{ \"JaggedStringArray2.0.1\" : \"str\" }, { \"JaggedStringArray2.1.3\" : \"str2\" }, { \"JaggedIntArray3.2.999.29\" : -1 }, { \"JaggedIntArray3.10.1.3\" : { \"$ne\" : 3 } }, { \"JaggedLongArray4.2.3.2.2\" : { \"$ne\" : NumberLong(-9223372036854775808) } }, { \"JaggedIntArray3.10.1.3\" : { \"$ne\" : 23 } }, { \"JaggedLongArray4.10.1.3.3\" : { \"$ne\" : NumberLong(3) } }, { \"JaggedShortArray5.2.999.29.12.144\" : { \"$ne\" : 0 } }, { \"JaggedShortArray5.10.1.3.3.32\" : { \"$ne\" : 0 } }] }")]
        public void Jagged_array_of_predefined_type_items_access()
        {
            _ = Builders<SimpleTypesArraysHolder>.Filter.Eq(u => u.JaggedStringArray2[0][1], "str") |
                Builders<SimpleTypesArraysHolder>.Filter.Eq(u => u.JaggedStringArray2.ElementAt(1).ElementAt(3), "str2") |
                Builders<SimpleTypesArraysHolder>.Filter.Eq(u => u.JaggedIntArray3[2][999][29], -1) |
                Builders<SimpleTypesArraysHolder>.Filter.Ne(u => u.JaggedIntArray3.ElementAt(10).ElementAt(1).ElementAt(3), 3) |
                Builders<SimpleTypesArraysHolder>.Filter.Ne(u => u.JaggedLongArray4[2][3][2][2], long.MinValue) |
                Builders<SimpleTypesArraysHolder>.Filter.Ne(u => u.JaggedIntArray3.ElementAt(10).ElementAt(1).ElementAt(3), 23) |
                Builders<SimpleTypesArraysHolder>.Filter.Ne(u => u.JaggedLongArray4.ElementAt(10).ElementAt(1).ElementAt(3).ElementAt(3), 3) |
                Builders<SimpleTypesArraysHolder>.Filter.Ne(u => u.JaggedShortArray5[2][999][29][12][144], 0) |
                Builders<SimpleTypesArraysHolder>.Filter.Ne(u => u.JaggedShortArray5.ElementAt(10).ElementAt(1).ElementAt(3).ElementAt(3).ElementAt(32), 0);
        }

        [BuildersMQL("{ \"$or\" : [{ \"EnumArrayWithDimension1.12\" : 32767 }, { \"TreeJaggedArray2.0.1.Root.Data\" : 1 }, { \"TreeNodeJaggedArray3.0.1.2.Data\" : 3 }, { \"TreeNodeJaggedArray3.2.1.0.Left.Data\" : { \"$ne\" : 123 } }] }")]
        public void Array_of_custom_type_item_access()
        {
            _ = Builders<CustomTypesArraysHolder>.Filter.Eq(u => u.EnumArrayWithDimension1[12], EnumInt16.MaxValue) |
                Builders<CustomTypesArraysHolder>.Filter.Eq(u => u.TreeJaggedArray2[0][1].Root.Data, 1) |
                Builders<CustomTypesArraysHolder>.Filter.Eq(u => u.TreeNodeJaggedArray3[0][1][2].Data, 3) |
                Builders<CustomTypesArraysHolder>.Filter.Ne(u => u.TreeNodeJaggedArray3[2][1][0].Left.Data, 123);
        }

        [BuildersMQL("{ \"$or\" : [{ \"Children.0.Data\" : 1 }, { \"Children.0.Children.1.Children.2.Children.3.Children.4.Data\" : 21 }] }")]
        public void Array_of_self()
        {
            _ = Builders<NestedArrayHolder>.Filter.Eq(t => t.Children[0].Data, 1) |
                Builders<NestedArrayHolder>.Filter.Eq(t => t.Children[0].Children[1].Children[2].Children[3].Children[4].Data, 21);
        }
    }
}
