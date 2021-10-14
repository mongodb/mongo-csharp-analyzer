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

internal sealed class TypesProcessor
{
    private static readonly Dictionary<string, string> s_knownTypes = new()
    {
        { "MongoDB.Bson.BsonDocument", "BsonDocumentCustom123" },
        { "MongoDB.Bson.BsonValue", "BsonValueCustom123" },
        { "MongoDB.Bson.BsonObjectId", "BsonObjectIdCustom123" },
        { "MongoDB.Bson.BsonType", "BsonTypeCustom123" }
    };

    private readonly Dictionary<string, (string NewName, MemberDeclarationSyntax NewDeclaration)> _processedTypes;

    private int _nextTypeId = 0;

    public MemberDeclarationSyntax[] TypesDeclarations => _processedTypes.Values.Select(p => p.NewDeclaration).ToArray();

    public TypesProcessor()
    {
        _processedTypes = new Dictionary<string, (string, MemberDeclarationSyntax)>();
    }

    public string GetTypeSymbolToGeneratedTypeMapping(ITypeSymbol typeSymbol)
    {
        if (typeSymbol == null)
            return null;

        var fullTypeName = GetFullName(typeSymbol);
        if (s_knownTypes.TryGetValue(fullTypeName, out var knowTypeName))
            return knowTypeName;

        if (_processedTypes.TryGetValue(fullTypeName, out var result))
        {
            return result.NewName;
        }

        return null;
    }

    public string ProcessTypeSymbol(ITypeSymbol typeSymbol)
    {
        if (typeSymbol == null)
            return null;

        var fullTypeName = GetFullName(typeSymbol);
        if (s_knownTypes.TryGetValue(fullTypeName, out var knowTypeName))
            return knowTypeName;

        if (_processedTypes.TryGetValue(fullTypeName, out var pair))
        {
            return pair.NewName;
        }

        var newTypeName = GetNewNameForSymbol(typeSymbol);
        _processedTypes[fullTypeName] = (newTypeName, null); // Cache the name, for self referencing types

        var rewrittenDeclarationSyntax = GetSyntaxNodeFromSymbol(typeSymbol, newTypeName);

        var typeCode = rewrittenDeclarationSyntax.ToFullString();
        var newTypeDeclaration = SyntaxFactory.ParseMemberDeclaration(typeCode);

        newTypeName = rewrittenDeclarationSyntax.Identifier.Text;
        _processedTypes[fullTypeName] = (newTypeName, newTypeDeclaration);

        return newTypeName;
    }

    private BaseTypeDeclarationSyntax GetSyntaxNodeFromSymbol(ITypeSymbol typeSymbol, string newTypeName)
    {
        BaseTypeDeclarationSyntax typeDeclaration;

        switch (typeSymbol.TypeKind)
        {
            case TypeKind.Enum:
                {
                    typeDeclaration = GetSyntaxNodeForEnum(typeSymbol, newTypeName);
                    break;
                }
            case TypeKind.Class:
            case TypeKind.Struct:
                {
                    typeDeclaration = GetSyntaxForClassOrStruct(typeSymbol, newTypeName);
                    break;
                }
            default:
                throw new NotSupportedException($"Symbol type {typeSymbol.TypeKind} is not supported.");
        }

        typeDeclaration = typeDeclaration.NormalizeWhitespace();
        return typeDeclaration;
    }

    private TypeDeclarationSyntax GetSyntaxForClassOrStruct(ITypeSymbol typeSymbol, string newTypeName)
    {
        TypeDeclarationSyntax typeDeclaration = typeSymbol.TypeKind == TypeKind.Struct ?
            SyntaxFactory.StructDeclaration(newTypeName) :
            SyntaxFactory.ClassDeclaration(newTypeName);

        var members = new List<MemberDeclarationSyntax>();

        GenerateProperties(typeSymbol, members);
        GenerateFields(typeSymbol, members);

        typeDeclaration = typeDeclaration
            .AddMembers(members.ToArray())
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

        var baseSpecialType = typeSymbol.BaseType?.SpecialType;

        if (baseSpecialType != SpecialType.System_Object && baseSpecialType != SpecialType.System_ValueType)
        {
            var baseTypeNameGenerated = ProcessTypeSymbol(typeSymbol.BaseType);

            typeDeclaration = typeDeclaration.WithBaseList(GetBaseListSyntax(baseTypeNameGenerated));
        }

        return typeDeclaration;
    }

    private EnumDeclarationSyntax GetSyntaxNodeForEnum(ITypeSymbol typeSymbol, string newTypeName)
    {
        var members = typeSymbol
            .GetMembers()
            .OfType<IFieldSymbol>()
            .Select(v => SyntaxFactory.EnumMemberDeclaration(
                attributeLists: SyntaxFactory.List<AttributeListSyntax>(),
                SyntaxFactory.Identifier(v.Name),
                SyntaxFactory.EqualsValueClause(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactoryUtilities.GetConstantValueToken(v.ConstantValue)))))
                .ToArray();

        var typeDeclaration = SyntaxFactory
            .EnumDeclaration(newTypeName)
            .WithBaseList(GetBaseListSyntax((typeSymbol as INamedTypeSymbol).EnumUnderlyingType.Name))
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .AddMembers(members);

        return typeDeclaration;
    }

    private BaseListSyntax GetBaseListSyntax(string typeName)
    {
        var baseType = SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(typeName));
        var baseList = SyntaxFactory.BaseList(SyntaxFactory.SingletonSeparatedList<BaseTypeSyntax>(baseType));

        return baseList;
    }

    private void GenerateProperties(ITypeSymbol typeSymbol, List<MemberDeclarationSyntax> members)
    {
        var typeProperties = typeSymbol
              .GetMembers()
              .OfType<IPropertySymbol>()
              .Where(p =>
                !p.IsStatic &&
                !p.IsIndexer &&
                (p.Type.TypeKind != TypeKind.Interface || p.Type.IsSupportedCollection()) &&
                p.DeclaredAccessibility == Accessibility.Public);

        foreach (var propertySymbol in typeProperties)
        {
            var typeSyntax = CreateTypeSyntaxForSymbol(propertySymbol.Type);

            var propertyDeclaration = SyntaxFactory.PropertyDeclaration(typeSyntax, propertySymbol.Name);

            propertyDeclaration = propertyDeclaration.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            if (propertySymbol.GetMethod != null)
            {
                propertyDeclaration = propertyDeclaration.AddAccessorListAccessors(SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));
            }

            if (propertySymbol.SetMethod != null)
            {
                propertyDeclaration = propertyDeclaration.AddAccessorListAccessors(SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));
            }

            members.Add(propertyDeclaration);
        }
    }

    private void GenerateFields(ITypeSymbol typeSymbol, List<MemberDeclarationSyntax> members)
    {
        var typeFields = typeSymbol
            .GetMembers()
            .OfType<IFieldSymbol>()
            .Where(p =>
                !p.IsStatic &&
                (p.Type.TypeKind != TypeKind.Interface || p.Type.IsSupportedCollection()) &&
                p.DeclaredAccessibility == Accessibility.Public);

        foreach (var fieldSymbol in typeFields)
        {
            var typeSyntax = CreateTypeSyntaxForSymbol(fieldSymbol.Type);

            var variableDeclaration = SyntaxFactory.VariableDeclaration(typeSyntax, SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator(fieldSymbol.Name)));

            var fieldDeclaration = SyntaxFactory.FieldDeclaration(
                attributeLists: SyntaxFactory.List<AttributeListSyntax>(),
                modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)),
                declaration: variableDeclaration);

            members.Add(fieldDeclaration);
        }
    }

    private TypeSyntax CreateTypeSyntaxForSymbol(ITypeSymbol typeSymbol)
    {
        TypeSyntax result;

        if (typeSymbol is IArrayTypeSymbol arrayTypeSymbol)
        {
            SyntaxList<ArrayRankSpecifierSyntax>? arrayRankSpecifiers = null;

            var ranksList = SyntaxFactory.SeparatedList(
                Enumerable.Range(0, arrayTypeSymbol.Rank)
                .Select(_ => (ExpressionSyntax)SyntaxFactory.OmittedArraySizeExpression()));

            arrayRankSpecifiers = SyntaxFactory.List(new[] { SyntaxFactory.ArrayRankSpecifier(ranksList) });
            var nextTypeSyntax = CreateTypeSyntaxForSymbol(arrayTypeSymbol.ElementType);

            result = SyntaxFactory.ArrayType(nextTypeSyntax, arrayRankSpecifiers.Value);
        }
        // TODO optimize
        else if (typeSymbol is INamedTypeSymbol namedTypeSymbol &&
            namedTypeSymbol.TypeArguments.Length == 1 &&
            namedTypeSymbol.IsSupportedCollection())
        {
            var underlyingTypeSyntax = CreateTypeSyntaxForSymbol(namedTypeSymbol.TypeArguments.Single());
            var listSyntax = SyntaxFactory.GenericName(
                SyntaxFactory.Identifier("List"),
                SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList(new[] { underlyingTypeSyntax })));

            result = SyntaxFactory.QualifiedName(
                    SyntaxFactory.QualifiedName(
                        SyntaxFactory.QualifiedName(
                            SyntaxFactory.IdentifierName("System"),
                            SyntaxFactory.IdentifierName("Collections")),
                        SyntaxFactory.IdentifierName("Generic")),
                    listSyntax);
        }
        else
        {
            var typeName = typeSymbol.SpecialType == SpecialType.None ?
                ProcessTypeSymbol(typeSymbol) : typeSymbol.Name;

            result = SyntaxFactory.ParseTypeName(typeName);
        }

        return result;
    }

    private string GetNewNameForSymbol(ITypeSymbol typeSymbol) =>
        typeSymbol.TypeKind switch
        {
            TypeKind.Class or
            TypeKind.Enum or
            TypeKind.Struct => $"{AnalysisConstants.GeneratedTypeName}_{typeSymbol.TypeKind}_{_nextTypeId++}",
            _ => null
        };

    private string GetFullName(ITypeSymbol typeSymbol) =>
        typeSymbol.ToDisplayString();
}
