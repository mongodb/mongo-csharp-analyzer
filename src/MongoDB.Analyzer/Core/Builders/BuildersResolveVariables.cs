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

namespace MongoDB.Analyzer.Core.Builders;

internal static class BuildersResolveVariables
{
    private record ProcessContext(
        int level,
        Dictionary<int, Dictionary<string, ExpressionAnalysisContext>> variableValues);

    private static int ExistsInContext(Dictionary<int, Dictionary<string, ExpressionAnalysisContext>> variableValues,
                                        int currentLevel,
                                        string variableName)
    {
        int level = currentLevel;
        while(level >= 0)
        {
            if(variableValues.ContainsKey(level) && variableValues[level].ContainsKey(variableName))
            {
                break;
            }
            level--;
        }
        return level;
    }

    private static bool ParseExpression(SyntaxNode expression,
                                          int currentLevel,
                                          Dictionary<int, Dictionary<string, ExpressionAnalysisContext>> variableValues,
                                          Dictionary<string, ExpressionAnalysisContext> buildersToExpressionContext,
                                          List<SyntaxNode> childNodes)
    {
        var canEvaluate = true;
        while(expression is ParenthesizedExpressionSyntax parenthesizedExpression)
        {
            expression = parenthesizedExpression.Expression;
        }

        if(expression is not InvocationExpressionSyntax &&
           expression is not BinaryExpressionSyntax &&
           expression is not IdentifierNameSyntax)
        {
            canEvaluate = default;
        }

        if(expression is InvocationExpressionSyntax || expression is BinaryExpressionSyntax)
        {
            canEvaluate = buildersToExpressionContext.ContainsKey(expression.ToString());
        }

        if(expression is IdentifierNameSyntax)
        {
            canEvaluate = ExistsInContext(variableValues, currentLevel, expression.ToString()) != -1;
        }

        if(canEvaluate)
        {
            childNodes.Add(expression);
        }
        else
        {
            var descendantNodes = expression.ChildNodes();
            var canEvaluateChildNode = true;
            foreach(var descendantNode in descendantNodes)
            {
                canEvaluateChildNode = ParseExpression(descendantNode, currentLevel, variableValues,
                                                        buildersToExpressionContext, childNodes) && canEvaluateChildNode;
            }
            if(expression is BinaryExpressionSyntax)
            {
                canEvaluate = canEvaluateChildNode;
            }
        }
        return canEvaluate;
    }

    private static void StoreValue(List<ExpressionAnalysisContext> analysisContexts,
                                            Dictionary<string, ExpressionAnalysisContext> buildersToExpressionContext,
                                            SyntaxNode syntaxNode,
                                             ProcessContext processContext)
    {
        var variableNames = new List<string>();
        var RHS = syntaxNode;
        int level = processContext.level;
        SyntaxKind syntaxKind = syntaxNode.Kind();
        Dictionary<int, Dictionary<string, ExpressionAnalysisContext>> variableValues = processContext.variableValues;

        if(RHS is VariableDeclaratorSyntax variableDeclaratorSyntax)
        {
            var LHS = variableDeclaratorSyntax.Identifier;
            variableNames.Add(LHS.ToString());
            RHS = variableDeclaratorSyntax.Initializer.Value;
        }
        else
        {
            syntaxKind = RHS.Kind();
            while(RHS is AssignmentExpressionSyntax assignmentExpression)
            {
                var LHS = assignmentExpression.Left;
                variableNames.Add(LHS.ToString());
                RHS = assignmentExpression.Right;
            }
        }

        var childNodes = new List<SyntaxNode>();
        bool canEvaluate = ParseExpression(RHS, level, variableValues,
                                            buildersToExpressionContext, childNodes);

        Dictionary<SyntaxNode, SyntaxNode> nodesRemapping = new Dictionary<SyntaxNode, SyntaxNode>();
        var argumentTypeName = "";
        foreach(var childNode in childNodes)
        {
            string childNodeName = childNode.ToString();
            if(buildersToExpressionContext.ContainsKey(childNodeName))
            {
                var rewrittenExpression = buildersToExpressionContext[childNodeName].Node.RewrittenExpression;
                argumentTypeName = buildersToExpressionContext[childNodeName].Node.ArgumentTypeName;
                nodesRemapping.Add(childNode, rewrittenExpression);
            }
            else
            {
                var recentLevel = ExistsInContext(variableValues, level, childNodeName);
                var rewrittenExpression = variableValues[recentLevel][childNodeName].Node.RewrittenExpression;
                argumentTypeName = variableValues[recentLevel][childNodeName].Node.ArgumentTypeName;
                var constantsMapper = variableValues[recentLevel][childNodeName].Node.ConstantsRemapper;
                nodesRemapping.Add(childNode, rewrittenExpression);
                ExpressionAnalysisContext diagnostic = new ExpressionAnalysisContext(new ExpressionAnalysisNode(childNode, argumentTypeName, rewrittenExpression, constantsMapper));
                analysisContexts.Add(diagnostic);
            }
        }

        if(!canEvaluate)
        {
            foreach(var variable in variableNames)
            {
                if(variableValues[level].ContainsKey(variable))
                {
                    variableValues[level].Remove(variable);
                }
            }
            return;
        }

        var result = RHS;
        int context = ExistsInContext(variableValues, level, RHS.ToString());
        if(context == -1 && !buildersToExpressionContext.ContainsKey(RHS.ToString()))
        {
            result = RHS.ReplaceNodes(nodesRemapping.Keys, (n, _) => nodesRemapping[n]);
        }
        else if(context != -1)
        {
            result = variableValues[context][RHS.ToString()].Node.RewrittenExpression;
        }
        else if(buildersToExpressionContext.ContainsKey(RHS.ToString()))
        {
            result = buildersToExpressionContext[RHS.ToString()].Node.RewrittenExpression;
        }

        if(syntaxKind == SyntaxKind.AndAssignmentExpression)
        {
            var firstOperand = result;
            context = ExistsInContext(variableValues, level, variableNames.First());
            if(context != -1)
            {
                firstOperand = variableValues[context][variableNames.First()].Node.RewrittenExpression;
            }
            result = SyntaxFactory.BinaryExpression(SyntaxKind.BitwiseAndExpression, firstOperand as ExpressionSyntax, result as ExpressionSyntax);
        }
        else if(syntaxKind == SyntaxKind.OrAssignmentExpression)
        {
            var firstOperand = result;
            context = ExistsInContext(variableValues, level, variableNames.First());
            if(context != -1)
            {
                firstOperand = variableValues[context][variableNames.First()].Node.RewrittenExpression;
            }
            result = SyntaxFactory.BinaryExpression(SyntaxKind.BitwiseOrExpression, firstOperand as ExpressionSyntax, result as ExpressionSyntax);
        }

        ExpressionAnalysisContext variableInfo = new ExpressionAnalysisContext(new ExpressionAnalysisNode(RHS, argumentTypeName, result, new ConstantsMapper()));
        foreach(var variableName in variableNames)
        {
            if(!variableValues[level].ContainsKey(variableName) && canEvaluate)
            {
                variableValues[level].Add(variableName, variableInfo);
            }
            else if(canEvaluate)
            {
                variableValues[level][variableName] = variableInfo;
            }
        }
    }

    private static void ProcessNodes(List<ExpressionAnalysisContext> analysisContexts,
                                    Dictionary<string, ExpressionAnalysisContext> buildersToExpressionContext,
                                    SyntaxNode node,
                                    ProcessContext current)
    {
        int level = current.level;
        Dictionary<int, Dictionary<string, ExpressionAnalysisContext>> variableValues = current.variableValues;

        if(!variableValues.ContainsKey(level))
        {
            variableValues.Add(level, new Dictionary<string, ExpressionAnalysisContext>());
        }

        if(node is LocalDeclarationStatementSyntax localDeclarationStatementSyntax)
        {
            VariableDeclarationSyntax variableDeclaration = localDeclarationStatementSyntax.Declaration;
            foreach(var declaration in variableDeclaration.Variables)
            {
                StoreValue(analysisContexts, buildersToExpressionContext, declaration, current);
            }
            return;
        }
        else if(node is ExpressionStatementSyntax expressionStatementSyntax)
        {
            StoreValue(analysisContexts, buildersToExpressionContext, expressionStatementSyntax.Expression, current);
            return;
        }

        var childNodes = node.ChildNodes();
        foreach(var child in childNodes)
        {
            ProcessContext childProcessContext = new ProcessContext(level + 1, variableValues);
            ProcessNodes(analysisContexts, buildersToExpressionContext, child, childProcessContext);
        }

        variableValues.Remove(level + 1);
    }

    public static void ResolveVariables(List<ExpressionAnalysisContext> analysisContexts, Dictionary<string, ExpressionAnalysisContext> builderToAnalysisContextMap, SyntaxNode root)
    {
        ProcessNodes(analysisContexts, builderToAnalysisContextMap, root, new ProcessContext(0, new Dictionary<int, Dictionary<string, ExpressionAnalysisContext>>()));
    }
}
