﻿// Copyright 2021-present MongoDB Inc.
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

using System.Runtime.InteropServices;

namespace MongoDB.Analyzer.Core;

internal enum DriverTargetFramework
{
    Netstandard1_5 = 1 << 0,
    Netstandard2_0 = 1 << 1,
    Netstandard2_1 = 1 << 2,

    Net452 = 1 << 8,
    Net472 = 1 << 9,

    NetStandard = Netstandard1_5 | Netstandard2_0 | Netstandard2_1,
    NetFramework = Net452 | Net472,

    All = NetStandard | NetFramework
}

internal static class EnvironmentUtilities
{
    public static bool IsDotNetCore { get; private set; }
    public static bool IsDotNetFramework { get; private set; }

    static EnvironmentUtilities()
    {
        IsDotNetCore = RuntimeInformation.FrameworkDescription.Contains(".NET Core");
        IsDotNetFramework = RuntimeInformation.FrameworkDescription.Contains(".NET Framework");
    }

    public static bool IsDriverTargetFrameworkSupported(DriverTargetFramework driverTargetFramework) =>
        (IsDotNetCore && (driverTargetFramework & DriverTargetFramework.NetStandard) != 0) ||
        (IsDotNetFramework && (driverTargetFramework & DriverTargetFramework.NetFramework) != 0);
}
