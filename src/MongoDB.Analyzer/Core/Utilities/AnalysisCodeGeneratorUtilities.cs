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

using MongoDB.Analyzer.Core.HelperResources;

namespace MongoDB.Analyzer.Core.Utilities;

internal static class AnalysisCodeGeneratorUtilities
{
    private const string AnalysisAssemblyNameDRIVER_2_27_OR_Lower = "DynamicProxyGenAssembly2";
    private const string AnalysisAssemblyNameDRIVER_2_28_OR_Greater = "MongoDB.Analyzer.MQLGenerator";

    public static Type CompileAndGetGeneratorType(AnalysisType analysisType, MongoAnalysisContext context, ReferencesContainer referencesContainer, IEnumerable<SyntaxTree> syntaxTrees)
    {
        var publicKey = new byte[160] { 0, 36, 0, 0, 4, 128, 0, 0, 148, 0, 0, 0, 6, 2, 0, 0, 0, 36, 0, 0, 82, 83, 65, 49, 0, 4, 0, 0, 1, 0, 1, 0, 53, 40, 127, 13, 56, 131, 192, 160, 117, 200, 142, 12, 218, 60, 233, 59, 98, 16, 3, 236, 189, 94, 146, 13, 74, 140, 114, 56, 86, 79, 77, 47, 79, 104, 17, 106, 202, 40, 201, 178, 19, 65, 220, 58, 135, 118, 121, 193, 69, 86, 25, 43, 43, 47, 95, 226, 193, 29, 98, 78, 8, 148, 211, 8, 255, 123, 148, 191, 111, 215, 42, 239, 27, 65, 1, 127, 254, 37, 114, 233, 144, 25, 209, 198, 25, 99, 230, 140, 208, 237, 103, 115, 74, 66, 203, 51, 59, 128, 142, 56, 103, 203, 230, 49, 147, 114, 20, 227, 46, 64, 159, 177, 250, 98, 253, 182, 157, 73, 76, 37, 48, 230, 74, 64, 228, 23, 214, 238 };
        var compilation = CSharpCompilation.Create(AnalysisAssemblyNameDRIVER_2_28_OR_Greater, syntaxTrees, referencesContainer.References, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, publicSign: true, cryptoPublicKey: publicKey.ToImmutableArray()));

        using var memoryStream_Driver_2_28_Or_Greater = new MemoryStream();
        var emitResult = compilation.Emit(memoryStream_Driver_2_28_Or_Greater);

        Type generatorType = null;

        if (emitResult.Success)
        {
            context.Logger.Log("Compilation successful for AnalysisAssemblyName for 2.28 Driver Versions and Above");

            memoryStream_Driver_2_28_Or_Greater.Seek(0, SeekOrigin.Begin);

            generatorType = DynamicTypeProvider.GetType(referencesContainer, memoryStream_Driver_2_28_Or_Greater, GetGeneratorFullName(analysisType));
        }
        else
        {
            context.Logger.Log($"Compilation failed with AnalysisAssemblyName for 2.28 Driver Versions and Above : {string.Join(Environment.NewLine, emitResult.Diagnostics)}");

            compilation = CSharpCompilation.Create(AnalysisAssemblyNameDRIVER_2_27_OR_Lower, syntaxTrees, referencesContainer.References, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, publicSign: true, cryptoPublicKey: publicKey.ToImmutableArray()));

            using var memoryStream_Driver_2_27_Or_Lower = new MemoryStream();
            emitResult = compilation.Emit(memoryStream_Driver_2_27_Or_Lower);

            if (emitResult.Success)
            {
                context.Logger.Log("Compilation successful for AnalysisAssemblyName with 2.27 Driver Versions And Below");

                memoryStream_Driver_2_27_Or_Lower.Seek(0, SeekOrigin.Begin);

                generatorType = DynamicTypeProvider.GetType(referencesContainer, memoryStream_Driver_2_27_Or_Lower, GetGeneratorFullName(analysisType));

            }
            else
            {
                context.Logger.Log($"Compilation failed with AnalysisAssemblyName for 2.27 Driver Versions And Below : {string.Join(Environment.NewLine, emitResult.Diagnostics)}");
            }
        }

        return generatorType;
    }

    private static string GetGeneratorFullName(AnalysisType analysisType) =>
        analysisType switch
        {
            AnalysisType.Builders => MqlGeneratorSyntaxElements.Builders.MqlGeneratorFullName,
            AnalysisType.Linq => MqlGeneratorSyntaxElements.Linq.MqlGeneratorFullName,
            AnalysisType.Poco => JsonSyntaxElements.Poco.JsonGeneratorFullName,
            _ => throw new ArgumentOutOfRangeException(nameof(analysisType), analysisType, "Unsupported analysis type")
        };
}

