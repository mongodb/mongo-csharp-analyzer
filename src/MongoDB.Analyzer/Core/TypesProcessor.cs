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
    private readonly Dictionary<string, (string NewName, MemberDeclarationSyntax NewDeclaration)> _processedTypes;
    private int _nextTypeId = 0;

    public MemberDeclarationSyntax[] TypesDeclarations => _processedTypes.Values.Select(p => p.NewDeclaration).ToArray();

    public IEnumerable<(string NewName, string PreviousName)> GeneratedTypeToOriginalTypeMapping => _processedTypes.Select(pair => (pair.Value.NewName, pair.Key));

    public TypesProcessor()
    {
        _processedTypes = new Dictionary<string, (string, MemberDeclarationSyntax)>();
    }

    public string GetTypeSymbolToGeneratedTypeMapping(ITypeSymbol typeSymbol) =>
        GetGeneratedTypeMapping(typeSymbol, false).RemappedName;

    public MemberDeclarationSyntax GetTypeSymbolToMemberDeclarationMapping(ITypeSymbol typeSymbol)
    {
        if (typeSymbol == null)
        {
            return null;
        }

        var fullTypeName = GetFullName(typeSymbol);

        if (_processedTypes.TryGetValue(fullTypeName, out var result))
        {
            return result.NewDeclaration;
        }

        return null;
    }

    public bool IsUserTypeProcessed(ITypeSymbol typeSymbol) =>
        GetGeneratedTypeMapping(typeSymbol, true).RemappedName != null;

    public string ProcessTypeSymbol(ITypeSymbol typeSymbol)
    {
        var (remappedName, fullTypeName) = GetGeneratedTypeMapping(typeSymbol, false);
        if (fullTypeName == null)
        {
            return null;
        }

        if (remappedName != null)
        {
            return remappedName;
        }

        remappedName = GetNewNameForSymbol(typeSymbol);
        BaseTypeDeclarationSyntax rewrittenDeclarationSyntax;

        _processedTypes[fullTypeName] = (remappedName, null); // Cache the name, for self referencing types

        try
        {
            rewrittenDeclarationSyntax = GetSyntaxNodeFromSymbol(typeSymbol, remappedName);
        }
        catch
        {
            _processedTypes.Remove(fullTypeName);
            throw;
        }

        if (rewrittenDeclarationSyntax == null)
        {
            _processedTypes.Remove(fullTypeName);
            return null;
        }

        var typeCode = rewrittenDeclarationSyntax.ToFullString();
        var newTypeDeclaration = SyntaxFactory.ParseMemberDeclaration(typeCode);

        remappedName = rewrittenDeclarationSyntax.Identifier.Text;
        _processedTypes[fullTypeName] = (remappedName, newTypeDeclaration);

        return remappedName;
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

            if (nextTypeSyntax == null)
            {
                return null;
            }

            result = SyntaxFactory.ArrayType(nextTypeSyntax, arrayRankSpecifiers.Value);
        }
        // TODO optimize
        else if (typeSymbol is INamedTypeSymbol namedTypeSymbol &&
            namedTypeSymbol.TypeArguments.Length >= 1 &&
            namedTypeSymbol.IsDerivedFromSystemCollectionGenerics())
        {
            var underlyingTypeSyntaxes = namedTypeSymbol.TypeArguments.Select(typeArgument => CreateTypeSyntaxForSymbol(typeArgument));
            if (underlyingTypeSyntaxes.Any(underlyingTypeSyntax => underlyingTypeSyntax == null))
            {
                return null;
            }

            var collectionIdentifier = namedTypeSymbol.Name;

            var collectionSyntax = SyntaxFactory.GenericName(
                SyntaxFactory.Identifier(collectionIdentifier),
                SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList(underlyingTypeSyntaxes)));

            result = SyntaxFactory.QualifiedName(
                        SyntaxFactory.QualifiedName(
                            SyntaxFactory.QualifiedName(
                                SyntaxFactory.IdentifierName("System"),
                                SyntaxFactory.IdentifierName("Collections")),
                            SyntaxFactory.IdentifierName("Generic")),
                        collectionSyntax);
        }
        else if (typeSymbol.IsDerivedFromSystemCollectionGenerics(recursive: true))
        {
            return null;
        }
        else
        {
            var (isNullable, underlingTypeSymbol) = typeSymbol.DiscardNullable();
            var newTypeName = ProcessTypeSymbol(underlingTypeSymbol);

            if (newTypeName == null)
            {
                return null;
            }

            result = isNullable ? SyntaxFactoryUtilities.GetNullableType(newTypeName) :
                SyntaxFactory.ParseTypeName(newTypeName);
        }

        return result;
    }

    private AttributeArgumentSyntax GenerateBsonAttributeArgument(TypedConstant argumentValue, bool isNamed, string argumentKey)
    {
        var expressionSyntax = GenerateExpressionFromBsonAttributeArgumentInfo(argumentValue);
        var attributeArgument = isNamed ? SyntaxFactory.AttributeArgument(SyntaxFactory.NameEquals(SyntaxFactory.IdentifierName(argumentKey)), null, expressionSyntax) :
            SyntaxFactory.AttributeArgument(expressionSyntax);

        return attributeArgument;
    }

    private AttributeListSyntax GenerateBsonAttributeList(ISymbol symbol)
    {
        var bsonAttributes = symbol.GetAttributes().Where(attribute => attribute.AttributeClass.IsSupportedBsonAttribute());
        if (bsonAttributes.EmptyOrNull())
        {
            return null;
        }

        var generatedBsonAttributes = new List<AttributeSyntax>();
        foreach (var bsonAttribute in bsonAttributes)
        {
            var bsonAttributeArgumentList = new List<AttributeArgumentSyntax>();

            bsonAttributeArgumentList.AddRange(bsonAttribute.ConstructorArguments
                .Select(bsonAttributeArgument => GenerateBsonAttributeArgument(bsonAttributeArgument, false, null)));

            bsonAttributeArgumentList.AddRange(bsonAttribute.NamedArguments
                .Select(bsonAttributeArgument => GenerateBsonAttributeArgument(bsonAttributeArgument.Value, true, bsonAttributeArgument.Key)));

            var bsonAttributeSyntaxNode = SyntaxFactoryUtilities.GetAttribute(bsonAttribute.AttributeClass.Name, bsonAttributeArgumentList);
            generatedBsonAttributes.Add(bsonAttributeSyntaxNode);
        }

        var bsonAttributeList = SyntaxFactory.AttributeList(SyntaxFactory.SeparatedList<AttributeSyntax>(generatedBsonAttributes));
        return bsonAttributeList;
    }

    private ExpressionSyntax GenerateExpressionFromBsonAttributeArgumentInfo(TypedConstant attributeArgumentInfo) =>
        attributeArgumentInfo.Kind switch
        {
            TypedConstantKind.Enum => HandleEnumInBsonAttributeArgument(attributeArgumentInfo.Value, attributeArgumentInfo.Type),
            TypedConstantKind.Primitive => HandlePrimitiveInBsonAttributeArgument(attributeArgumentInfo.Value),
            TypedConstantKind.Array => HandleArrayInBsonAttributeArgument(attributeArgumentInfo.Values, attributeArgumentInfo.Type as IArrayTypeSymbol),
            TypedConstantKind.Type => HandleTypeInBsonAttributeArgument(attributeArgumentInfo.Value),
            _ => null
        };

    private bool GenerateFields(ITypeSymbol typeSymbol, List<MemberDeclarationSyntax> members)
    {
        var typeFields = typeSymbol
            .GetMembers()
            .OfType<IFieldSymbol>()
            .Where(p =>
                !p.IsStatic &&
                (p.Type.TypeKind != TypeKind.Interface || p.Type.IsDerivedFromSystemCollectionGenerics()) &&
                p.DeclaredAccessibility == Accessibility.Public);

        foreach (var fieldSymbol in typeFields)
        {
            var typeSyntax = CreateTypeSyntaxForSymbol(fieldSymbol.Type);
            if (typeSyntax == null)
            {
                return false;
            }

            var variableDeclaration = SyntaxFactory.VariableDeclaration(typeSyntax, SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator(fieldSymbol.Name)));

            var bsonAttributeList = GenerateBsonAttributeList(fieldSymbol);
            var attributeLists = bsonAttributeList != null && bsonAttributeList.Attributes.AnySafe() ?
                SyntaxFactory.List<AttributeListSyntax>(SyntaxFactory.SingletonSeparatedList<AttributeListSyntax>(bsonAttributeList)) :
                SyntaxFactory.List<AttributeListSyntax>();

            var fieldDeclaration = SyntaxFactory.FieldDeclaration(
                attributeLists: attributeLists,
                modifiers: SyntaxFactory.TokenList(fieldSymbol.GetFieldModifiers()),
                declaration: variableDeclaration);

            members.Add(fieldDeclaration);
        }

        return true;
    }

    private bool GenerateProperties(ITypeSymbol typeSymbol, List<MemberDeclarationSyntax> members)
    {
        var typeProperties = typeSymbol
              .GetMembers()
              .OfType<IPropertySymbol>()
              .Where(p =>
                !p.IsStatic &&
                !p.IsIndexer &&
                (p.Type.TypeKind != TypeKind.Interface || p.Type.IsDerivedFromSystemCollectionGenerics()) &&
                p.DeclaredAccessibility == Accessibility.Public);

        foreach (var propertySymbol in typeProperties)
        {
            var typeSyntax = CreateTypeSyntaxForSymbol(propertySymbol.Type);
            if (typeSyntax == null)
            {
                return false;
            }

            var propertyDeclaration = SyntaxFactory.PropertyDeclaration(typeSyntax, propertySymbol.Name);

            propertyDeclaration = propertyDeclaration.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddAccessorListAccessors(propertySymbol.GetPropertyAccessors());

            var bsonAttributeList = GenerateBsonAttributeList(propertySymbol);
            if (bsonAttributeList != null && bsonAttributeList.Attributes.AnySafe())
            {
                propertyDeclaration = propertyDeclaration.AddAttributeLists(bsonAttributeList);
            }

            members.Add(propertyDeclaration);
        }

        return true;
    }

    private BaseListSyntax GetBaseListSyntax(string typeName)
    {
        var baseType = SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(typeName));
        var baseList = SyntaxFactory.BaseList(SyntaxFactory.SingletonSeparatedList<BaseTypeSyntax>(baseType));

        return baseList;
    }

    private string GetFullName(ITypeSymbol typeSymbol) =>
        typeSymbol.ToDisplayString();

    private (string RemappedName, string FullTypeName) GetGeneratedTypeMapping(ITypeSymbol typeSymbol, bool userOnlyTypes)
    {
        if (typeSymbol == null ||
            typeSymbol.IsDerivedFromSystemCollectionGenerics(recursive: true))
        {
            return default;
        }

        var fullTypeName = GetFullName(typeSymbol);
        if (!userOnlyTypes && typeSymbol.IsSupportedBsonType(fullTypeName))
        {
            return (typeSymbol.Name, fullTypeName);
        }

        if (_processedTypes.TryGetValue(fullTypeName, out var result))
        {
            return (result.NewName, fullTypeName);
        }

        if (!userOnlyTypes && typeSymbol.IsSupportedSystemType(fullTypeName))
        {
            return (fullTypeName, fullTypeName);
        }

        return (null, fullTypeName);
    }

    private string GetNewNameForSymbol(ITypeSymbol typeSymbol) =>
        typeSymbol.TypeKind switch
        {
            TypeKind.Class or
            TypeKind.Enum or
            TypeKind.Struct => $"{AnalysisConstants.GeneratedTypeName}_{typeSymbol.TypeKind}_{_nextTypeId++}",
            _ => null
        };

    private TypeDeclarationSyntax GetSyntaxForClassOrStruct(ITypeSymbol typeSymbol, string newTypeName)
    {
        TypeDeclarationSyntax typeDeclaration = typeSymbol.TypeKind == TypeKind.Struct ?
            SyntaxFactory.StructDeclaration(newTypeName) :
            SyntaxFactory.ClassDeclaration(newTypeName);

        var members = new List<MemberDeclarationSyntax>();

        if (!GenerateProperties(typeSymbol, members) ||
            !GenerateFields(typeSymbol, members))
        {
            return null;
        }

        typeDeclaration = typeDeclaration
            .AddMembers(members.ToArray())
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

        var baseSpecialType = typeSymbol.BaseType?.SpecialType;

        if (baseSpecialType != SpecialType.System_Object && baseSpecialType != SpecialType.System_ValueType)
        {
            var baseTypeNameGenerated = ProcessTypeSymbol(typeSymbol.BaseType);

            if (baseTypeNameGenerated == null)
            {
                return null;
            }

            typeDeclaration = typeDeclaration.WithBaseList(GetBaseListSyntax(baseTypeNameGenerated));
        }

        var bsonAttributeList = GenerateBsonAttributeList(typeSymbol);
        if (bsonAttributeList != null && bsonAttributeList.Attributes.AnySafe())
        {
            typeDeclaration = typeDeclaration.AddAttributeLists(bsonAttributeList);
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

        if (typeDeclaration == null)
        {
            return null;
        }

        typeDeclaration = typeDeclaration.NormalizeWhitespace();
        return typeDeclaration;
    }

    private ExpressionSyntax HandleArrayInBsonAttributeArgument(ImmutableArray<TypedConstant> argumentValues, IArrayTypeSymbol arrayTypeSymbol)
    {
        var ranksList = SyntaxFactory.SeparatedList(
                Enumerable.Range(0, arrayTypeSymbol.Rank)
                .Select(_ => (ExpressionSyntax)SyntaxFactory.OmittedArraySizeExpression()));

        var arrayRankSpecifiers = SyntaxFactory.List(new[] { SyntaxFactory.ArrayRankSpecifier(ranksList) });
        var arrayElementTypeName = ProcessTypeSymbol(arrayTypeSymbol.ElementType);

        var arrayTypeSyntax = SyntaxFactory.ArrayType(SyntaxFactory.ParseTypeName(arrayElementTypeName), arrayRankSpecifiers);
        var expressionList = argumentValues.Select(argumentValue => GenerateExpressionFromBsonAttributeArgumentInfo(argumentValue)).ToArray();

        return SyntaxFactoryUtilities.GetArrayCreationExpression(arrayTypeSyntax, expressionList);
    }

    private ExpressionSyntax HandleEnumInBsonAttributeArgument(object value, ITypeSymbol typeSymbol) =>
        SyntaxFactoryUtilities.GetCastConstantExpression(ProcessTypeSymbol(typeSymbol), value);

    private LiteralExpressionSyntax HandlePrimitiveInBsonAttributeArgument(object value) =>
        SyntaxFactoryUtilities.GetConstantExpression(value);

    private ExpressionSyntax HandleTypeInBsonAttributeArgument(object argumentValue) =>
        SyntaxFactory.TypeOfExpression(SyntaxFactory.ParseTypeName(ProcessTypeSymbol(argumentValue as INamedTypeSymbol)));
}
