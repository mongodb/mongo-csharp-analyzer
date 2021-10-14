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
using MongoDB.Driver.Linq;

namespace MongoDB.Analyzer.Helpers.Linq
{
    public static class MqlGenerator
    {
        private sealed class MqlGeneratorTemplateType
        {
            public Tuple<int, int> Field { get; set; }
        }

        private static readonly IQueryableProvider s_queryableProvider = new IQueryableProviderV2();

        public static string GetDriverVersion() => typeof(IMongoQueryable<>).Assembly.GetName().Version.ToString(3);

        public static string GetMQL(bool isV3)
        {
            var queryable = s_queryableProvider.GetQueryable<MqlGeneratorTemplateType>(isV3);
            var queryableWithExpression = queryable.Where(t => t.Field.Item1 == 1);

            queryableWithExpression.FirstOrDefault();
            return queryableWithExpression.ToString();
        }
    }
}
