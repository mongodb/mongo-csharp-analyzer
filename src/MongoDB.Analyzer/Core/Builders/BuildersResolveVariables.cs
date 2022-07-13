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
    private record VariableContext(
        int level,
        ExpressionAnalysisContext analysisContext);

    private record ProcessContext(
        int level,
        Dictionary<string, VariableContext> variableValues,
        SemanticModel semanticModel);

    private static ExpressionAnalysisContext ExistsInContext(Dictionary<string, VariableContext> variableValues,
                                                             string variableName)
    {
        if (!variableValues.ContainsKey(variableName))
        {
            return null;
        }
        return variableValues[variableName].analysisContext;
    }

    private static bool ParseExpression(SyntaxNode expression,
                                        SyntaxNode RHS,
                                        Dictionary<string, VariableContext> variableValues,
                                        Dictionary<string, ExpressionAnalysisContext> buildersToExpressionContext,
                                        List<SyntaxNode> childNodes,
                                        SemanticModel semanticModel)
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

        if (expression is IdentifierNameSyntax)
        {
            var symbolInfo = semanticModel.GetSymbolInfo(expression);
            bool containedInLambda = symbolInfo.Symbol.IsContainedInLambda(RHS);
            canEvaluate = ExistsInContext(variableValues, expression.ToString()) != null
                            && !containedInLambda;
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
                canEvaluateChildNode = ParseExpression(descendantNode, RHS, variableValues,
                                                        buildersToExpressionContext, childNodes, semanticModel) && canEvaluateChildNode;
            }
            if(expression is BinaryExpressionSyntax)
            {
                canEvaluate = canEvaluateChildNode;
            }
        }
        return canEvaluate;
    }

    private static List<String> StoreValue(List<ExpressionAnalysisContext> analysisContexts,
                                            Dictionary<string, ExpressionAnalysisContext> buildersToExpressionContext,
                                            SyntaxNode syntaxNode,
                                             ProcessContext processContext)
    {
        var variableNames = new List<string>();
        var RHS = syntaxNode;
        int level = processContext.level;
        SyntaxKind syntaxKind = syntaxNode.Kind();
        Dictionary<string, VariableContext> variableValues = processContext.variableValues;
        var semanticModel = processContext.semanticModel;

        if(RHS is VariableDeclarationSyntax variableDeclarationSyntax)
        {
            foreach(var declaration in variableDeclarationSyntax.Variables)
            {
                variableNames.Add(declaration.Identifier.ToString());
                StoreValue(analysisContexts, buildersToExpressionContext, declaration, processContext);
            }
            return variableNames;
        }
        else if(RHS is VariableDeclaratorSyntax variableDeclaratorSyntax)
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
        bool canEvaluate = ParseExpression(RHS, RHS, variableValues, buildersToExpressionContext, childNodes, semanticModel);

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
                var analysisContext = variableValues[childNodeName].analysisContext;
                var rewrittenExpression = analysisContext.Node.RewrittenExpression;
                argumentTypeName = analysisContext.Node.ArgumentTypeName;
                var constantsMapper = analysisContext.Node.ConstantsRemapper;
                nodesRemapping.Add(childNode, rewrittenExpression);
                ExpressionAnalysisContext diagnostic = new ExpressionAnalysisContext(new ExpressionAnalysisNode(childNode, argumentTypeName, rewrittenExpression, constantsMapper));
                analysisContexts.Add(diagnostic);
            }
        }

        if(!canEvaluate)
        {
            foreach(var variable in variableNames)
            {
                if (variableValues.ContainsKey(variable))
                {
                    variableValues.Remove(variable);
                }
            }
            return variableNames;
        }

        var result = RHS;
        ExpressionAnalysisContext context = ExistsInContext(variableValues, RHS.ToString());
        if(context == null && !buildersToExpressionContext.ContainsKey(RHS.ToString()))
        {
            result = RHS.ReplaceNodes(nodesRemapping.Keys, (n, _) => nodesRemapping[n]);
        }
        else if(context != null)
        {
            result = context.Node.RewrittenExpression;
        }
        else if(buildersToExpressionContext.ContainsKey(RHS.ToString()))
        {
            result = buildersToExpressionContext[RHS.ToString()].Node.RewrittenExpression;
        }

        if(syntaxKind == SyntaxKind.AndAssignmentExpression)
        {
            var firstOperand = result;
            context = ExistsInContext(variableValues, variableNames.First());
            if(context != null)
            {
                firstOperand = context.Node.RewrittenExpression;
            }
            result = SyntaxFactory.BinaryExpression(SyntaxKind.BitwiseAndExpression, firstOperand as ExpressionSyntax, result as ExpressionSyntax);
        }
        else if(syntaxKind == SyntaxKind.OrAssignmentExpression)
        {
            var firstOperand = result;
            context = ExistsInContext(variableValues, variableNames.First());
            if(context != null)
            {
                firstOperand = context.Node.RewrittenExpression;
            }
            result = SyntaxFactory.BinaryExpression(SyntaxKind.BitwiseOrExpression, firstOperand as ExpressionSyntax, result as ExpressionSyntax);
        }

        ExpressionAnalysisContext variableInfo = new ExpressionAnalysisContext(new ExpressionAnalysisNode(RHS, argumentTypeName, result, new ConstantsMapper()));
        foreach(var variableName in variableNames)
        {
            if(!variableValues.ContainsKey(variableName) && canEvaluate)
            {
                variableValues.Add(variableName, new VariableContext(level, variableInfo));
            }
            else if(canEvaluate)
            {
                variableValues[variableName] = new VariableContext(level, variableInfo);
            }
        }
        return variableNames;
    }

    private static void ProcessNodes(List<ExpressionAnalysisContext> analysisContexts,
                                    Dictionary<string, ExpressionAnalysisContext> buildersToExpressionContext,
                                    SemanticModel semanticModel,
                                    SyntaxNode node,
                                    ProcessContext current,
                                    List<string> variables)
    {
        int level = current.level;
        Dictionary<string, VariableContext> variableValues = current.variableValues;

        if(node is LocalDeclarationStatementSyntax localDeclarationStatementSyntax)
        {
            node = localDeclarationStatementSyntax.Declaration;
        }

        if(node is ExpressionStatementSyntax expressionStatementSyntax)
        {
            node = expressionStatementSyntax.Expression;
        }

        ISymbol typeInfo = null;
        if (node is VariableDeclarationSyntax variableDeclaration)
        {
            typeInfo = semanticModel.GetTypeInfo(variableDeclaration.Type).Type;
        }
        if(node is AssignmentExpressionSyntax assignmentExpression)
        {
            typeInfo = semanticModel.GetTypeInfo(assignmentExpression.Left).Type;
        }

        if(node is IdentifierNameSyntax identifierName)
        {
            typeInfo = semanticModel.GetTypeInfo(identifierName).Type;
        }

        if (typeInfo != null && typeInfo is ITypeSymbol typeSymbol
            && SymbolExtensions.IsBuilderDefinition(typeSymbol))
        {
            List<String> variablesValuesChanged = StoreValue(analysisContexts, buildersToExpressionContext, node, current);
            variables.AddRange(variablesValuesChanged); 
            return;
        }

        int newScope = level;
        var childNodes = node.ChildNodes();
        foreach (var child in childNodes)
        {
            if(child is BlockSyntax)
            {
                newScope = level + 1;
                ProcessContext childProcessContext = new ProcessContext(newScope, variableValues, current.semanticModel);
                ProcessNodes(analysisContexts, buildersToExpressionContext, semanticModel,
                                child, childProcessContext, new List<string>());
            }
            else
            {
                ProcessNodes(analysisContexts, buildersToExpressionContext, semanticModel, child, current, variables);
            } 
        }

        if(node is BlockSyntax)
        {
            foreach(var variable in variables)
            {
                if (variableValues.ContainsKey(variable))
                {
                    variableValues.Remove(variable);
                }
            }
        }
    }

    private static void ProcessTree(List<ExpressionAnalysisContext> analysisContexts,
                                    Dictionary<string, ExpressionAnalysisContext> buildersToExpressionContext,
                                    SemanticModel semanticModel,
                                    SyntaxNode node,
                                    ProcessContext current)
    {
        int level = current.level;
        Dictionary<string, VariableContext> variableValues = current.variableValues;
        if(node is MethodDeclarationSyntax)
        {
            ProcessContext childProcessContext = new ProcessContext(level, variableValues, semanticModel);
            ProcessNodes(analysisContexts, buildersToExpressionContext, semanticModel, node, childProcessContext, new List<string>());
            return;
        }

        var childNodes = node.ChildNodes();
        foreach(var childNode in childNodes)
        {
            ProcessTree(analysisContexts, buildersToExpressionContext, semanticModel, childNode, current);
        }
    }

    public static void ResolveVariables(List<ExpressionAnalysisContext> analysisContexts,
                                        Dictionary<string, ExpressionAnalysisContext> builderToAnalysisContextMap,
                                        SemanticModel semanticModel)
    {
        var root = semanticModel.SyntaxTree.GetRoot();
        ProcessContext processContext = new ProcessContext(0, new Dictionary<string, VariableContext>(), semanticModel);
        ProcessTree(analysisContexts, builderToAnalysisContextMap, semanticModel, root, processContext);
    }
}
