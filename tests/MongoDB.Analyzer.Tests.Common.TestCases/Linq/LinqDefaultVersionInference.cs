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

using System.Linq;
using MongoDB.Driver.Linq;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Linq
{
    public sealed class LinqDefaultVersionInference : TestCasesBase
    {
        //// DriverVersion = [2.19.0,)
        //// DefaultLinqProvider = V2
        [NotSupportedLinq2("Supported in LINQ3 only: db.coll.Aggregate([{ \"$match\" : { \"Name\" : /^\\s*(?!\\s)123(?<!\\s)\\s*$/s } }])", version: DriverVersions.Linq3DefaultOrGreater)]
        //// DefaultLinqProvider = V3
        [MQL("db.coll.Aggregate([{ \"$match\" : { \"Name\" : /^\\s*(?!\\s)123(?<!\\s)\\s*$/s } }])", DriverVersions.Linq3DefaultOrGreater, LinqVersion.V3)]
        //// DefaultLinqProvider = Undefined
        [MQL("db.coll.Aggregate([{ \"$match\" : { \"Name\" : /^\\s*(?!\\s)123(?<!\\s)\\s*$/s } }])", DriverVersions.Linq3DefaultOrGreater, LinqVersion.Undefined)]
        public void Expression_should_be_supported_in_default_LINQ3()
        {
            _ = GetMongoQueryable()
                .Where(u => u.Name.Trim() == "123");
        }

        // DriverVersion = [2.14.0-beta1, 2.19.0)
        // DefaultLinqProvider = V2
        [NotSupportedLinq2("Supported in LINQ3 only: db.coll.Aggregate([{ \"$match\" : { \"Name\" : /^\\s*(?!\\s)123(?<!\\s)\\s*$/s } }])", version: DriverVersions.Linq3NonDefault, linqVersion: LinqVersion.V2)]
        // DefaultLinqProvider = V3
        [MQL("db.coll.Aggregate([{ \"$match\" : { \"Name\" : /^\\s*(?!\\s)123(?<!\\s)\\s*$/s } }])", DriverVersions.Linq3NonDefault, LinqVersion.V3)]
        // DefaultLinqProvider = Undefined
        [NotSupportedLinq2("Supported in LINQ3 only: db.coll.Aggregate([{ \"$match\" : { \"Name\" : /^\\s*(?!\\s)123(?<!\\s)\\s*$/s } }])", version: DriverVersions.Linq3NonDefault, linqVersion: LinqVersion.Undefined)]
        public void Expression_should_be_supported_in_non_default_LINQ3()
        {
            _ = GetMongoQueryable()
                .Where(u => u.Name.Trim() == "123");
        }

        // DriverVersion = (,2.14.0-beta1]
        // DefaultLinqProvider = V2
        [InvalidLinq("{document}{Name}.Trim() is not supported.", DriverVersions.Linq2OrLower, LinqVersion.V2)]
        // DefaultLinqProvider = V3
        [InvalidLinq("{document}{Name}.Trim() is not supported.", DriverVersions.Linq2OrLower, LinqVersion.V3)]
        // DefaultLinqProvider = Undefined
        [InvalidLinq("{document}{Name}.Trim() is not supported.", DriverVersions.Linq2OrLower, LinqVersion.Undefined)]
        public void Expression_should_not_be_supported_in_LINQ2_only()
        {
            _ = GetMongoQueryable()
                .Where(u => u.Name.Trim() == "123");
        }
    }
}
