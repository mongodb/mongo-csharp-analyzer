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

using static MongoDB.Analyzer.Core.HelperResources.MqlGeneratorSyntaxElements.Builders;
namespace MongoDB.Analyzer.Core.Builders;

internal sealed class BuildersMqlGeneratorTemplateBuilder
{
    internal record SyntaxElements(
        SyntaxNode Root,
        ClassDeclarationSyntax ClassDeclarationSyntax,
        MethodDeclarationSyntax TestMethodNode,
        SyntaxNode BuilderDefinitionNode,
        SyntaxNode CollectionTypeNode)
    {
        public SyntaxNode[] NodesToReplace { get; } = new[] { BuilderDefinitionNode, CollectionTypeNode };
    }

    private readonly SyntaxElements _syntaxElements;

    private ClassDeclarationSyntax _mqlGeneratorDeclarationSyntaxNew;
    private int _nextTestMethodIndex;

    public BuildersMqlGeneratorTemplateBuilder(SyntaxElements syntaxElements)
    {
        _syntaxElements = syntaxElements;
        _mqlGeneratorDeclarationSyntaxNew = _syntaxElements.ClassDeclarationSyntax;
    }

    public string AddBuildersExpression(string typeArgumentName, SyntaxNode buildersExpression)
    {
        var newMethodDeclaration = _syntaxElements.TestMethodNode.ReplaceNodes(_syntaxElements.NodesToReplace, (n, _) =>
        n switch
        {
            _ when n == _syntaxElements.BuilderDefinitionNode => buildersExpression,
            _ when n == _syntaxElements.CollectionTypeNode => SyntaxFactory.IdentifierName(typeArgumentName),
            _ => throw new Exception($"Unrecognized node {n}")
        });

        var newMqlGeneratorMethodName = $"{_syntaxElements.TestMethodNode.Identifier.Value}_{_nextTestMethodIndex++}";
        newMethodDeclaration = newMethodDeclaration.WithIdentifier(SyntaxFactory.Identifier(newMqlGeneratorMethodName));

        _mqlGeneratorDeclarationSyntaxNew = _mqlGeneratorDeclarationSyntaxNew.AddMembers(newMethodDeclaration);
        return newMqlGeneratorMethodName;
    }

    public SyntaxTree GenerateSyntaxTree() =>
        _syntaxElements.Root.ReplaceNode(_syntaxElements.ClassDeclarationSyntax, _mqlGeneratorDeclarationSyntaxNew).SyntaxTree;

    public static SyntaxElements CreateSyntaxElements(SyntaxTree mqlGeneratorSyntaxTree)
    {
        var root = mqlGeneratorSyntaxTree.GetRoot();

        var classDeclarationSyntax = root.GetSingleClassDeclaration(MqlGenerator);
        var mainTestMethodNode = classDeclarationSyntax.GetSingleMethod(MqlGeneratorMainMethodName);
        var builderDefinitionNode = mainTestMethodNode.GetSingleIdentifier(FilterName).Parent.Parent;
        var collectionTypeNode = mainTestMethodNode.GetIdentifiers(MqlGeneratorTemplateType).ElementAt(0);

        return new SyntaxElements(root, classDeclarationSyntax, mainTestMethodNode, builderDefinitionNode, collectionTypeNode);
    }
}
