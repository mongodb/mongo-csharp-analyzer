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

namespace MongoDB.Analyzer.Core;

internal enum AnalysisType
{
    Unknown,
    Builders,
    EF,
    Linq,
    Poco
}

internal sealed class ExpressionsAnalysis
{
    public MemberDeclarationSyntax[] TypesDeclarations { get; set; }

    public ExpressionAnalysisContext[] AnalysisNodeContexts { get; set; }
    public InvalidExpressionAnalysisNode[] InvalidExpressionNodes { get; set; }
}

internal record AnalysisStats(
    int MqlCount,
    int JsonCount,
    int InternalExceptionsCount,
    int DriverExceptionsCount,
    string DriverVersion,
    string TargetFramework)
{
    public static AnalysisStats Empty { get; } = new AnalysisStats(0, 0, 0, 0, null, null);
}

internal record ExpressionAnalysisContext(ExpressionAnalysisNode Node)
{
    public string EvaluationMethodName { get; set; }
}

internal record ExpressionAnalysisNode(
    SyntaxNode OriginalExpression,
    string ArgumentTypeName,
    SyntaxNode RewrittenExpression,
    ConstantsMapper ConstantsRemapper,
    params Location[] Locations) :
    ExpressionAnalysisNodeBase(OriginalExpression);

internal abstract record ExpressionAnalysisNodeBase(SyntaxNode OriginalExpression);

internal record InvalidExpressionAnalysisNode(SyntaxNode OriginalExpression, params string[] Errors) :
    ExpressionAnalysisNodeBase(OriginalExpression);