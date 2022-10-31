﻿// Copyright 2021-present MongoDB Inc.
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
using System.Text;
using Microsoft.CodeAnalysis;

namespace MongoDB.Analyzer.Tests.Infrastructure;

public record DiagnosticTestCaseResult(
    string Name,
    int TestCaseMethodStartLine,
    Diagnostic[] Diagnostics)
{
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Name: {Name}");
        sb.AppendLine($"Offset: {TestCaseMethodStartLine}");

        foreach (var group in Diagnostics.GroupBy(d => d.GetMessage()))
        {
            sb.Append("\"");
            sb.Append(group.Key);
            sb.Append("\",");
            sb.Append(string.Join(",", group.Select(d => d.Location.GetLineSpan().StartLinePosition.Line - TestCaseMethodStartLine)));
            sb.Append(" a: ");
            sb.Append(string.Join(",", group.Select(d => d.Location.GetLineSpan().StartLinePosition.Line)));
            sb.AppendLine();
        }

        return sb.ToString();
    }
}
