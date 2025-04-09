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
    private const string NamespaceEF = "Microsoft.EntityFrameworkCore";
    private const string NamespaceMongoDBBson = "MongoDB.Bson";
    private const string NamespaceMongoDBBsonAttributes = "MongoDB.Bson.Serialization.Attributes";
    private const string NamespaceMongoDBDriver = "MongoDB.Driver";
    private const string NamespaceMongoDBLinq = "MongoDB.Driver.Linq";
    private const string NamespaceSystem = "System";
    private const string NamespaceSystemLinq = "System.Linq";

    private static readonly HashSet<string> s_supportedBsonAttributes = new()
    {
        "BsonConstructorAttribute",
        "BsonDateTimeOptionsAttribute",
        "BsonDefaultValueAttribute",
        "BsonDiscriminatorAttribute",
        "BsonElementAttribute",
        "BsonExtraElementsAttribute",
        "BsonFactoryMethodAttribute",
        "BsonIdAttribute",
        "BsonIgnoreAttribute",
        "BsonIgnoreExtraElementsAttribute",
        "BsonIgnoreIfDefaultAttribute",
        "BsonIgnoreIfNullAttribute",
        "BsonKnownTypesAttribute",
        "BsonNoIdAttribute",
        "BsonRequiredAttribute",
        "BsonTimeSpanOptionsAttribute"
    };

    private static readonly HashSet<string> s_supportedBsonTypes = new()
    {
        "MongoDB.Bson.BsonDocument",
        "MongoDB.Bson.BsonObjectId",
        "MongoDB.Bson.BsonType",
        "MongoDB.Bson.BsonValue",
        "MongoDB.Bson.Serialization.Options.TimeSpanUnits"
    };

    private static readonly HashSet<string> s_supportedCollections = new()
    {
        "System.Collections.Generic.IEnumerable<T>",
        "System.Collections.Generic.IList<T>",
        "System.Collections.Generic.List<T>"
    };

    private static readonly HashSet<string> s_supportedSystemTypes = new()
    {
        "System.DateTime",
        "System.DateTimeKind",
        "System.DateTimeOffset",
        "System.TimeSpan",
        "System.Type"
    };

    public static (bool IsNullable, ITypeSymbol underlyingType) DiscardNullable(this ITypeSymbol typeSymbol) =>
        typeSymbol?.OriginalDefinition.SpecialType switch
        {
            SpecialType.System_Nullable_T => (true, ((INamedTypeSymbol)typeSymbol).TypeArguments.SingleOrDefault()),
            _ => (false, typeSymbol)
        };

    public static string GetBuilderDefinitionName(this ITypeSymbol typeSymbol) =>
        typeSymbol?.Name switch
        {
            "FilterDefinitionBuilder" => "Filter",
            "IndexKeysDefinitionBuilder" => "IndexKeys",
            "IndexKeysDefinitionExtensions" => "IndexKeys",
            "ProjectionDefinitionBuilder" => "Projection",
            "ProjectionDefinitionExtensions" => "Projection",
            "PipelineDefinitionBuilder" => "Projection",
            "SearchDefinitionBuilder" => "Search",
            "SearchSpanDefinitionBuilder" => "SearchSpan",
            "SortDefinitionBuilder" => "Sort",
            "SortDefinitionExtensions" => "Sort",
            "UpdateDefinitionBuilder" => "Update",
            _ => null
        };

    public static SyntaxToken[] GetFieldModifiers(this IFieldSymbol fieldSymbol) =>
        fieldSymbol.IsReadOnly ? GetReadOnlyPublicFieldModifiers() : GetPublicFieldModifiers();

    public static IMethodSymbol GetMethodSymbol(this SyntaxNode node, SemanticModel semanticModel) =>
        semanticModel.GetSymbolInfo(node).Symbol as IMethodSymbol;

    public static AccessorDeclarationSyntax[] GetPropertyAccessors(this IPropertySymbol propertySymbol) =>
        propertySymbol.IsReadOnly ? GetReadOnlyPropertyAccessors() : (propertySymbol.IsWriteOnly ? GetWriteOnlyPropertyAccessors() : GetReadWritePropertyAccessors());

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
            "SearchSpanDefinitionBuilder" or
            "SortDefinitionBuilder" or
            "SortDefinitionExtensions" or
            "UpdateDefinitionBuilder" => true,
            _ => false
        };

    public static bool IsBuildersContainer(this INamedTypeSymbol namedSymbol) =>
        namedSymbol?.Name == "Builders" && IsDefinedInMongoDriver(namedSymbol);

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

    public static bool IsBuilderMethod(this IMethodSymbol methodSymbol) =>
        methodSymbol != null &&
        (methodSymbol.ReceiverType.IsBuilder() ||
         (methodSymbol.ReducedFrom?.ReceiverType).IsBuilder());

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

    public static bool IsDBSet(this ITypeSymbol typeSymbol) =>
        typeSymbol?.Name == "DbSet" &&
        typeSymbol?.ContainingNamespace?.ToDisplayString() == NamespaceEF;

    public static bool IsDefinedInMongoDriver(this ISymbol symbol) => symbol?.ContainingAssembly.Name == AssemblyMongoDBDriver;

    public static bool IsDefinedInMongoLinqOrSystemLinq(this ISymbol symbol)
    {
        var containingNamespace = symbol?.ContainingNamespace?.ToDisplayString();

        // In case of system linq, the containing module is not validated for simplicity,
        // as it can differ in different .net frameworks.
        return containingNamespace == NamespaceSystemLinq ||
            containingNamespace == NamespaceMongoDBLinq &&
            symbol?.ContainingAssembly.Name == AssemblyMongoDBDriver;
    }

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

    public static bool IsIQueryable(this ITypeSymbol typeSymbol) =>
        ImplementsOrIsInterface(typeSymbol, NamespaceSystemLinq, nameof(IQueryable));

    public static bool IsString(this ITypeSymbol typeSymbol) =>
        typeSymbol?.SpecialType == SpecialType.System_String;

    public static bool IsSupportedBsonAttribute(this ITypeSymbol typeSymbol) =>
        s_supportedBsonAttributes.Contains(typeSymbol?.Name) &&
        typeSymbol?.ContainingNamespace?.ToDisplayString() == NamespaceMongoDBBsonAttributes;

    public static bool IsSupportedBsonType(this ITypeSymbol typeSymbol, string fullTypeName) =>
        s_supportedBsonTypes.Contains(fullTypeName) &&
        (typeSymbol?.ContainingNamespace?.ToDisplayString().StartsWith(NamespaceMongoDBBson) ?? false);

    public static bool IsSupportedBuilderType(this ITypeSymbol typeSymbol) =>
        typeSymbol?.TypeKind switch
        {
            TypeKind.Class or
            TypeKind.Enum or
            TypeKind.Struct => true,
            _ => false
        };

    public static bool IsSupportedCollection(this ITypeSymbol typeSymbol)
    {
        if (typeSymbol is not INamedTypeSymbol namedTypeSymbol)
        {
            return false;
        }

        while (namedTypeSymbol != null)
        {
            if (s_supportedCollections.Contains(namedTypeSymbol.ConstructedFrom?.ToDisplayString()))
            {
                return true;
            }

            if (namedTypeSymbol.Interfaces.Any(i => s_supportedCollections.Contains(i.ConstructedFrom?.ToDisplayString()))){
                return true;
            }

            namedTypeSymbol = namedTypeSymbol.BaseType;
        }

        return false;
    }

    public static bool IsSupportedIMongoCollection(this ITypeSymbol typeSymbol) =>
        typeSymbol.IsIMongoCollection() &&
        typeSymbol is INamedTypeSymbol namedType &&
        namedType.TypeArguments.Length == 1 &&
        namedType.TypeArguments[0].IsSupportedMongoCollectionType();

    public static bool IsSupportedMongoCollectionType(this ITypeSymbol typeSymbol) =>
        typeSymbol.TypeKind == TypeKind.Class &&
        !typeSymbol.IsAnonymousType;
    
    public static bool IsSupportedSystemType(this ITypeSymbol typeSymbol, string fullTypeName) =>
        (typeSymbol.SpecialType != SpecialType.None || s_supportedSystemTypes.Contains(fullTypeName)) &&
        typeSymbol?.ContainingNamespace?.ToDisplayString() == NamespaceSystem;

    private static SyntaxToken[] GetPublicFieldModifiers() =>
        new[] { SyntaxFactory.Token(SyntaxKind.PublicKeyword) };

    private static AccessorDeclarationSyntax[] GetReadOnlyPropertyAccessors() =>
        new[] { SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)) };

    private static SyntaxToken[] GetReadOnlyPublicFieldModifiers() =>
        new[] { SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword) };

    private static AccessorDeclarationSyntax[] GetReadWritePropertyAccessors() =>
        new[] { SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)), SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)) };

    private static AccessorDeclarationSyntax[] GetWriteOnlyPropertyAccessors() =>
        new[] { SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)) };

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
