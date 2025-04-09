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
        public const string V3_0_OrGreater = "[3.0.0, )";
        public const string V3_1_AndLower = "[3.0.0, 3.2.0)";
        public const string V3_2_OrGreater = "[3.2.0, )";
    }
}
