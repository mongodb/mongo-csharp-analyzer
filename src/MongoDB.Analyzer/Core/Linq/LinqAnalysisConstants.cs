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

internal static class LinqAnalysisConstants
{
    public const string GeneratedTypeName = "GenType";
}

internal static class LinqAnalysisErrorMessages
{
    public const string MethodInvocationNotSupported = "Method referencing lambda parameter is not supported LINQ expression.";
}

internal static class EFAnalysisErrorMessages
{
    public const string ByteArraysNotSupported = "Byte array type is not supported by this version of the EF provider.";
    public const string GroupByMethodNotSupported = "GroupBy is not supported by this version of the EF provider.";
}
