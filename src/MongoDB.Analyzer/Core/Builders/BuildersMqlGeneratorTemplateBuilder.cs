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

using MongoDB.Analyzer.Core.Utilities;
using static MongoDB.Analyzer.Core.HelperResources.MqlGeneratorSyntaxElements.Builders;

namespace MongoDB.Analyzer.Core.Builders;

internal sealed class BuildersMqlGeneratorTemplateBuilder
{
    private ClassDeclarationSyntax _mqlGeneratorDeclarationSyntaxNew;
    private int _nextTestMethodIndex;
    private readonly MqlGeneratorTestMethodTemplate _testMethodTemplate;

    public BuildersMqlGeneratorTemplateBuilder(MqlGeneratorTestMethodTemplate testMethodTemplate)
    {
        _testMethodTemplate = testMethodTemplate;
        _mqlGeneratorDeclarationSyntaxNew = _testMethodTemplate.ClassDeclarationSyntax;
    }

    public void AddMqlGeneratorMethods(MemberDeclarationSyntax[] methodDeclarations) =>
        _mqlGeneratorDeclarationSyntaxNew = _mqlGeneratorDeclarationSyntaxNew.AddMembers(methodDeclarations);

    public static MqlGeneratorTestMethodTemplate CreateTestMethodTemplate(SyntaxTree mqlGeneratorSyntaxTree)
    {
        var root = mqlGeneratorSyntaxTree.GetRoot();

        var classDeclarationSyntax = root.GetSingleClassDeclaration(MqlGenerator);
        var mainTestMethodNode = classDeclarationSyntax.GetSingleMethod(MqlGeneratorMainMethodName);
        var builderDefinitionNode = mainTestMethodNode.GetSingleIdentifier(FilterName).Parent.Parent;
        var collectionTypeNode = mainTestMethodNode.GetIdentifiers(MqlGeneratorTemplateType).ElementAt(0);

        return new MqlGeneratorTestMethodTemplate(root, classDeclarationSyntax, mainTestMethodNode, builderDefinitionNode, collectionTypeNode, AnalysisType.Builders);
    }

    public (string newMethodName, MethodDeclarationSyntax newMethodDeclaration) GenerateMqlGeneratorMethod(string typeArgumentName, SyntaxNode buildersExpression)
    {
        var newMethodDeclaration = _testMethodTemplate.TestMethodNode.ReplaceNodes(_testMethodTemplate.NodesToReplace, (n, _) =>
            n switch
            {
                _ when n == _testMethodTemplate.ExpressionNode => buildersExpression,
                _ when n == _testMethodTemplate.TypeNode => SyntaxFactory.IdentifierName(typeArgumentName),
                _ => throw new Exception($"Unrecognized node {n}")
            });

        var newMqlGeneratorMethodName = $"{_testMethodTemplate.TestMethodNode.Identifier.Value}_{_nextTestMethodIndex++}";
        newMethodDeclaration = newMethodDeclaration.WithIdentifier(SyntaxFactory.Identifier(newMqlGeneratorMethodName));

        return (newMqlGeneratorMethodName, newMethodDeclaration);
    }

    public SyntaxTree GenerateSyntaxTree() =>
        _testMethodTemplate.Root.ReplaceNode(_testMethodTemplate.ClassDeclarationSyntax, _mqlGeneratorDeclarationSyntaxNew).SyntaxTree;
}
