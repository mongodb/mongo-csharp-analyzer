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
using static MongoDB.Analyzer.Core.HelperResources.ResourcesUtilities;

namespace MongoDB.Analyzer.Core.Poco;

internal static class AnalysisCodeGenerator
{
    private static readonly SyntaxTree[] s_helpersSyntaxTrees;
    private static readonly PocoJsonGeneratorTemplateBuilder.SyntaxElements s_jsonGeneratorSyntaxElements;
    private static readonly ParseOptions s_parseOptions;

    private static readonly string s_projectParentFolderPrefix = Path.Combine("..", "..", "..", "..", "..");
    private static string PocoAnalysisAssemblyPath { get; } = GetFullPathRelativeToParent("src", "MongoDB.Analyzer.Helpers", "Poco", "PropertyAndFieldHandler.cs");
    private static MetadataReference s_pocoPopulationMetadataReference { get; set; }

    static AnalysisCodeGenerator()
    {
        s_helpersSyntaxTrees = GetCommonCodeResources();
        var jsonGeneratorSyntaxTree = GetCodeResource(ResourceNames.Poco.JsonGenerator);
        s_jsonGeneratorSyntaxElements = PocoJsonGeneratorTemplateBuilder.CreateSyntaxElements(jsonGeneratorSyntaxTree);
        s_parseOptions = jsonGeneratorSyntaxTree.Options;
    }

    public static CompilationResult Compile(MongoAnalysisContext context, ExpressionsAnalysis pocoExpressionAnalysis)
    {
        var semanticModel = context.SemanticModelAnalysisContext.SemanticModel;
        var referencesContainer = ReferencesProvider.GetReferences(semanticModel.Compilation.References, context.Logger);
        if (referencesContainer == null)
        {
            return CompilationResult.Failure;
        }

        var metadataReferences = referencesContainer.References.ToList();
        CompilePocoPopulationCode(metadataReferences);
        referencesContainer = new ReferencesContainer(metadataReferences.ToArray(), referencesContainer.DriverPaths, referencesContainer.Version);

        var typesSyntaxTree = TypesGeneratorHelper.GenerateTypesSyntaxTree(AnalysisType.Poco, pocoExpressionAnalysis.TypesDeclarations, s_parseOptions);
        var jsonGeneratorSyntaxTree = GenerateJsonGeneratorSyntaxTree(pocoExpressionAnalysis);

        var syntaxTrees = new List<SyntaxTree>(s_helpersSyntaxTrees)
            {
                typesSyntaxTree,
                jsonGeneratorSyntaxTree
            };

        var pocoTestCodeExecutor = AnalysisCodeGeneratorUtilities.GetCodeExecutor<PocoJsonGeneratorExecutor>(context, referencesContainer, syntaxTrees.ToArray(), AnalysisType.Poco);

        var result = new CompilationResult(
            pocoTestCodeExecutor != null,
            pocoTestCodeExecutor,
            referencesContainer.Version);

        return result;
    }

    public static SyntaxTree GenerateJsonGeneratorSyntaxTree(ExpressionsAnalysis pocoExpressionAnalysis)
    {
        var testCodeBuilder = new PocoJsonGeneratorTemplateBuilder(s_jsonGeneratorSyntaxElements);

        foreach (var pocoContext in pocoExpressionAnalysis.AnalysisNodeContexts)
        {
            pocoContext.EvaluationMethodName = testCodeBuilder.AddPoco(pocoContext.Node.RewrittenExpression as ClassDeclarationSyntax);
        }

        return testCodeBuilder.GenerateSyntaxTree();
    }

    private static void CompilePocoPopulationCode(List<MetadataReference> metadataReferences)
    {
        if (s_pocoPopulationMetadataReference == null)
        {
            var staticCompilation = CSharpCompilation.Create(
                PocoAnalysisConstants.PropertyAndFieldHandlerAssemblyName,
                new List<SyntaxTree>() { CSharpSyntaxTree.ParseText(File.ReadAllText(PocoAnalysisAssemblyPath)) },
                new List<MetadataReference>(metadataReferences),
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var memoryStream = new MemoryStream();
            var emitResult = staticCompilation.Emit(memoryStream);

            if (emitResult.Success)
            {
                var rawAssembly = memoryStream.ToArray();
                s_pocoPopulationMetadataReference = MetadataReference.CreateFromImage(rawAssembly);
                metadataReferences.Add(s_pocoPopulationMetadataReference);
                AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
                    new AssemblyName(args.Name).Name == PocoAnalysisConstants.PropertyAndFieldHandlerAssemblyName ?
                    Assembly.Load(rawAssembly) : null;
            }
        }
        else
        {
            metadataReferences.Add(s_pocoPopulationMetadataReference);
        }
    }

    private static string GetFullPathRelativeToParent(params string[] pathComponents) =>
        Path.GetFullPath(Path.Combine(s_projectParentFolderPrefix, pathComponents.Length == 1 ? pathComponents[0] : Path.Combine(pathComponents)));
}