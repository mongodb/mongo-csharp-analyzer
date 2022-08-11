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

namespace MongoDB.Analyzer.Core.Builders;

internal static class ResourceNames
{
    private const string HelpersResourcesBasePath = "MongoDB.Analyzer.Core.Builders.HelpersResources.";
    private const string CorePath = "MongoDB.Analyzer.Core.";

    public const string MqlGenerator = HelpersResourcesBasePath + nameof(MqlGenerator) + ".cs";

    public const string Renderer = HelpersResourcesBasePath + nameof(Renderer) + ".cs";

    public const string MongoCollectionMock = CorePath + nameof(MongoCollectionMock) + ".cs";

    public const string EmptyCursor = CorePath + nameof(EmptyCursor) + ".cs";

    public const string MongoDatabaseMock = CorePath + nameof(MongoDatabaseMock) + ".cs";

    public const string MongoClientMock = CorePath + nameof(MongoClientMock) + ".cs";
}
