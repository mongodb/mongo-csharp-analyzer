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
    public class EnumHolder
    {
        public EnumInt8 EnumInt8 { get; set; }
        public EnumUInt8 EnumUInt8 { get; set; }
        public EnumInt16 EnumInt16 { get; set; }
        public EnumUInt16 EnumUInt16 { get; set; }
        public EnumInt32 EnumInt32 { get; set; }
        public EnumUInt32 EnumUInt32 { get; set; }
        public EnumInt64 EnumInt64 { get; set; }
        public EnumUInt64 EnumUInt64 { get; set; }
    }

    public enum EnumInt8 : sbyte
    {
        Value0,
        Value1,

        Value9 = 9,
        MaxValue = sbyte.MaxValue
    }

    public enum EnumUInt8 : byte
    {
        Value0,
        Value1,

        Value9 = 9,
        MaxValue = byte.MaxValue
    }

    public enum EnumInt16 : short
    {
        Value0,
        Value1,

        Value999 = 999,
        MaxValue = short.MaxValue
    }

    public enum EnumUInt16 : ushort
    {
        Value0,
        Value1,

        Value999 = 999,
        MaxValue = ushort.MaxValue
    }

    public enum EnumInt32 : int
    {
        Value0,
        Value1,

        Value999 = 999,
        MaxValue = int.MaxValue
    }

    public enum EnumUInt32 : uint
    {
        Value0,
        Value1,

        Value999 = 999,
        MaxValue = uint.MaxValue
    }

    public enum EnumInt64 : long
    {
        Value0,
        Value1,

        Value999 = 999,
        MaxValue = long.MaxValue
    }

    public enum EnumUInt64 : ulong
    {
        Value0,
        Value1,

        Value999 = 999,
        MaxValue = ulong.MaxValue
    }
}
