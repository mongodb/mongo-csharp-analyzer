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

internal static class SyntaxNodeExtensions
{
    public static string GetCommentText(this SyntaxTrivia syntaxTrivia) =>
        syntaxTrivia.ToFullString().Trim('/', ' ');

    public static IdentifierNameSyntax[] GetIdentifiers(this SyntaxNode syntaxNode, string identifierName) =>
        syntaxNode.DescendantNodes()
        .OfType<IdentifierNameSyntax>()
        .Where(i => i.Identifier.Text == identifierName)
        .ToArray();

    public static IdentifierNameSyntax GetSingleIdentifier(this SyntaxNode syntaxNode, string identifierName) =>
        syntaxNode.DescendantNodes().OfType<IdentifierNameSyntax>().Single(i => i.Identifier.Text == identifierName);

    public static ClassDeclarationSyntax GetSingleClassDeclaration(this SyntaxNode syntaxNode, string className) =>
        syntaxNode.DescendantNodes().OfType<ClassDeclarationSyntax>().Single(i => i.Identifier.Text == className);

    public static MethodDeclarationSyntax GetSingleMethod(this SyntaxNode syntaxNode, string name) =>
        syntaxNode.DescendantNodes().OfType<MethodDeclarationSyntax>().Single(m => m.Identifier.Text == name);

    public static SyntaxNode GetTopMostInvocationOrBinaryExpressionSyntax(SimpleNameSyntax identifier, IdentifierNameSyntax[] lambdaIdentifiers)
    {
        SyntaxNode previous = null;
        SyntaxNode result = identifier;

        while (IsValid(previous, result.Parent))
        {
            previous = result;
            result = result.Parent;
        }

        bool IsValid(SyntaxNode previousSyntaxNode, SyntaxNode syntaxNode)
        {
            if (syntaxNode is InvocationExpressionSyntax)
            {
                return true;
            }

            if (syntaxNode is BinaryExpressionSyntax binaryExpressionSyntax)
            {
                if (binaryExpressionSyntax.IsKind(SyntaxKind.EqualsExpression) ||
                    binaryExpressionSyntax.IsKind(SyntaxKind.NotEqualsExpression))
                {
                    return false;
                }

                if (lambdaIdentifiers != null)
                {
                    if (binaryExpressionSyntax.Left != previousSyntaxNode &&
                        lambdaIdentifiers.Any(l => binaryExpressionSyntax.Left.Contains(l)))
                    {
                        return false;
                    }

                    if (binaryExpressionSyntax.Right != previousSyntaxNode &&
                        lambdaIdentifiers.Any(l => binaryExpressionSyntax.Right.Contains(l)))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        return result;
    }

    public static bool IsLeaf(this SimpleNameSyntax identifier)
    {
        // parent is access expression
        if (identifier.Parent is MemberAccessExpressionSyntax memberAccessExpressionSyntax)
        {
            // not part of access expression
            if (memberAccessExpressionSyntax.Name == identifier)
            {
                if (memberAccessExpressionSyntax.Parent.IsKind(SyntaxKind.SimpleMemberAccessExpression))
                {
                    // part of field/property access chain
                    return false;
                }
                else if (memberAccessExpressionSyntax.Parent.IsKind(SyntaxKind.ElementAccessExpression))
                {
                    // part of array
                    return false;
                }
                else if (IsMethodAndNotLeaf(memberAccessExpressionSyntax.Parent))
                {
                    // part of methods access chain
                    return false;
                }

                return true;
            }
        }
        else if (IsMethodAndNotLeaf(identifier.Parent))
        {
            // part of methods access chain
            return false;
        }
        else // arbitrary parent
        {
            return true;
        }

        static bool IsMethodAndNotLeaf(SyntaxNode syntaxNode) =>
            syntaxNode.IsKind(SyntaxKind.InvocationExpression) && syntaxNode.Parent.IsKind(SyntaxKind.SimpleMemberAccessExpression);

        return false;
    }
}
