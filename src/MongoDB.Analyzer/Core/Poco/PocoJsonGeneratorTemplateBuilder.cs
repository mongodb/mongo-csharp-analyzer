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

using static MongoDB.Analyzer.Core.HelperResources.JsonSyntaxElements.Poco;

namespace MongoDB.Analyzer.Core.Poco;

internal sealed class PocoJsonGeneratorTemplateBuilder
{
    internal record SyntaxElements(
        SyntaxNode Root,
        ClassDeclarationSyntax ClassDeclarationSyntax,
        MethodDeclarationSyntax TestMethodNode,
        PredefinedTypeSyntax PredefinedTypeNode)
    {
        public SyntaxNode[] NodesToReplace { get; } = new[] { PredefinedTypeNode };
    }

    private readonly SyntaxElements _syntaxElements;
    private ClassDeclarationSyntax _jsonGeneratorDeclarationSyntaxNew;
    private int _nextTestMethodIndex;

    public PocoJsonGeneratorTemplateBuilder(SyntaxElements syntaxElements)
    {
        _syntaxElements = syntaxElements;
        _jsonGeneratorDeclarationSyntaxNew = _syntaxElements.ClassDeclarationSyntax;
    }

    public string AddPoco(ClassDeclarationSyntax poco)
    {
        var newMethodDeclaration = _syntaxElements.TestMethodNode.ReplaceNode(_syntaxElements.PredefinedTypeNode, SyntaxFactory.IdentifierName(poco.Identifier.ValueText));
        var newJsonGeneratorMethodName = $"{_syntaxElements.TestMethodNode.Identifier.Value}_{_nextTestMethodIndex++}";
        newMethodDeclaration = newMethodDeclaration.WithIdentifier(SyntaxFactory.Identifier(newJsonGeneratorMethodName));

        _jsonGeneratorDeclarationSyntaxNew = _jsonGeneratorDeclarationSyntaxNew.AddMembers(newMethodDeclaration);
        return newJsonGeneratorMethodName;
    }

    public SyntaxTree GenerateSyntaxTree() =>
        _syntaxElements.Root.ReplaceNode(_syntaxElements.ClassDeclarationSyntax, _jsonGeneratorDeclarationSyntaxNew).SyntaxTree;

    public static SyntaxElements CreateSyntaxElements(SyntaxTree jsonGeneratorSyntaxTree)
    {
        var root = jsonGeneratorSyntaxTree.GetRoot();
        var classDeclarationSyntax = root.GetSingleClassDeclaration(JsonGenerator);
        var mainTestMethodNode = classDeclarationSyntax.GetSingleMethod(JsonGeneratorMainMethodName);

        var localDeclaration = mainTestMethodNode.DescendantNodes().OfType<LocalDeclarationStatementSyntax>().FirstOrDefault();
        var predefinedType = localDeclaration.DescendantNodes().OfType<PredefinedTypeSyntax>().FirstOrDefault();

        return new SyntaxElements(root, classDeclarationSyntax, mainTestMethodNode, predefinedType);
    }
}