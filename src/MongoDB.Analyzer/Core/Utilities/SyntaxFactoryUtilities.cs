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

internal static class SyntaxFactoryUtilities
{
    public static MemberAccessExpressionSyntax SimpleMemberAccess(string source, string member) =>
        SyntaxFactory.MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            SyntaxFactory.IdentifierName(source),
            SyntaxFactory.IdentifierName(member));

    public static CastExpressionSyntax GetCastConstantExpression(string castToTypeName, object constantValue) =>
        SyntaxFactory.CastExpression(SyntaxFactory.ParseTypeName(castToTypeName), GetConstantExpression(constantValue));

    public static LiteralExpressionSyntax GetConstantExpression(object constantValue)
    {
        var syntaxToken = GetConstantValueToken(constantValue);
        var expressionKind = constantValue is string ? SyntaxKind.StringLiteralExpression : SyntaxKind.NumericLiteralExpression;

        return SyntaxFactory.LiteralExpression(expressionKind, syntaxToken);
    }

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

    public static SimpleNameSyntax GetUnderlyingNameSyntax(SyntaxNode syntax)
    {
        var underlyingNameSyntax = syntax as SimpleNameSyntax;

        if (syntax is MemberAccessExpressionSyntax memberAccessExpressionSyntax)
        {
            while (true)
            {
                var trimmedExpression = TrimElementAccessAndInvocationSyntax(memberAccessExpressionSyntax.Expression);
                if (trimmedExpression is not MemberAccessExpressionSyntax nextMemberAccessExpressionSyntax)
                {
                    underlyingNameSyntax = memberAccessExpressionSyntax.Expression as SimpleNameSyntax;

                    break;
                }

                memberAccessExpressionSyntax = nextMemberAccessExpressionSyntax;
            }
        }

        return underlyingNameSyntax;
    }

    public static SimpleNameSyntax GetUnderlyingIdentifier(SyntaxNode identifier) =>
        GetUnderlyingNameSyntax(identifier.Parent);

    public static bool IsMemberOfAnonymousObject(this SyntaxNode identifier) =>
        identifier.Parent.IsKind(SyntaxKind.NameEquals) &&
        identifier.Parent.Parent.IsKind(SyntaxKind.AnonymousObjectMemberDeclarator);

    public static ExpressionSyntax TrimElementAccessAndInvocationSyntax(ExpressionSyntax expressionSyntax)
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
            else
            {
                break;
            }
        }

        return result;
    }

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
}
