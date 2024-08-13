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
using static MongoDB.Analyzer.Core.HelperResources.JsonSyntaxElements.Poco;

namespace MongoDB.Analyzer.Core.Poco;

internal sealed class PocoJsonGeneratorTemplateBuilder
{
    private readonly MqlGeneratorTestMethodTemplate _testMethodTemplate;
    private ClassDeclarationSyntax _jsonGeneratorDeclarationSyntaxNew;
    private int _nextTestMethodIndex;

    public PocoJsonGeneratorTemplateBuilder(MqlGeneratorTestMethodTemplate syntaxElements)
    {
        _testMethodTemplate = syntaxElements;
        _jsonGeneratorDeclarationSyntaxNew = _testMethodTemplate.ClassDeclarationSyntax;
    }

    public void AddJsonGeneratorMethods(MemberDeclarationSyntax[] methodDeclarations) =>
        _jsonGeneratorDeclarationSyntaxNew = _jsonGeneratorDeclarationSyntaxNew.AddMembers(methodDeclarations);

    public static MqlGeneratorTestMethodTemplate CreateTestMethodTemplate(SyntaxTree jsonGeneratorSyntaxTree)
    {
        var root = jsonGeneratorSyntaxTree.GetRoot();
        var classDeclarationSyntax = root.GetSingleClassDeclaration(JsonGenerator);
        var mainTestMethodNode = classDeclarationSyntax.GetSingleMethod(JsonGeneratorMainMethodName);

        var localDeclaration = mainTestMethodNode.DescendantNodes().OfType<LocalDeclarationStatementSyntax>().FirstOrDefault();
        var predefinedType = localDeclaration.DescendantNodes().OfType<PredefinedTypeSyntax>().FirstOrDefault();

        return new(root, classDeclarationSyntax, mainTestMethodNode, null, predefinedType, AnalysisType.Poco);
    }

    public (string newMethodName, MethodDeclarationSyntax newMethodDeclaration) GenerateJsonGeneratorMethod(ClassDeclarationSyntax poco)
    {
        var newMethodDeclaration = _testMethodTemplate.TestMethodNode.ReplaceNode(_testMethodTemplate.TypeNode, SyntaxFactory.IdentifierName(poco.Identifier.ValueText));
        var newJsonGeneratorMethodName = $"{_testMethodTemplate.TestMethodNode.Identifier.Value}_{_nextTestMethodIndex++}";
        newMethodDeclaration = newMethodDeclaration.WithIdentifier(SyntaxFactory.Identifier(newJsonGeneratorMethodName));
        return (newJsonGeneratorMethodName, newMethodDeclaration);
    }

    public SyntaxTree GenerateSyntaxTree() =>
        _testMethodTemplate.Root.ReplaceNode(_testMethodTemplate.ClassDeclarationSyntax, _jsonGeneratorDeclarationSyntaxNew).SyntaxTree;
}