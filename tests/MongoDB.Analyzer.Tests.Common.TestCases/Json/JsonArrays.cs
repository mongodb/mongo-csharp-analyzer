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

namespace MongoDB.Analyzer.Tests.Common.TestCases.Json
{
    public sealed class JsonArrays : TestCasesBase
    {
        [Json("{ \"BooleanArray\" : [false, false], \"ByteArray\" : new BinData(0, \"AAA=\"), \"SByteArray\" : [0, 0], \"ShortArray\" : [0, 0], \"UShortArray\" : [0, 0], \"IntArray\" : [0, 0], \"UIntArray\" : [0, 0], \"LongArray\" : [NumberLong(0), NumberLong(0)], \"ULongArray\" : [NumberLong(0), NumberLong(0)], \"CharArray\" : [0, 0], \"DoubleArray\" : [0.0, 0.0], \"StringArray\" : [\"StringArray\", \"StringArray\"], \"FloatArray\" : [0.0, 0.0], \"ObjectArray\" : [{ }, { }], \"JaggedStringArray2\" : [[\"JaggedStringArray2\", \"JaggedStringArray2\"], [\"JaggedStringArray2\", \"JaggedStringArray2\"]], \"JaggedIntArray3\" : [[[0, 0], [0, 0]], [[0, 0], [0, 0]]], \"JaggedLongArray4\" : [[[[NumberLong(0), NumberLong(0)], [NumberLong(0), NumberLong(0)]], [[NumberLong(0), NumberLong(0)], [NumberLong(0), NumberLong(0)]]], [[[NumberLong(0), NumberLong(0)], [NumberLong(0), NumberLong(0)]], [[NumberLong(0), NumberLong(0)], [NumberLong(0), NumberLong(0)]]]], \"JaggedShortArray5\" : [[[[[0, 0], [0, 0]], [[0, 0], [0, 0]]], [[[0, 0], [0, 0]], [[0, 0], [0, 0]]]], [[[[0, 0], [0, 0]], [[0, 0], [0, 0]]], [[[0, 0], [0, 0]], [[0, 0], [0, 0]]]]] }")]
        public void SimpleTypesArraysHolder()
        {
        }

        [Json("{ \"Matrix2\" : [[0, 0], [0, 0]], \"Matrix3\" : [[[0, 0], [0, 0]], [[0, 0], [0, 0]]] }")]
        public void MultiDimensionalArrayHolder()
        {
        }

        [Json("{ \"EnumArrayWithDimension1\" : [0, 0], \"JaggedEnumArray\" : [[0, 0], [0, 0]], \"TreeJaggedArray2\" : [[{ \"Root\" : { \"Data\" : 0, \"Left\" : { \"Data\" : 0, \"Left\" : null, \"Right\" : null, \"Tree\" : null }, \"Right\" : { \"Data\" : 0, \"Left\" : null, \"Right\" : null, \"Tree\" : null }, \"Tree\" : { \"Root\" : null } } }, { \"Root\" : { \"Data\" : 0, \"Left\" : { \"Data\" : 0, \"Left\" : null, \"Right\" : null, \"Tree\" : null }, \"Right\" : { \"Data\" : 0, \"Left\" : null, \"Right\" : null, \"Tree\" : null }, \"Tree\" : { \"Root\" : null } } }], [{ \"Root\" : null }, { \"Root\" : null }]], \"TreeNodeJaggedArray3\" : [[[{ \"Data\" : 0, \"Left\" : null, \"Right\" : null, \"Tree\" : null }, { \"Data\" : 0, \"Left\" : null, \"Right\" : null, \"Tree\" : null }], [{ \"Data\" : 0, \"Left\" : null, \"Right\" : null, \"Tree\" : null }, { \"Data\" : 0, \"Left\" : null, \"Right\" : null, \"Tree\" : null }]], [[{ \"Data\" : 0, \"Left\" : null, \"Right\" : null, \"Tree\" : null }, { \"Data\" : 0, \"Left\" : null, \"Right\" : null, \"Tree\" : null }], [{ \"Data\" : 0, \"Left\" : null, \"Right\" : null, \"Tree\" : null }, { \"Data\" : 0, \"Left\" : null, \"Right\" : null, \"Tree\" : null }]]] }")]
        public void CustomTypesArraysHolder()
        {
        }

        [Json("{ \"Data\" : 0, \"Children\" : [{ \"Data\" : 0, \"Children\" : null }, { \"Data\" : 0, \"Children\" : null }] }")]
        public void NestedArrayHolder()
        {
        }

        public class TestClasses
        {
            public class SimpleTypesArraysHolder
            {
                public bool[] BooleanArray { get; set; }
                public byte[] ByteArray { get; set; }
                public sbyte[] SByteArray { get; set; }

                public short[] ShortArray { get; set; }
                public ushort[] UShortArray { get; set; }

                public int[] IntArray { get; set; }
                public uint[] UIntArray { get; set; }

                public long[] LongArray { get; set; }
                public ulong[] ULongArray { get; set; }

                public char[] CharArray { get; set; }
                public double[] DoubleArray { get; set; }

                public string[] StringArray { get; set; }
                public float[] FloatArray { get; set; }
                public object[] ObjectArray { get; set; }

                public string[][] JaggedStringArray2 { get; set; }
                public int[][][] JaggedIntArray3 { get; set; }
                public long[][][][] JaggedLongArray4 { get; set; }
                public short[][][][][] JaggedShortArray5 { get; set; }
            }

            public class MultiDimensionalArrayHolder
            {
                public int[,] Matrix2 { get; set; }
                public int[,,] Matrix3 { get; set; }
            }

            public class CustomTypesArraysHolder
            {
                public EnumInt16[] EnumArrayWithDimension1 { get; set; }
                public EnumInt16[][] JaggedEnumArray { get; set; }

                public Tree[][] TreeJaggedArray2 { get; set; }
                public TreeNode[][][] TreeNodeJaggedArray3 { get; set; }
            }

            public class NestedArrayHolder
            {
                public int Data { get; set; }
                public NestedArrayHolder[] Children { get; set; }
            }
        }
    }
}

