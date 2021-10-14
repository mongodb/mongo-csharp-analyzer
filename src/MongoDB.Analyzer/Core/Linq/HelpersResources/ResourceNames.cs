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

internal static class ResourceNames
{
    private const string HelpersResourcesBasePath = "MongoDB.Analyzer.Core.Linq.HelpersResources.";

    public const string EmptyCursor = HelpersResourcesBasePath + nameof(EmptyCursor) + ".cs";
    public const string IQueryableProvider = HelpersResourcesBasePath + nameof(IQueryableProvider) + ".cs";
    public const string IQueryableProviderV2 = HelpersResourcesBasePath + nameof(IQueryableProviderV2) + ".cs";
    public const string IQueryableProviderV3 = HelpersResourcesBasePath + nameof(IQueryableProviderV3) + ".cs";
    public const string MongoCollectionMock = HelpersResourcesBasePath + nameof(MongoCollectionMock) + ".cs";
    public const string MqlGenerator = HelpersResourcesBasePath + nameof(MqlGenerator) + ".cs";
}
