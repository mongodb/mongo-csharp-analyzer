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
    public static SyntaxNode TrimParenthesis(this SyntaxNode syntaxNode)
    {
        while (syntaxNode is ParenthesizedExpressionSyntax parenthesizedExpression)
        {
            syntaxNode = parenthesizedExpression.Expression;
        }

        return syntaxNode;
    }

    public static IEnumerable<SyntaxNode> DescendantNodesWithSkipList(this SyntaxNode syntaxNode, HashSet<SyntaxNode> skipList)
    {
        foreach (var node in syntaxNode.DescendantNodes(n => !skipList.Contains(n.Parent)))
        {
            if (skipList.Contains(node.Parent))
            {
                continue;
            }

            yield return node;
        }
    }

    public static string GetCommentText(this SyntaxTrivia syntaxTrivia) =>
        syntaxTrivia.ToFullString().Trim('/', ' ');

    public static IdentifierNameSyntax[] GetIdentifiers(this SyntaxNode syntaxNode, string identifierName) =>
        syntaxNode.DescendantNodes()
        .OfType<IdentifierNameSyntax>()
        .Where(i => i.Identifier.Text == identifierName)
        .ToArray();

    public static ExpressionSyntax GetNextNestedInvocation(this SyntaxNode syntaxNode) =>
           ((syntaxNode as InvocationExpressionSyntax)?.Expression as MemberAccessExpressionSyntax)?.Expression;

    public static IEnumerable<ExpressionSyntax> NestedInvocations(this SyntaxNode syntaxNode)
    {
        var expressionSyntax = GetNextNestedInvocation(syntaxNode);

        while (expressionSyntax != null)
        {
            yield return expressionSyntax;

            expressionSyntax = GetNextNestedInvocation(expressionSyntax);
        }

        ExpressionSyntax GetNextNestedInvocation(SyntaxNode syntaxNode) =>
            ((syntaxNode as InvocationExpressionSyntax)?.Expression as MemberAccessExpressionSyntax)?.Expression;
    }

    public static ArgumentSyntax GetParentArgumentSyntaxIfExists(this SyntaxNode syntaxNode)
    {
        while (syntaxNode != null)
        {
            if (syntaxNode is ArgumentSyntax argumentSyntax)
            {
                return argumentSyntax;
            }

            syntaxNode = syntaxNode.Parent;
        }

        return null;
    }

    public static IdentifierNameSyntax GetSingleIdentifier(this SyntaxNode syntaxNode, string identifierName) =>
        syntaxNode.DescendantNodes().OfType<IdentifierNameSyntax>().Single(i => i.Identifier.Text == identifierName);

    public static ClassDeclarationSyntax GetSingleClassDeclaration(this SyntaxNode syntaxNode, string className) =>
        syntaxNode.DescendantNodes().OfType<ClassDeclarationSyntax>().Single(i => i.Identifier.Text == className);

    public static MethodDeclarationSyntax GetSingleMethod(this SyntaxNode syntaxNode, string name) =>
        syntaxNode.DescendantNodes().OfType<MethodDeclarationSyntax>().Single(m => m.Identifier.Text == name);

    public static SyntaxNode GetTopMostInvocationOrBinaryExpressionSyntax(SimpleNameSyntax identifier, IdentifierNameSyntax[] lambdaAndQueryIdentifiers)
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

                if (lambdaAndQueryIdentifiers != null)
                {
                    if (binaryExpressionSyntax.Left != previousSyntaxNode &&
                        lambdaAndQueryIdentifiers.Any(l => binaryExpressionSyntax.Left.Contains(l)))
                    {
                        return false;
                    }

                    if (binaryExpressionSyntax.Right != previousSyntaxNode &&
                        lambdaAndQueryIdentifiers.Any(l => binaryExpressionSyntax.Right.Contains(l)))
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
