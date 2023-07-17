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

using static MongoDB.Analyzer.Core.HelperResources.MqlGeneratorSyntaxElements.Linq;

namespace MongoDB.Analyzer.Core.Linq;

internal sealed class LinqMqlGeneratorTemplateBuilder
{
    internal record SyntaxElements(
      SyntaxNode Root,
      ClassDeclarationSyntax ClassDeclarationSyntax,
      MethodDeclarationSyntax TestMethodNode,
      SyntaxNode LinqExpressionNode,
      SyntaxNode QueryableTypeNode)
    {
        public SyntaxNode[] NodesToReplace { get; } = new[] { LinqExpressionNode, QueryableTypeNode };
    }

    private readonly SyntaxElements _syntaxElements;
    private ClassDeclarationSyntax _mqlGeneratorDeclarationSyntaxNew;
    private int _nextTestMethodIndex;

    public LinqMqlGeneratorTemplateBuilder(SyntaxElements syntaxElements)
    {
        _syntaxElements = syntaxElements;
        _mqlGeneratorDeclarationSyntaxNew = _syntaxElements.ClassDeclarationSyntax;
    }

    public string AddLinqExpression(string collectionTypeName, SyntaxNode linqExpression)
    {
        var newMethodDeclaration = _syntaxElements.TestMethodNode.ReplaceNodes(_syntaxElements.NodesToReplace, (n, _) =>
            n.Kind() switch
            {
                _ when n == _syntaxElements.LinqExpressionNode => linqExpression,
                _ when n == _syntaxElements.QueryableTypeNode => SyntaxFactory.IdentifierName(collectionTypeName),
                _ => throw new Exception($"Unrecognized node {n}")
            });

        var newMqlGeneratorMethodName = $"{_syntaxElements.TestMethodNode.Identifier.Value}_{ _nextTestMethodIndex++}";
        newMethodDeclaration = newMethodDeclaration.WithIdentifier(SyntaxFactory.Identifier(newMqlGeneratorMethodName));

        _mqlGeneratorDeclarationSyntaxNew = _mqlGeneratorDeclarationSyntaxNew.AddMembers(newMethodDeclaration);

        return newMqlGeneratorMethodName;
    }

    public SyntaxTree GenerateSyntaxTree() =>
         _syntaxElements.Root.ReplaceNode(_syntaxElements.ClassDeclarationSyntax, _mqlGeneratorDeclarationSyntaxNew).SyntaxTree;

    public static SyntaxElements CreateSyntaxElements(SyntaxTree mqlGeneratorSyntaxTree, bool isLinq3)
    {
        var root = mqlGeneratorSyntaxTree.GetRoot();

        if (isLinq3)
        {
            var linqProviderIdentifier = root.GetSingleIdentifier(IQueryableProviderV2);
            var linqProviderIdentifierNew = linqProviderIdentifier.WithIdentifier(SyntaxFactory.Identifier(IQueryableProviderV3));

            root = root.ReplaceNode(linqProviderIdentifier, linqProviderIdentifierNew);
        }

        var classDeclarationSyntax = root.GetSingleClassDeclaration(MqlGenerator);
        var mainTestMethodNode = classDeclarationSyntax.GetSingleMethod(MqlGeneratorMainMethodName);
        var queryableTypeNode = mainTestMethodNode.GetSingleIdentifier(MqlGeneratorTemplateType);
        var linqExpressionNode = mainTestMethodNode.GetSingleIdentifier(LinqMethodName).Parent.Parent;

        return new SyntaxElements(root, classDeclarationSyntax, mainTestMethodNode, linqExpressionNode, queryableTypeNode);
    }
}
