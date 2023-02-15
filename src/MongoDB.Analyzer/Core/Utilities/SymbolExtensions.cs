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
    private const string AssemblyMongoDBDriver = "MongoDB.Driver";
    private const string NamespaceMongoDBDriver = "MongoDB.Driver";
    private const string NamespaceMongoDBLinq = "MongoDB.Driver.Linq";
    private const string NamespaceSystemLinq = "System.Linq";

    private static readonly HashSet<string> s_supportedCollections = new()
    {
        "System.Collections.Generic.List<T>",
        "System.Collections.Generic.IList<T>",
        "System.Collections.Generic.IEnumerable<T>"
    };

    public static IMethodSymbol GetMethodSymbol(this SyntaxNode node, SemanticModel semanticModel) =>
        semanticModel.GetSymbolInfo(node).Symbol as IMethodSymbol;

    public static bool IsContainedInLambda(this ISymbol symbol, SyntaxNode parentNode)
    {
        var isContainedInLambda = (symbol?.ContainingSymbol is IMethodSymbol methodSymbol && methodSymbol.MethodKind == MethodKind.AnonymousFunction);
        var result = isContainedInLambda && symbol.DeclaringSyntaxReferences.Any(d => parentNode.Contains(d.GetSyntax()));

        return result;
    }

    public static bool IsContainedInLambdaOrQueryParameter(this ISymbol symbol, SyntaxNode parentNode) =>
        symbol switch
        {
            IRangeVariableSymbol => symbol.DeclaringSyntaxReferences.Any(d => parentNode.Contains(d.GetSyntax())),
            _ => symbol.IsContainedInLambda(parentNode)
        };

    public static bool IsDefinedInMongoLinqOrSystemLinq(this ISymbol symbol)
    {
        var containingNamespace = symbol?.ContainingNamespace?.ToDisplayString();

        // In case of system linq, the containing module is not validated for simplicity,
        // as it can differ in different .net frameworks.
        return containingNamespace == NamespaceSystemLinq ||
            containingNamespace == NamespaceMongoDBLinq &&
            symbol?.ContainingAssembly.Name == AssemblyMongoDBDriver;
    }

    public static bool IsBuilder(this ITypeSymbol typeSymbol) =>
        typeSymbol?.Name switch
        {
            "FilterDefinitionBuilder" or
            "IndexKeysDefinitionBuilder" or
            "IndexKeysDefinitionExtensions" or
            "ProjectionDefinitionBuilder" or
            "ProjectionDefinitionExtensions" or
            "PipelineDefinitionBuilder" or
            "SearchDefinitionBuilder" or
            "SortDefinitionBuilder" or
            "SortDefinitionExtensions" or
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
            "ProjectionDefinition" or
            "PipelineDefinition" or
            "SearchDefinition" or
            "SortDefinition" or
            "UpdateDefinition" => true,
            _ => false
        };

    public static bool IsFindFluent(this ITypeSymbol typeSymbol) =>
        typeSymbol?.Name switch
        {
            "IFindFluent" or
            "IOrderedFindFluent" => true,
            _ => false
        };

    public static bool IsFindFluentMethod(this IMethodSymbol methodSymbol) =>
        methodSymbol != null &&
        (methodSymbol.ReceiverType.IsIMongoCollection() || methodSymbol.ReceiverType.IsFindFluent()) &&
        methodSymbol.ReturnType.IsFindFluent();

    public static bool IsFindOptions(this ITypeSymbol namedTypeSymbol) =>
       namedTypeSymbol?.Name == "FindOptions" &&
       namedTypeSymbol?.ContainingAssembly.Name == AssemblyMongoDBDriver;

    public static bool IsIMongoCollection(this ITypeSymbol typeSymbol) => ImplementsOrIsInterface(typeSymbol, NamespaceMongoDBDriver, "IMongoCollection");

    public static bool IsIMongoQueryable(this ITypeSymbol typeSymbol) =>
        ImplementsOrIsInterface(typeSymbol, NamespaceMongoDBLinq, "IMongoQueryable") ||
        ImplementsOrIsInterface(typeSymbol, NamespaceMongoDBLinq, "IOrderedMongoQueryable");

    public static bool IsIQueryable(this ITypeSymbol typeSymbol) =>
        ImplementsOrIsInterface(typeSymbol, NamespaceSystemLinq, nameof(IQueryable));

    public static bool IsMongoQueryable(this ITypeSymbol typeSymbol) =>
        typeSymbol?.Name == "MongoQueryable";

    public static bool IsSupportedCollection(this ITypeSymbol typeSymbol) =>
        typeSymbol is INamedTypeSymbol namedTypeSymbol &&
        s_supportedCollections.Contains(namedTypeSymbol.ConstructedFrom?.ToDisplayString());

    public static bool IsSupportedMongoCollectionType(this ITypeSymbol typeSymbol) =>
        typeSymbol.TypeKind == TypeKind.Class &&
        !typeSymbol.IsAnonymousType;

    public static bool IsString(this ITypeSymbol typeSymbol) =>
        typeSymbol?.SpecialType == SpecialType.System_String;

    public static bool IsSupportedBuilderType(this ITypeSymbol typeSymbol) =>
        typeSymbol?.TypeKind switch
        {
            TypeKind.Class or
            TypeKind.Enum or
            TypeKind.Struct => true,
            _ => false
        };

    public static bool IsSupportedIMongoCollection(this ITypeSymbol typeSymbol) =>
        typeSymbol.IsIMongoCollection() &&
        typeSymbol is INamedTypeSymbol namedType &&
        namedType.TypeArguments.Length == 1 &&
        namedType.TypeArguments[0].IsSupportedMongoCollectionType();

    private static bool ImplementsOrIsInterface(this ITypeSymbol typeSymbol, string @namespace, string interfaceName)
    {
        if (typeSymbol == null)
        {
            return false;
        }

        return IsType(typeSymbol) || typeSymbol.Interfaces.Any(IsType);

        bool IsType(ITypeSymbol typeSymbol) =>
            typeSymbol.Name == interfaceName && typeSymbol.ContainingNamespace.ToDisplayString() == @namespace;
    }
}
