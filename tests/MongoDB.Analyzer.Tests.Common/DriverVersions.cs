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

namespace MongoDB.Analyzer.Tests.Common
{
    public static class DriverVersions
    {
        public const string Linq3OrGreater = "[2.14.0-beta1,)";
        public const string Linq2OrLower = "(,2.14.0-beta1)";
        public const string Linq3NonDefault = "[2.14.0-beta1, 2.19.0)";
        public const string Linq3DefaultOrGreater = V2_19_OrGreater;
        public const string Linq2DefaultAndLower = V2_18_OrLower;
        public const string V2_18_OrLower = "(, 2.19.0)";
        public const string V2_19_OrGreater = "[2.19.0,)";
        public const string V2_20_OrLower = "(, 2.20.0)";
        public const string V2_21_OrGreater = "[2.21.0,)";
        public const string V2_22_OrLower = "(, 2.23.0)";
        public const string V2_23_OrGreater = "[2.23.0,)";
        public const string V2_19_to_2_20= "[2.19.0, 2.20.0)";
    }
}
