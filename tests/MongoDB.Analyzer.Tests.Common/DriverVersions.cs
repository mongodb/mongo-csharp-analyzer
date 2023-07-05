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
        public const string Linq3AndHigher = "[2.14.0-beta1,)";
        public const string Linq2AndLower = "(,2.14.0-beta1)";
        public const string Linq3NonDefault = "[2.14.0-beta1, 2.19.0)";
        public const string Linq3DefaultAndHigher = V2_19_AndHigher;
        public const string Linq2DefaultAndLower = V2_18_AndLower;

        public const string V2_14_Beta1_Till_V2_18 = "[2.14.0-beta1, 2.18.0)";
        public const string V2_18_AndLower = "(, 2.19.0)";
        public const string V2_19_AndHigher = "[2.19.0,)";

        public const string V2_14_Beta1 = "2.14.0-beta1";
        public const string V2_18 = "2.18.0";
        public const string V2_19 = "2.19.0";
    }
}
