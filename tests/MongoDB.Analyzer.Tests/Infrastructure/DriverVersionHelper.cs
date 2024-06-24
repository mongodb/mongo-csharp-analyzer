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

using System;
using System.Linq;
using NuGet.Versioning;

namespace MongoDB.Analyzer.Tests.Infrastructure;

internal static class DriverVersionHelper
{
    public static readonly NuGetVersion[] DriverVersions;

    static DriverVersionHelper()
    {
        var driverVersion = Environment.GetEnvironmentVariable("DRIVER_VERSION");
        //DriverVersions = new[] { NuGetVersion.Parse(driverVersion) };

        // For running tests in IDE with specific driver version, either set DRIVER_VERSION or adjust DriverVersions explicitly
        DriverVersions = new[]
        {
           NuGetVersion.Parse("2.12.4"),
           NuGetVersion.Parse("2.18.0"),
           NuGetVersion.Parse("2.19.0"),
           NuGetVersion.Parse("2.21.0"),
           NuGetVersion.Parse("2.25.0")
         };
    }

    public static NuGetVersion[] FilterVersionForRange(string versionRange)
    {
        if (versionRange == null)
        {
            return DriverVersions;
        }

        var versionRangeParsed = VersionRange.Parse(versionRange);
        return DriverVersions.Where(versionRangeParsed.Satisfies).ToArray();
    }
}
