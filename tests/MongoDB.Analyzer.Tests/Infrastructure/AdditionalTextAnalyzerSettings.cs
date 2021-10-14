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

using System.Text.Json;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using MongoDB.Analyzer.Core;

namespace MongoDB.Analyzer.Tests.Infrastructure;

internal sealed class AdditionalTextAnalyzerSettings : AdditionalText
{
    private readonly SourceText _sourceText;

    public override string Path { get; }

    public AdditionalTextAnalyzerSettings(MongoDBAnalyzerSettings settings)
    {
        Path = SettingsHelper.SettingsFileName;
        _sourceText = SourceText.From(JsonSerializer.Serialize(settings));
    }

    public override SourceText GetText(CancellationToken cancellationToken = default) =>
        _sourceText;
}
