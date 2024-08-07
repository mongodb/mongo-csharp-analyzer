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

namespace MongoDB.Analyzer.Core;

internal static class SyntaxFactoryUtilities
{
    public static SyntaxNode NewFindOptionsArgument { get; } =
        SyntaxFactory.Argument(SyntaxFactory.ParseExpression("new FindOptions()"));

    public static ArrayCreationExpressionSyntax GetArrayCreationExpression(ArrayTypeSyntax arrayTypeSyntax, ExpressionSyntax[] expressions) =>
        SyntaxFactory.ArrayCreationExpression(SyntaxFactory.Token(SyntaxKind.NewKeyword), arrayTypeSyntax,
        SyntaxFactory.InitializerExpression(SyntaxKind.ArrayInitializerExpression, SyntaxFactory.SeparatedList<ExpressionSyntax>(expressions)));

    public static AttributeSyntax GetAttribute(string name, List<AttributeArgumentSyntax> arguments)
    {
        var separatedAttributeArgumentList = SyntaxFactory.SeparatedList<AttributeArgumentSyntax>(arguments);
        var attributeArgumentListNode = SyntaxFactory.AttributeArgumentList(separatedAttributeArgumentList);

        var nameSyntax = SyntaxFactory.ParseName(name);
        var attributeNode = SyntaxFactory.Attribute(nameSyntax, attributeArgumentListNode);

        return attributeNode;
    }

    public static CastExpressionSyntax GetCastConstantExpression(string castToTypeName, object constantValue, bool isCastToTypeNullable = false) =>
        SyntaxFactory.CastExpression(
            isCastToTypeNullable ? GetNullableType(castToTypeName) : SyntaxFactory.ParseTypeName(castToTypeName),
            GetConstantExpression(constantValue));

    public static LiteralExpressionSyntax GetConstantExpression(object constantValue) =>
        constantValue switch
        {
            bool @bool => SyntaxFactory.LiteralExpression(@bool ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression),
            string @string => SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, GetConstantValueToken(@string)),
            _ => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, GetConstantValueToken(constantValue))
        };
    
    public static SyntaxToken GetConstantValueToken(object value) =>
        value switch
        {
            sbyte @sbyte => SyntaxFactory.Literal(@sbyte),
            byte @byte => SyntaxFactory.Literal(@byte),
            short @short => SyntaxFactory.Literal(@short),
            ushort @ushort => SyntaxFactory.Literal(@ushort),
            int @int => SyntaxFactory.Literal(@int),
            uint @uint => SyntaxFactory.Literal(@uint),
            long @long => SyntaxFactory.Literal(@long),
            ulong @ulong => SyntaxFactory.Literal(@ulong),
            double @double => SyntaxFactory.Literal(@double),
            string @string => SyntaxFactory.Literal(@string),
            _ => throw new NotSupportedException($"Not supported type {value?.GetType()}")
        };

    public static NameSyntax GetIdentifier(string identifierName)
    {
        var parts = identifierName.Split('.');

        NameSyntax result = SyntaxFactory.IdentifierName(parts[0]);

        for (int i = 1; i < parts.Length; i++)
        {
            result = SyntaxFactory.QualifiedName(result, SyntaxFactory.IdentifierName(parts[i]));
        }

        return result;
    }

    public static TypeSyntax GetNullableType(string typeName) =>
        SyntaxFactory.NullableType(SyntaxFactory.ParseTypeName(typeName));

    public static SpecialType GetSpecialType(object value) =>
        value switch
        {
            sbyte @sbyte => SpecialType.System_SByte,
            byte @byte => SpecialType.System_Byte,
            short @short => SpecialType.System_Int16,
            ushort @ushort => SpecialType.System_UInt16,
            int @int => SpecialType.System_Int32,
            uint @uint => SpecialType.System_UInt32,
            long @long => SpecialType.System_Int64,
            ulong @ulong => SpecialType.System_UInt64,
            double @double => SpecialType.System_Double,
            string @string => SpecialType.System_String,
            _ => throw new NotSupportedException($"Not supported type {value?.GetType()}")
        };

    public static SimpleNameSyntax GetUnderlyingIdentifier(SyntaxNode identifier) =>
        GetUnderlyingNameSyntax(identifier.Parent);

    public static SimpleNameSyntax GetUnderlyingNameSyntax(SyntaxNode syntax)
    {
        var underlyingNameSyntax = syntax as SimpleNameSyntax;

        if (syntax is MemberAccessExpressionSyntax memberAccessExpressionSyntax)
        {
            while (true)
            {
                var trimmedExpression = TrimElementAccessNullOperatorAndInvocationSyntax(memberAccessExpressionSyntax.Expression);
                if (trimmedExpression is not MemberAccessExpressionSyntax nextMemberAccessExpressionSyntax)
                {
                    underlyingNameSyntax = trimmedExpression as SimpleNameSyntax;

                    break;
                }

                memberAccessExpressionSyntax = nextMemberAccessExpressionSyntax;
            }
        }

        return underlyingNameSyntax;
    }

    public static bool IsMemberOfAnonymousObject(this SyntaxNode identifier) =>
        identifier.Parent.IsKind(SyntaxKind.NameEquals) &&
        identifier.Parent.Parent.IsKind(SyntaxKind.AnonymousObjectMemberDeclarator);

    public static SyntaxNode ResolveAccessExpressionNode(SyntaxNode identifier)
    {
        SyntaxNode result = identifier;

        while (result.Parent.IsKind(SyntaxKind.InvocationExpression) ||
             result.Parent.IsKind(SyntaxKind.SimpleMemberAccessExpression))
        {
            result = result.Parent;
        }

        return result;
    }

    public static MemberAccessExpressionSyntax SimpleMemberAccess(string source, string member) =>
        SyntaxFactory.MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            SyntaxFactory.IdentifierName(source),
            SyntaxFactory.IdentifierName(member));

    public static ExpressionSyntax TrimElementAccessNullOperatorAndInvocationSyntax(ExpressionSyntax expressionSyntax)
    {
        var result = expressionSyntax;

        while (true)
        {
            if (result is ElementAccessExpressionSyntax elementAccessExpressionSyntax)
            {
                result = elementAccessExpressionSyntax.Expression;
            }
            else if (result is InvocationExpressionSyntax invocationExpressionSyntax)
            {
                result = invocationExpressionSyntax.Expression;
            }
            else if (result is PostfixUnaryExpressionSyntax postfixUnaryExpressionSyntax &&
                postfixUnaryExpressionSyntax.Kind() == SyntaxKind.SuppressNullableWarningExpression)
            {
                result = postfixUnaryExpressionSyntax.Operand;
            }
            else
            {
                break;
            }
        }

        return result;
    }
}
