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

namespace MongoDB.Analyzer.Tests.Common.TestCases.Poco
{
    public sealed class PocoArrays : TestCasesBase
    {
        [PocoJson("{ \"EnumArrayWithDimension1\" : [], \"JaggedEnumArray\" : [], \"TreeJaggedArray2\" : [], \"TreeNodeJaggedArray3\" : [] }")]
        public void CustomTypesArraysHolder()
        {
        }

        [PocoJson("{ \"Matrix2\" : [], \"Matrix3\" : [], \"Matrix4\" : [], \"Matrix5\" : [], \"Matrix6\" : [] }")]
        public void MultiDimensionalArrayHolder()
        {
        }

        [PocoJson("{ \"Data\" : 4, \"Children\" : [] }")]
        public void NestedArrayHolder()
        {
        }

        [PocoJson("{ \"BooleanArray\" : [], \"ByteArray\" : new BinData(0, \"\"), \"SByteArray\" : [], \"ShortArray\" : [], \"UShortArray\" : [], \"IntArray\" : [], \"UIntArray\" : [], \"LongArray\" : [], \"ULongArray\" : [], \"CharArray\" : [], \"DoubleArray\" : [], \"StringArray\" : [], \"FloatArray\" : [], \"ObjectArray\" : [], \"JaggedStringArray2\" : [], \"JaggedIntArray3\" : [], \"JaggedLongArray4\" : [], \"JaggedShortArray5\" : [] }")]
        public void SimpleTypesArraysHolder()
        {
        }

        public class TestClasses
        {
            public class CustomTypesArraysHolder
            {
                public EnumInt16[] EnumArrayWithDimension1 { get; set; }
                public EnumInt16[][] JaggedEnumArray { get; set; }

                public Tree[][] TreeJaggedArray2 { get; set; }
                public TreeNode[][][] TreeNodeJaggedArray3 { get; set; }
            }

            public class MultiDimensionalArrayHolder
            {
                public int[,] Matrix2 { get; set; }
                public int[,,] Matrix3 { get; set; }
                public int[,][,] Matrix4 { get; set; }
                public int[][,][,] Matrix5 { get; set; }
                public int[,][,][][] Matrix6 { get; set; }
            }

            public class NestedArrayHolder
            {
                public int Data { get; set; }
                public NestedArrayHolder[] Children { get; set; }
            }

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
        }
    }
}

