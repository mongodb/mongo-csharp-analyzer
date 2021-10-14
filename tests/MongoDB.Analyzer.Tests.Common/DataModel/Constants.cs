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
    public sealed class ConstantsHolder
    {
        public const byte ConstantByte = 1;
        public const int ConstantInt = 2;
        public const short ConstantShort = short.MaxValue;
        public const long ConstantLong = long.MaxValue;
        public const string ConstantString = "BBB";
        public const string ConstantString2 = "CCCC";
        public const double ConstantDouble = 234.432;
        public const VehicleTypeEnum ConstantEnum = VehicleTypeEnum.Bus;
    }

    public static class StaticHolder
    {
        public static readonly byte ReadonlyByte = 1;
        public static readonly int ReadonlyInt = 2;
        public static readonly short ReadonlyShort = short.MaxValue;
        public static readonly long ReadonlyLong = long.MaxValue;
        public static readonly string ReadonlyString = "BBB";
        public static readonly string ReadonlyString2 = "CCCC";
        public static readonly double ReadonlyDouble = 234.432;
        public static readonly VehicleTypeEnum ReadonlyEnum = VehicleTypeEnum.Bus;

        public static byte PropByte { get; } = 1;
        public static int PropInt { get; } = 2;
        public static short PropShort { get; } = short.MaxValue;
        public static long PropLong { get; } = long.MaxValue;
        public static string PropString { get; } = "BBB";
        public static string PropString2 { get; } = "CCCC";
        public static double PropDouble { get; } = 234.432;
        public static VehicleTypeEnum PropEnum { get; } = VehicleTypeEnum.Bus;

        public static Person Person { get; }
    }
}
