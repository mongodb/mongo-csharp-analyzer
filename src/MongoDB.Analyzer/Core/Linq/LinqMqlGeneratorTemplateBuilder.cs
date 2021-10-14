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

internal sealed class LinqMqlGeneratorTemplateBuilder
{
    private readonly SyntaxNode _root;
    private readonly ClassDeclarationSyntax _mqlGeneratorDeclarationSyntax;
    private readonly MethodDeclarationSyntax _mainTestMethodNode;
    private readonly SyntaxNode[] _nodesToReplace;

    private ClassDeclarationSyntax _mqlGeneratorDeclarationSyntaxNew;

    private int _nextTestMethodIndex;

    public LinqMqlGeneratorTemplateBuilder(SyntaxTree mqlGeneratorSyntaxTree, bool isLinq3)
    {
        _root = mqlGeneratorSyntaxTree.GetRoot();

        if (isLinq3)
        {
            var linqProviderIdentifier = _root.GetSingleIdentifier(MqlGeneratorSyntaxElements.IQueryableProviderV2);
            var linqProviderIdentifierNew = linqProviderIdentifier.WithIdentifier(SyntaxFactory.Identifier(MqlGeneratorSyntaxElements.IQueryableProviderV3));

            _root = _root.ReplaceNode(linqProviderIdentifier, linqProviderIdentifierNew);
        }

        _mqlGeneratorDeclarationSyntax = _root.GetSingleClassDeclaration(MqlGeneratorSyntaxElements.MqlGenerator);
        _mqlGeneratorDeclarationSyntaxNew = _mqlGeneratorDeclarationSyntax;
        _mainTestMethodNode = _mqlGeneratorDeclarationSyntax.GetSingleMethod(MqlGeneratorSyntaxElements.MqlGeneratorMainMethodName);
        var queryableTypeNode = _mainTestMethodNode.GetSingleIdentifier(MqlGeneratorSyntaxElements.MqlGeneratorTemplateType);

        var linqExpressionEntryNode = _mainTestMethodNode.DescendantNodes()
            .OfType<IdentifierNameSyntax>()
            .Single(i => i.Identifier.Text == "Where").Parent.Parent;

        _nodesToReplace = new[] { queryableTypeNode, linqExpressionEntryNode };
    }

    public string AddLinqExpression(string collectionTypeName, SyntaxNode linqExpression)
    {
        var newMethodDeclaration = _mainTestMethodNode.ReplaceNodes(_nodesToReplace, (n, _) =>
            n.Kind() switch
            {
                SyntaxKind.InvocationExpression => linqExpression,
                SyntaxKind.IdentifierName => SyntaxFactory.IdentifierName(collectionTypeName),
                _ => throw new Exception($"Unrecognized node {n}")
            });

        var newMqlGeneratorMethodName = $"{_mainTestMethodNode.Identifier.Value}_{ _nextTestMethodIndex++}";
        newMethodDeclaration = newMethodDeclaration.WithIdentifier(SyntaxFactory.Identifier(newMqlGeneratorMethodName));

        _mqlGeneratorDeclarationSyntaxNew = _mqlGeneratorDeclarationSyntaxNew.AddMembers(newMethodDeclaration);

        return newMqlGeneratorMethodName;
    }

    public SyntaxTree GenerateSyntaxTree() =>
        _root.ReplaceNode(_mqlGeneratorDeclarationSyntax, _mqlGeneratorDeclarationSyntaxNew).SyntaxTree;
}
