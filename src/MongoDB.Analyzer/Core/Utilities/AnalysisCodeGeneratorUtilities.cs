﻿// Copyright 2021-present MongoDB Inc.
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

using MongoDB.Analyzer.Core.Builders;
using MongoDB.Analyzer.Core.HelperResources;
using MongoDB.Analyzer.Core.Json;
using MongoDB.Analyzer.Core.Linq;

namespace MongoDB.Analyzer.Core.Utilities
{
    internal static class AnalysisCodeGeneratorUtilities
    {
        private record LinqContext(
            bool IsLinq3,
            bool IsLinq3Default,
            LinqVersion DefaultLinqVersion);

        public static GeneratorExecutor GetCodeExecutor(MongoAnalysisContext context, AnalysisType analysisType, SyntaxTree[] syntaxTrees)
        {
            var semanticModel = context.SemanticModelAnalysisContext.SemanticModel;
            var referencesContainer = ReferencesProvider.GetReferences(semanticModel.Compilation.References, context.Logger);

            var compilation = CSharpCompilation.Create(
                GetAnalysisAssemblyName(analysisType),
                syntaxTrees,
                referencesContainer.References,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var linqContext = analysisType == AnalysisType.Linq ? GenerateLinqContext(context) : null;

            using var memoryStream = new MemoryStream();
            var emitResult = compilation.Emit(memoryStream);
            GeneratorExecutor codeExecutor = null;

            if (emitResult.Success)
            {
                context.Logger.Log("Compilation successful");

                memoryStream.Seek(0, SeekOrigin.Begin);

                var generatorType = DynamicTypeProvider.GetType(referencesContainer, memoryStream, GetGeneratorFullName(analysisType));
                codeExecutor = GetGeneratorExecutor(analysisType, linqContext, generatorType);
            }
            else
            {
                context.Logger.Log($"Compilation failed with: {string.Join(Environment.NewLine, emitResult.Diagnostics)}");
            }

            return codeExecutor;
        }

        private static LinqContext GenerateLinqContext(MongoAnalysisContext context)
        {
            var semanticModel = context.SemanticModelAnalysisContext.SemanticModel;
            var referencesContainer = ReferencesProvider.GetReferences(semanticModel.Compilation.References, context.Logger);

            var isLinq3 = referencesContainer.Version >= LinqAnalysisConstants.MinLinq3Version;
            var isLinq3Default = referencesContainer.Version >= LinqAnalysisConstants.DefaultLinq3Version;
            var defaultLinqVersion = context.Settings.DefaultLinqVersion ?? (isLinq3Default ? LinqVersion.V3 : LinqVersion.V2);

            return new LinqContext(isLinq3, isLinq3Default, defaultLinqVersion); 
        }

        private static string GetAnalysisAssemblyName(AnalysisType analysisType) =>
            analysisType switch
            {
                AnalysisType.Builders => BuildersAnalysisConstants.AnalysisAssemblyName,
                AnalysisType.Linq => LinqAnalysisConstants.AnalysisAssemblyName,
                AnalysisType.Poco => JsonAnalysisConstants.AnalysisAssemblyName,
                _ => throw new Exception("Unsupported Analysis Type")
            };

        private static GeneratorExecutor GetGeneratorExecutor(AnalysisType analysisType, LinqContext linqContext, Type generatorType) =>
            analysisType switch
            {
                AnalysisType.Builders => new BuildersMqlGeneratorExecutor(generatorType),
                AnalysisType.Linq => new LinqMqlGeneratorExecutor(generatorType, linqContext.IsLinq3 ? LinqVersion.V3 : LinqVersion.V2, linqContext.DefaultLinqVersion),
                AnalysisType.Poco => new JsonGeneratorExecutor(generatorType),
                _ => throw new Exception("Unsupported Analysis Type")
            };

        private static string GetGeneratorFullName(AnalysisType analysisType) =>
            analysisType switch
            {
                AnalysisType.Builders => MqlGeneratorSyntaxElements.Builders.MqlGeneratorFullName,
                AnalysisType.Linq => MqlGeneratorSyntaxElements.Linq.MqlGeneratorFullName,
                AnalysisType.Poco => JsonSyntaxElements.Json.JsonGeneratorFullName,
                _ => throw new Exception("Unsupported Analysis Type")
            };
    }
}

