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

using Microsoft.CodeAnalysis.CSharp.Syntax;
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
    private static readonly NamespaceDeclarationSyntax s_namespaceDeclarationSyntaxJson = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(JsonSyntaxElements.Json.JsonGeneratorNamespace));

    public static SyntaxTree GenerateTypesSyntaxTree(
        AnalysisType analysisType,
        MemberDeclarationSyntax[] typesDeclarations,
        ParseOptions parseOptions)
    {
        var namespaceDeclaration = analysisType switch
        {
            AnalysisType.Builders => s_namespaceDeclarationSyntaxBuilders,
            AnalysisType.Linq => s_namespaceDeclarationSyntaxLinq,
            AnalysisType.Poco => s_namespaceDeclarationSyntaxJson,
            _ => throw new ArgumentOutOfRangeException(nameof(analysisType), analysisType, "Unsupported analysis type")
        };

        var modifiedTypesDeclarations = analysisType == AnalysisType.Poco ? GenerateModifiedTypesDeclarations(typesDeclarations, 2) : typesDeclarations;

        var modifiedTypeNames = modifiedTypesDeclarations.OfType<ClassDeclarationSyntax>().Select(c => c.Identifier.ValueText);
        var generatedTypesCompilationUnit = s_generatedTypesCompliationUnit
            .AddMembers(namespaceDeclaration.AddMembers(modifiedTypesDeclarations));

        var syntaxTree = generatedTypesCompilationUnit.SyntaxTree
            .WithRootAndOptions(generatedTypesCompilationUnit.SyntaxTree.GetRoot(), parseOptions);

        return syntaxTree;
    }

    private static MemberDeclarationSyntax[] GenerateModifiedTypesDeclarations(MemberDeclarationSyntax[] typesDeclarations, int depthLevel)
    {
        var rewrittenTypeDeclarations = new List<MemberDeclarationSyntax>();
        var alreadyProcessed = new HashSet<string>();

        foreach (var typeDeclaration in typesDeclarations)
        {
            var rewrittenTypeDeclaration = ProcessTypeDeclaration(typesDeclarations, rewrittenTypeDeclarations, alreadyProcessed, typeDeclaration, 0, depthLevel);
            rewrittenTypeDeclarations.Add(rewrittenTypeDeclaration);
        }

        return rewrittenTypeDeclarations.ToArray();
    }

    private static MemberDeclarationSyntax ProcessTypeDeclaration(MemberDeclarationSyntax[] typesDeclarations, List<MemberDeclarationSyntax> rewrittenTypeDeclarations, HashSet<string> alreadyProcessed,
        MemberDeclarationSyntax typeDeclaration, int depth, int depthLevel)
    {
        if (typeDeclaration is ClassDeclarationSyntax classDeclaration)
        {
            var nodesRemapping = new Dictionary<SyntaxNode, SyntaxNode>();
            ProcessProperties(typesDeclarations, rewrittenTypeDeclarations, alreadyProcessed, classDeclaration, nodesRemapping, depth, depthLevel);
            ProcessFields(typesDeclarations, rewrittenTypeDeclarations, alreadyProcessed, classDeclaration, nodesRemapping, depth, depthLevel);
            typeDeclaration = typeDeclaration.ReplaceNodes(nodesRemapping.Keys, (n, _) => nodesRemapping[n]);
        }

        return typeDeclaration;
    }

    private static void ProcessProperties(MemberDeclarationSyntax[] typesDeclarations, List<MemberDeclarationSyntax> rewrittenTypeDeclarations, HashSet<string> alreadyProcessed,
        ClassDeclarationSyntax classDeclaration, Dictionary<SyntaxNode, SyntaxNode> nodesRemapping, int depth, int depthLevel)
    {
        foreach (var property in classDeclaration.DescendantNodes().OfType<PropertyDeclarationSyntax>())
        {
            var propertyName = property.Identifier.ValueText;
            var propertyType = property.Type;

            if (propertyType is PredefinedTypeSyntax predefinedType)
            {
                nodesRemapping.Add(property, property.WithInitializer(ProcessPredefinedType(predefinedType, propertyName)));
            }

            else if (propertyType is ArrayTypeSyntax arrayType)
            {
                nodesRemapping.Add(property, property.WithInitializer(ProcessArrayType(arrayType)));
            }

            else if (propertyType is QualifiedNameSyntax qualifiedName)
            {
                nodesRemapping.Add(property, property.WithInitializer(ProcessQualifiedName(qualifiedName)));
            }

            else if (propertyType is IdentifierNameSyntax identifierNameSyntax && depth <= depthLevel)
            {
                var (rewrittenPropertyTypeName, rewrittenPropertyTypeDeclaration) = ProcessIdentifierName(typesDeclarations, classDeclaration, identifierNameSyntax);

                if (rewrittenPropertyTypeDeclaration == null)
                {
                    continue;
                }

                var rewrittenPropertyTypeSyntax = SyntaxFactory.ParseTypeName(rewrittenPropertyTypeName);

                var modifiedProperty = property.WithType(rewrittenPropertyTypeSyntax)
                    .WithInitializer(SyntaxFactory.EqualsValueClause(SyntaxFactory.ObjectCreationExpression(rewrittenPropertyTypeSyntax, SyntaxFactory.ArgumentList(new SeparatedSyntaxList<ArgumentSyntax>()), null)));

                var propertyMember = ProcessTypeDeclaration(typesDeclarations, rewrittenTypeDeclarations, alreadyProcessed, rewrittenPropertyTypeDeclaration, depth + 1, depthLevel);

                nodesRemapping.Add(property, modifiedProperty);
                if (alreadyProcessed.Add(rewrittenPropertyTypeName))
                {
                    rewrittenTypeDeclarations.Add(propertyMember);
                }
            }
        }
    }

    private static void ProcessFields(MemberDeclarationSyntax[] typesDeclarations, List<MemberDeclarationSyntax> rewrittenTypeDeclarations, HashSet<string> alreadyProcessed,
        ClassDeclarationSyntax classDeclaration, Dictionary<SyntaxNode, SyntaxNode> nodesRemapping, int depth, int depthLevel)
    {
        foreach (var field in classDeclaration.DescendantNodes().OfType<FieldDeclarationSyntax>())
        {
            var fieldType = field.Declaration.Type;
            var variableDeclaration = field.Declaration;

            foreach (var variableDeclarator in variableDeclaration.Variables)
            {
                if (fieldType is PredefinedTypeSyntax predefinedType)
                {
                    nodesRemapping.Add(variableDeclarator, variableDeclarator.WithInitializer(ProcessPredefinedType(predefinedType, variableDeclarator.Identifier.ValueText)));
                }

                else if (fieldType is ArrayTypeSyntax arrayType)
                {
                    nodesRemapping.Add(variableDeclarator, variableDeclarator.WithInitializer(ProcessArrayType(arrayType)));
                }

                else if (fieldType is QualifiedNameSyntax qualifiedName)
                {
                    nodesRemapping.Add(variableDeclarator, variableDeclarator.WithInitializer(ProcessQualifiedName(qualifiedName)));
                }

                else if (fieldType is IdentifierNameSyntax identifierNameSyntax && depth <= depthLevel)
                {
                    var (rewrittenFieldTypeName, rewrittenFieldTypeDeclaration) = ProcessIdentifierName(typesDeclarations, classDeclaration, identifierNameSyntax);

                    if (rewrittenFieldTypeDeclaration == null)
                    {
                        continue;
                    }

                    var rewrittenFieldTypeSyntax = SyntaxFactory.ParseTypeName(rewrittenFieldTypeName);

                    var modifiedVariableDeclarator = variableDeclarator.WithInitializer(SyntaxFactory.EqualsValueClause(
                        SyntaxFactory.ObjectCreationExpression(rewrittenFieldTypeSyntax, SyntaxFactory.ArgumentList(new SeparatedSyntaxList<ArgumentSyntax>()), null)));

                    var rewrittenVariableDeclaration = variableDeclaration.WithType(rewrittenFieldTypeSyntax)
                        .WithVariables(SyntaxFactory.SingletonSeparatedList(modifiedVariableDeclarator));

                    var rewrittenField = field.WithDeclaration(rewrittenVariableDeclaration);
                    var fieldMember = ProcessTypeDeclaration(typesDeclarations, rewrittenTypeDeclarations, alreadyProcessed, rewrittenFieldTypeDeclaration, depth + 1, depthLevel);

                    nodesRemapping.Add(field, rewrittenField);
                    if (alreadyProcessed.Add(rewrittenFieldTypeName))
                    {
                        rewrittenTypeDeclarations.Add(fieldMember);
                    }
                }
            }
        }
    }

    private static EqualsValueClauseSyntax ProcessPredefinedType(PredefinedTypeSyntax predefinedType, string memberName) =>
        predefinedType.Keyword.ValueText == "string" ?
        SyntaxFactory.EqualsValueClause(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(memberName))) : null;

    private static EqualsValueClauseSyntax ProcessArrayType(ArrayTypeSyntax arrayType) =>
        SyntaxFactory.EqualsValueClause(SyntaxFactory.ArrayCreationExpression(arrayType, SyntaxFactory.InitializerExpression(SyntaxKind.ArrayInitializerExpression, SyntaxFactory.SeparatedList<ExpressionSyntax>())));

    private static EqualsValueClauseSyntax ProcessQualifiedName(QualifiedNameSyntax qualifiedName) =>
        qualifiedName.Left.ToString() == "System.Collections.Generic" ? 
        SyntaxFactory.EqualsValueClause(SyntaxFactory.ObjectCreationExpression(qualifiedName, SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList<ArgumentSyntax>()), null)) : null;

    private static (string RewrittenMemberTypeName, ClassDeclarationSyntax RewrittenMemberTypeDeclaration) ProcessIdentifierName(MemberDeclarationSyntax[] typesDeclarations, ClassDeclarationSyntax classDeclaration, IdentifierNameSyntax memberTypeName)
    {
        var memberTypeDeclaration = typesDeclarations.FirstOrDefault(t => t is ClassDeclarationSyntax classDeclaration
            && classDeclaration.Identifier.ValueText == memberTypeName.Identifier.ValueText) as ClassDeclarationSyntax;

        if (memberTypeDeclaration == null)
        {
            return default;
        }

        var classIdentifierName = classDeclaration.Identifier.ValueText;
        var rewrittenMemberTypeName = classIdentifierName + "_" + memberTypeName.Identifier.ValueText;
        var rewrittenMemberTypeDeclaration = memberTypeDeclaration.WithIdentifier(SyntaxFactory.Identifier(rewrittenMemberTypeName));

        return (rewrittenMemberTypeName, rewrittenMemberTypeDeclaration);
    }

    private static UsingDirectiveSyntax Using(string namespaceName) =>
        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(namespaceName));
}
