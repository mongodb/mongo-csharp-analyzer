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
    public sealed class LinqArrays : TestCasesBase
    {
        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"IntArray.12\" : 1 }, { \"IntArray.0\" : 2147483647 }] } }, { \"$match\" : { \"$or\" : [{ \"ObjectArray.0\" : null }, { \"ObjectArray.10\" : { \"$ne\" : null } }] } }])")]
        public void Array_of_predefined_type_item_access()
        {
            _ = GetMongoQueryable<SimpleTypesArraysHolder>()
                .Where(t => t.IntArray[12] == 1 || t.IntArray.ElementAt(0) == int.MaxValue)
                .Where(t => t.ObjectArray[0] == null || t.ObjectArray.ElementAt(10) != null);
        }

        [MQL("aggregate([{ \"$match\" : { \"IntArray.0\" : { \"$exists\" : true } } }, { \"$match\" : { \"ObjectArray\" : { \"$size\" : 3 } } }])")]
        public void Array_of_predefined_type_array_members_access()
        {
            _ = GetMongoQueryable<SimpleTypesArraysHolder>()
                .Where(t => t.IntArray.Length > 0)
                .Where(t => t.ObjectArray.Length == 3);
        }

        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"JaggedStringArray2.0.1\" : \"str\" }, { \"JaggedStringArray2.1.3\" : \"str2\" }] } }, { \"$match\" : { \"$or\" : [{ \"JaggedIntArray3.2.999.29\" : -1 }, { \"JaggedIntArray3.10.1.3\" : 3 }] } }, { \"$match\" : { \"$or\" : [{ \"JaggedLongArray4.2.3.2.2\" : NumberLong(-9223372036854775808) }, { \"JaggedLongArray4.10.1.3.3\" : NumberLong(23) }] } }, { \"$match\" : { \"$or\" : [{ \"JaggedShortArray5.2.999.29.12.144\" : 0 }, { \"JaggedShortArray5.10.1.3.3.32\" : 23 }] } }])")]
        public void Jagged_array_of_predefined_type_items_access()
        {
            _ = GetMongoQueryable<SimpleTypesArraysHolder>()
                .Where(t => t.JaggedStringArray2[0][1] == "str" || t.JaggedStringArray2.ElementAt(1).ElementAt(3) == "str2")
                .Where(t => t.JaggedIntArray3[2][999][29] == -1 || t.JaggedIntArray3.ElementAt(10).ElementAt(1).ElementAt(3) == 3)
                .Where(t => t.JaggedLongArray4[2][3][2][2] == long.MinValue || t.JaggedLongArray4.ElementAt(10).ElementAt(1).ElementAt(3).ElementAt(3) == 23)
                .Where(t => t.JaggedShortArray5[2][999][29][12][144] == 0 || t.JaggedShortArray5.ElementAt(10).ElementAt(1).ElementAt(3).ElementAt(3).ElementAt(32) == 23);
        }

        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"JaggedStringArray2\" : { \"$size\" : 1 } }, { \"JaggedStringArray2.0\" : { \"$size\" : 2 } }] } }, { \"$match\" : { \"$or\" : [{ \"JaggedIntArray3\" : { \"$size\" : 1 } }, { \"JaggedIntArray3.1.1\" : { \"$size\" : 2 } }] } }, { \"$match\" : { \"$or\" : [{ \"JaggedLongArray4\" : { \"$size\" : 1 } }, { \"JaggedLongArray4.1.2.3\" : { \"$size\" : 2 } }] } }, { \"$match\" : { \"$or\" : [{ \"JaggedShortArray5\" : { \"$size\" : 1 } }, { \"JaggedShortArray5.1.2.3.4\" : { \"$size\" : 2 } }] } }])")]
        public void Jagged_array_of_predefined_type_members_access()
        {
            _ = GetMongoQueryable<SimpleTypesArraysHolder>()
                .Where(t => t.JaggedStringArray2.Length == 1 || t.JaggedStringArray2[0].Length == 2)
                .Where(t => t.JaggedIntArray3.Length == 1 || t.JaggedIntArray3[1][1].Length == 2)
                .Where(t => t.JaggedLongArray4.Length == 1 || t.JaggedLongArray4[1][2][3].Length == 2)
                .Where(t => t.JaggedShortArray5.Length == 1 || t.JaggedShortArray5[1][2][3][4].Length == 2);
        }

        [MQL("aggregate([{ \"$match\" : { \"EnumArrayWithDimension1.0\" : 32767 } }, { \"$match\" : { \"TreeJaggedArray2.0.1.Root.Data\" : 1 } }, { \"$match\" : { \"$or\" : [{ \"TreeNodeJaggedArray3.0.1.2.Data\" : 3 }, { \"TreeNodeJaggedArray3.2.1.0.Left.Data\" : 1 }] } }])")]
        public void Array_of_custom_type_item_access()
        {
            _ = GetMongoQueryable<CustomTypesArraysHolder>()
                .Where(t => t.EnumArrayWithDimension1[0] == EnumInt16.MaxValue)
                .Where(t => t.TreeJaggedArray2[0][1].Root.Data == 1)
                .Where(t => t.TreeNodeJaggedArray3[0][1][2].Data == 3 || t.TreeNodeJaggedArray3[2][1][0].Left.Data == 1);
        }

        [MQL("aggregate([{ \"$match\" : { \"EnumArrayWithDimension1\" : { \"$size\" : 1 } } }, { \"$match\" : { \"TreeJaggedArray2\" : { \"$size\" : 2 } } }, { \"$match\" : { \"TreeNodeJaggedArray3\" : { \"$size\" : 3 } } }])")]
        public void Array_of_custom_type_member_access()
        {
            _ = GetMongoQueryable<CustomTypesArraysHolder>()
                .Where(t => t.EnumArrayWithDimension1.Length == 1)
                .Where(t => t.TreeJaggedArray2.Length == 2)
                .Where(t => t.TreeNodeJaggedArray3.Length == 3);
        }

        [MQL("aggregate([{ \"$match\" : { \"Children.0.Data\" : 1 } }, { \"$match\" : { \"Children.0.Children.1.Children.2.Children.3.Children.4.Data\" : 2 } }])")]
        public void Array_of_self()
        {
            _ = GetMongoQueryable<NestedArrayHolder>()
                .Where(t => t.Children[0].Data == 1)
                .Where(t => t.Children[0].Children[1].Children[2].Children[3].Children[4].Data == 2);
        }
    }
}
