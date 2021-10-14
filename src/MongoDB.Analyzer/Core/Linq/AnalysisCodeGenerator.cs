using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.IO;
using System.Reflection;

namespace MongoDB.Analyzer.Core.Linq
{
    internal record CompilationResult(bool Success, LinqMqlGeneratorExecutor LinqTestCodeExecutor, string MongoDBDriverVersion);

    internal static class AnalysisCodeGenerator
    {
        private static readonly NamespaceDeclarationSyntax s_namespaceDeclarationSyntax;
        private readonly static string s_mockCollectionCode;
        private static SyntaxTree s_mockCollectionSyntaxTree;

        static AnalysisCodeGenerator()
        {
            s_namespaceDeclarationSyntax = SyntaxFactory.
                NamespaceDeclaration(SyntaxFactory.ParseName(LinqAnalysisConsts.TestNamespace)).
                AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")),
                    SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("MongoDB.Bson")),
                    SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("MongoDB.Bson.Serialization.Attributes")));

            using var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(LinqAnalysisConsts.MockCollectionFilename);
            using var streamReader = new StreamReader(resourceStream);

            s_mockCollectionCode = streamReader.ReadToEnd();
        }

        public static CompilationResult Compile(SemanticModel semanticModel, ExpressionsAnalysis linqExpressionAnalysis, CSharpParseOptions parseOptions)
        {
            var typesSyntaxTree = GenerateTypesSyntaxTree(linqExpressionAnalysis, parseOptions);
            var testCodeSyntaxTree = GenerateTestCodeSyntaxTree(linqExpressionAnalysis, parseOptions);
            var mockCollectionSyntaxTree = GetMockCollectionSyntaxTree(parseOptions);

            var syntaxTrees = new SyntaxTree[]
            {
                typesSyntaxTree,
                testCodeSyntaxTree,
                mockCollectionSyntaxTree
            };

            var mongoDBreferences = ReferencesProvider.GetMetadataReferences(semanticModel.Compilation.References);

            var compilation = CSharpCompilation.Create(
                LinqAnalysisConsts.AnalysisAssemblyName,
                syntaxTrees,
                mongoDBreferences,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using var memoryStream = new MemoryStream();
            var emitResult = compilation.Emit(memoryStream);

            LinqMqlGeneratorExecutor linqTestCodeExecutor = null;
            string mongoVersion = null;

            if (emitResult.Success)
            {
                memoryStream.Seek(0, SeekOrigin.Begin);
                var assembly = Assembly.Load(memoryStream.ToArray());

                var testClassType = assembly.GetType(LinqAnalysisConsts.TestClassFullName);

                if (testClassType != null)
                {
                    linqTestCodeExecutor = new LinqMqlGeneratorExecutor(testClassType);
                }

                mongoVersion = ReferencesProvider.GetDriverVersion(assembly);
            }

            var result = new CompilationResult(
                linqTestCodeExecutor != null,
                linqTestCodeExecutor,
                mongoVersion);

            return result;
        }

        private static SyntaxTree GetMockCollectionSyntaxTree(CSharpParseOptions parseOptions)
        {
            var exsitingParseOptions = s_mockCollectionSyntaxTree?.Options as CSharpParseOptions;
            if (exsitingParseOptions?.LanguageVersion != parseOptions.LanguageVersion)
            {
                s_mockCollectionSyntaxTree = CSharpSyntaxTree.ParseText(s_mockCollectionCode, parseOptions);
            }

            return s_mockCollectionSyntaxTree;
        }

        private static SyntaxTree GenerateTestCodeSyntaxTree(ExpressionsAnalysis linqExpressionAnalysis, CSharpParseOptions parseOptions)
        {
            var testCodeBuilder = new LinqGeneratorTemplateBuilder();

            foreach (var linqContext in linqExpressionAnalysis.AnalysisNodeContexts)
            {
                var analysisNode = linqContext.Node;

                linqContext.EvaluationMethodName = testCodeBuilder.AddTest(
                    analysisNode.ArgumentTypeName,
                    analysisNode.RewrittenExpression.ToFullString());
            }

            var testCode = testCodeBuilder.Finalize();
            var syntaxTreeTestCode = CSharpSyntaxTree.ParseText(testCode, parseOptions);

            return syntaxTreeTestCode;
        }

        private static SyntaxTree GenerateTypesSyntaxTree(ExpressionsAnalysis linqExpressionAnalysis, CSharpParseOptions parseOptions)
        {
            var typesNamespace = s_namespaceDeclarationSyntax.AddMembers(linqExpressionAnalysis.TypesDeclarations);
            var typesCode = typesNamespace.NormalizeWhitespace().ToFullString();

            var typesSyntaxTree = CSharpSyntaxTree.ParseText(typesCode, parseOptions);
            return typesSyntaxTree;
        }
    }
}
