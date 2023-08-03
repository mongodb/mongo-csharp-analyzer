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

namespace MongoDB.Analyzer.Tests.Common.DataModel
{
    public class PrimitiveTypeHolder
    {
        public bool BooleanValue { get; set; }
        public byte ByteValue { get; set; }
        public sbyte SByteValue { get; set; }

        public short ShortValue { get; set; }
        public ushort UShortValue { get; set; }

        public int IntValue { get; set; }
        public uint UIntValue { get; set; }

        public long LongValue { get; set; }
        public ulong ULongValue { get; set; }

        public char CharValue { get; set; }
        public double DoubleValue { get; set; }

        public string StringValue { get; set; }
        public float FloatValue { get; set; }
    }
}

