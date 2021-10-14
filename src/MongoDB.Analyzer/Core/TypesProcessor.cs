using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MongoDB.Analyzer.Core
{
    internal sealed class TypesProcessor
    {
        private readonly Dictionary<string, (string NewName, MemberDeclarationSyntax NewDeclaration)> _processedTypes;

        private int _nextTypeId = 0;

        public MemberDeclarationSyntax[] TypesDeclarations => _processedTypes.Values.Select(p => p.NewDeclaration).ToArray();

        public TypesProcessor()
        {
            _processedTypes = new Dictionary<string, (string, MemberDeclarationSyntax)>();
        }

        public string GetTypeSymbolToGeneratedTypeMapping(ITypeSymbol typeSymbol)
        {
            var fullTypeName = typeSymbol.ToDisplayString();

            if (_processedTypes.TryGetValue(fullTypeName, out var result))
            {
                return result.NewName;
            }

            return null;
        }

        public string ProcessTypeSymbol(ITypeSymbol typeSymbol)
        {
            var newTypeName = GetTypeSymbolToGeneratedTypeMapping(typeSymbol);

            if (newTypeName != null)
            {
                return newTypeName;
            }

            var rewritenDeclarationSyntax = GetSyntaxNodeFromSymbol(typeSymbol);

            var typeCode = rewritenDeclarationSyntax.ToFullString();
            var newTypeDeclaration = SyntaxFactory.ParseMemberDeclaration(typeCode);

            newTypeName = rewritenDeclarationSyntax.Identifier.Text;
            _processedTypes.Add(typeSymbol.ToDisplayString(), (newTypeName, newTypeDeclaration));

            return newTypeName;
        }

        private BaseTypeDeclarationSyntax GetSyntaxNodeFromSymbol(ITypeSymbol typeSymbol) =>
            typeSymbol.TypeKind switch
            {
                TypeKind.Enum => GetSyntaxNodeForEnum(typeSymbol),
                TypeKind.Class or TypeKind.Struct => GetSyntaxForClassOrStruct(typeSymbol),
                _ => throw new NotSupportedException($"Symbol type {typeSymbol.TypeKind} is not supported.")
            };

        private BaseTypeDeclarationSyntax GetSyntaxForClassOrStruct(ITypeSymbol typeSymbol)
        {
            TypeDeclarationSyntax typeDeclaration = typeSymbol.TypeKind == TypeKind.Struct ?
                SyntaxFactory.StructDeclaration(GetNextTypeName("struct")) :
                SyntaxFactory.ClassDeclaration(GetNextTypeName("class"));

            var members = new List<MemberDeclarationSyntax>();

            GenerateProperties(typeSymbol, members);
            GenerateFields(typeSymbol, members);

            typeDeclaration = typeDeclaration.
                AddMembers(members.ToArray()).
                AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            var baseTypeName = typeSymbol.BaseType.Name;
            if (baseTypeName != "Object" && baseTypeName != "ValueType")
            {
                var baseTypeNameGenerated = ProcessTypeSymbol(typeSymbol.BaseType);
                var baseType = SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(baseTypeNameGenerated));
                var baseList = SyntaxFactory.BaseList(SyntaxFactory.SingletonSeparatedList<BaseTypeSyntax>(baseType));

                typeDeclaration = typeDeclaration.WithBaseList(baseList);
            }

            return typeDeclaration.NormalizeWhitespace();
        }

        private BaseTypeDeclarationSyntax GetSyntaxNodeForEnum(ITypeSymbol typeSymbol)
        {
            var members = typeSymbol.
             GetMembers().
             OfType<IFieldSymbol>().
             Select(v => SyntaxFactory.EnumMemberDeclaration(
                 attributeLists: SyntaxFactory.List<AttributeListSyntax>(),
                 SyntaxFactory.Identifier(v.Name),
                 SyntaxFactory.EqualsValueClause(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(Convert.ToInt32(v.ConstantValue)))))).
                 ToArray();

            var typeDeclaration = SyntaxFactory.
                EnumDeclaration(GetNextTypeName("enum")).
                AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword)).
                AddMembers(members);

            return typeDeclaration.NormalizeWhitespace();
        }

        private void GenerateProperties(ITypeSymbol typeSymbol, List<MemberDeclarationSyntax> members)
        {
            var typeProperties = typeSymbol.
                  GetMembers().
                  OfType<IPropertySymbol>().
                  Where(p => !p.IsStatic && (p.DeclaredAccessibility == Accessibility.Public || p.DeclaredAccessibility == Accessibility.Internal));

            foreach (var propertySymbol in typeProperties)
            {
                var typeSyntax = CreateTypeSyntaxForSymbol(propertySymbol.Type);

                var propertyDeclaration = SyntaxFactory.PropertyDeclaration(typeSyntax, propertySymbol.Name);

                propertyDeclaration = propertyDeclaration.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

                if (propertySymbol.GetMethod != null)
                    propertyDeclaration = propertyDeclaration.AddAccessorListAccessors(SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));

                if (propertySymbol.SetMethod != null)
                    propertyDeclaration = propertyDeclaration.AddAccessorListAccessors(SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));

                members.Add(propertyDeclaration);
            }
        }

        private void GenerateFields(ITypeSymbol typeSymbol, List<MemberDeclarationSyntax> members)
        {
            var typeFields = typeSymbol.
                  GetMembers().
                  OfType<IFieldSymbol>().
                  Where(p => p.DeclaredAccessibility == Accessibility.Public && !p.IsStatic);

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

                var ranksList = SyntaxFactory.SeparatedList(Enumerable.
                    Range(0, arrayTypeSymbol.Rank).
                    Select(_ => (ExpressionSyntax)SyntaxFactory.OmittedArraySizeExpression()));

                arrayRankSpecifiers = SyntaxFactory.List(new[] { SyntaxFactory.ArrayRankSpecifier(ranksList) });
                var nextTypeSyntax = CreateTypeSyntaxForSymbol(arrayTypeSymbol.ElementType);

                result = SyntaxFactory.ArrayType(nextTypeSyntax, arrayRankSpecifiers.Value);
            }
            else
            {
                var typeName = typeSymbol.SpecialType == SpecialType.None ?
                    ProcessTypeSymbol(typeSymbol) : typeSymbol.Name;

                result = SyntaxFactory.ParseTypeName(typeName);
            }

            return result;
        }

        private string GetNextTypeName(string suffix) =>
            $"{AnalysisConsts.GeneratedTypeName}_{suffix}_{_nextTypeId++}";
    }
}