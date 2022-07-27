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

namespace MongoDB.Analyzer.Core.Builders;

internal sealed class BuildersMqlGeneratorTemplateBuilder
{
    private readonly SyntaxNode _root;
    private readonly ClassDeclarationSyntax _mqlGeneratorDeclarationSyntax;
    private readonly MethodDeclarationSyntax _mainTestMethodNode;
    private readonly SyntaxNode _builderDefinitionNode;

    private ClassDeclarationSyntax _mqlGeneratorDeclarationSyntaxNew;

    private int _nextTestMethodIndex;

    public BuildersMqlGeneratorTemplateBuilder(SyntaxTree mqlGeneratorSyntaxTree)
    {
        _root = mqlGeneratorSyntaxTree.GetRoot();

        _mqlGeneratorDeclarationSyntax = _root.GetSingleClassDeclaration(MqlGeneratorSyntaxElements.MqlGenerator);
        _mqlGeneratorDeclarationSyntaxNew = _mqlGeneratorDeclarationSyntax;
        _mainTestMethodNode = _mqlGeneratorDeclarationSyntax.GetSingleMethod(MqlGeneratorSyntaxElements.MqlGeneratorMainMethodName);

        _builderDefinitionNode = _mainTestMethodNode.DescendantNodes()
            .OfType<IdentifierNameSyntax>()
            .Single(i => i.Identifier.Text == "Filter").Parent.Parent.Parent;
    }

    public string AddBuildersExpression(string typeArgumentName, SyntaxNode buildersExpression)
    {
        var newMethodDeclaration = _mainTestMethodNode.ReplaceNode(_builderDefinitionNode, buildersExpression);

        var newMqlGeneratorMethodName = $"{_mainTestMethodNode.Identifier.Value}_{ _nextTestMethodIndex++}";
        newMethodDeclaration = newMethodDeclaration.WithIdentifier(SyntaxFactory.Identifier(newMqlGeneratorMethodName));

        _mqlGeneratorDeclarationSyntaxNew = _mqlGeneratorDeclarationSyntaxNew.AddMembers(newMethodDeclaration);

        return newMqlGeneratorMethodName;
    }

    public SyntaxTree GenerateSyntaxTree() =>
        _root.ReplaceNode(_mqlGeneratorDeclarationSyntax, _mqlGeneratorDeclarationSyntaxNew).SyntaxTree;
}
