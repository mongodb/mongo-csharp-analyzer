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

namespace MongoDB.Analyzer.Core;

internal static class SymbolExtensions
{
    private static readonly HashSet<string> s_supportedCollections = new()
    {
        "System.Collections.Generic.List<T>",
        "System.Collections.Generic.IList<T>",
        "System.Collections.Generic.IEnumerable<T>"
    };

    public static bool IsDefinedInMongoLinq(this ISymbol symbol) =>
        symbol?.ContainingModule?.Name?.ToLowerInvariant() == "mongodb.driver.dll" &&
        symbol?.ContainingNamespace?.Name == "Linq";

    public static bool IsIMongoQueryable(this ITypeSymbol typeSymbol) =>
        ImplementsOrIsInterface(typeSymbol, "MongoDB.Driver.Linq", "IMongoQueryable") ||
        ImplementsOrIsInterface(typeSymbol, "MongoDB.Driver.Linq", "IOrderedMongoQueryable");

    public static bool IsSupportedBuilderType(this ITypeSymbol typeSymbol) =>
        (typeSymbol.TypeKind == TypeKind.Class ||
         typeSymbol.TypeKind == TypeKind.Struct ||
         typeSymbol.TypeKind == TypeKind.Enum) &&
        !typeSymbol.IsAnonymousType;

    public static bool IsSupportedCollection(this ITypeSymbol typeSymbol) =>
        typeSymbol is INamedTypeSymbol namedTypeSymbol &&
        s_supportedCollections.Contains(namedTypeSymbol.ConstructedFrom?.ToDisplayString());

    public static bool IsSupportedMongoCollectionType(this ITypeSymbol typeSymbol) =>
        typeSymbol.TypeKind == TypeKind.Class &&
        !typeSymbol.IsAnonymousType;

    public static bool IsMongoQueryable(this ITypeSymbol typeSymbol) =>
        typeSymbol?.Name == "MongoQueryable";

    public static bool IsBuilder(this ITypeSymbol typeSymbol) =>
        typeSymbol?.Name switch
        {
            "FilterDefinitionBuilder" or
            "IndexKeysDefinitionBuilder" or
            "SortDefinitionBuilder" or
            "SortDefinitionExtensions" or
            //"ProjectionDefinitionBuilder" or
            "UpdateDefinitionBuilder" => true,
            _ => false
        };

    public static bool IsBuilderMethod(this IMethodSymbol methodSymbol) =>
        methodSymbol != null &&
        (methodSymbol.ReceiverType.IsBuilder() ||
         (methodSymbol.ReducedFrom?.ReceiverType).IsBuilder());

    public static bool IsBuilderDefinition(this ITypeSymbol typeSymbol) =>
        typeSymbol?.Name switch
        {
            "FilterDefinition" or
            "IndexKeysDefinition" or
            "SortDefinition" or
            // "ProjectionDefinition" ors
            "UpdateDefinition" => true,
            _ => false
        };

    public static bool IsLinqEnumerable(this ITypeSymbol typeSymbol) =>
        typeSymbol?.Name == "Enumerable" && typeSymbol.ContainingNamespace.Name == "Linq";

    public static bool IsString(this ITypeSymbol typeSymbol) =>
        typeSymbol?.SpecialType == SpecialType.System_String;

    public static bool IsContainedInLambda(this ISymbol symbol, SyntaxNode parentNode)
    {
        var isContainedInLambda = (symbol?.ContainingSymbol is IMethodSymbol methodSymbol && methodSymbol.MethodKind == MethodKind.AnonymousFunction);
        var result = isContainedInLambda &&
            (symbol != null && symbol.DeclaringSyntaxReferences.Any(d => parentNode.Contains(d.GetSyntax())));

        return result;
    }

    private static bool ImplementsOrIsInterface(this ITypeSymbol typeSymbol, string @namespace, string interfaceName) =>
        typeSymbol?.TypeKind switch
        {
            TypeKind.Class => typeSymbol.Interfaces.Any(i => i.Name == interfaceName && i.ContainingNamespace.ToDisplayString() == @namespace),
            TypeKind.Interface => typeSymbol.Name == interfaceName && typeSymbol.ContainingNamespace.ToDisplayString() == @namespace,
            _ => false
        };
}
