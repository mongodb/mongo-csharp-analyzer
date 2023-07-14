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

using MongoDB.Analyzer.Core.HelperResources;

namespace MongoDB.Analyzer.Core;

internal static class TypesGeneratorHelper
{
    private static readonly CompilationUnitSyntax s_generatedTypesCompliationUnit = SyntaxFactory.CompilationUnit()
        .AddUsings(
            Using("System"),
            Using("MongoDB.Bson"),
            Using("MongoDB.Bson.Serialization.Attributes"),
            Using("MongoDB.Bson.Serialization.Options"));


    private static readonly NamespaceDeclarationSyntax s_namespaceDeclarationSyntaxBuilders = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(MqlGeneratorSyntaxElements.Builders.MqlGeneratorNamespace));
    private static readonly NamespaceDeclarationSyntax s_namespaceDeclarationSyntaxLinq = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(MqlGeneratorSyntaxElements.Linq.MqlGeneratorNamespace));

    public static SyntaxTree GenerateTypesSyntaxTree(
        AnalysisType analysisType,
        MemberDeclarationSyntax[] typesDeclarations,
        ParseOptions parseOptions)
    {
        var namespaceDeclaration = analysisType switch
        {
            AnalysisType.Builders => s_namespaceDeclarationSyntaxBuilders,
            AnalysisType.Linq => s_namespaceDeclarationSyntaxLinq,
            _ => throw new ArgumentOutOfRangeException(nameof(analysisType), analysisType, "Unsupported analysis type")
        };

        var generatedTypesCompilationUnit = s_generatedTypesCompliationUnit
            .AddMembers(namespaceDeclaration.AddMembers(typesDeclarations));

        var syntaxTree = generatedTypesCompilationUnit.SyntaxTree
            .WithRootAndOptions(generatedTypesCompilationUnit.SyntaxTree.GetRoot(), parseOptions);

        return syntaxTree;
    }

    private static UsingDirectiveSyntax Using(string namespaceName) =>
        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(namespaceName));

    private static UsingDirectiveSyntax Using(string typeNewName, string typeName) =>
        SyntaxFactory.UsingDirective(
                SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName(typeNewName)),
                SyntaxFactory.ParseName(typeName));
}
