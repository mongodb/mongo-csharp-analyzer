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

namespace MongoDB.Analyzer.Core.Linq;

internal static class MqlGeneratorSyntaxElements
{
    public const string GetDriverVersion = nameof(GetDriverVersion);
    public const string IQueryableProviderV2 = nameof(IQueryableProviderV2);
    public const string IQueryableProviderV3 = nameof(IQueryableProviderV3);
    public const string QueryableVarialbeName = "queryable";
    public const string MqlGenerator = nameof(MqlGenerator);
    public const string MqlGeneratorTemplateType = nameof(MqlGeneratorTemplateType);

    public const string MqlGeneratorNamespace = "MongoDB.Analyzer.Helpers.Linq";
    public const string MqlGeneratorFullName = MqlGeneratorNamespace + "." + MqlGenerator;
    public const string MqlGeneratorMainMethodName = "GetMQL";
}
