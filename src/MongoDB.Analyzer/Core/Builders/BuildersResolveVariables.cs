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
        Dictionary<string, ExpressionAnalysisContext> variableValues,
        Dictionary<string, SyntaxNode> functions,
        SemanticModel semanticModel);

    private static bool ParseExpression(SyntaxNode expression,
                                        SyntaxNode RHS,
                                        Dictionary<string, ExpressionAnalysisContext> variableValues,
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

        if(expression is IdentifierNameSyntax identifier)
        {
            var symbolInfo = semanticModel.GetSymbolInfo(expression);
            bool containedInLambda = symbolInfo.Symbol.IsContainedInLambda(RHS);
            bool isMemberAccess = identifier.Parent is MemberAccessExpressionSyntax;
            bool existsInContext = variableValues.ContainsKey(expression.ToString());
            canEvaluate = existsInContext && !containedInLambda && !isMemberAccess;
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
        SyntaxKind syntaxKind = syntaxNode.Kind();
        Dictionary<string, ExpressionAnalysisContext> variableValues = processContext.variableValues;
        bool canEvaluate = true;

        var semanticModel = processContext.semanticModel;

        if(RHS is VariableDeclarationSyntax variableDeclarationSyntax)
        {
            foreach (var declaration in variableDeclarationSyntax.Variables)
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
        else if(RHS is AssignmentExpressionSyntax)
        {
            syntaxKind = RHS.Kind();
            while(RHS is AssignmentExpressionSyntax assignmentExpression)
            {
                var LHS = assignmentExpression.Left;
                variableNames.Add(LHS.ToString());
                RHS = assignmentExpression.Right;
            }
        }
        else
        {
            canEvaluate = false;
        }

        var childNodes = new List<SyntaxNode>();
        var functionArguments = new List<string>();
        canEvaluate = ParseExpression(RHS, RHS, variableValues, buildersToExpressionContext, childNodes, semanticModel)
                        && canEvaluate;

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
                if(childNode.Parent is ArgumentSyntax)
                {
                    functionArguments.Add(childNodeName);
                }
                var analysisContext = variableValues[childNodeName];
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
                if(variableValues.ContainsKey(variable))
                {
                    variableValues.Remove(variable);
                }
            }
            foreach(var variable in functionArguments)
            {
                if(variableValues.ContainsKey(variable))
                {
                    variableValues.Remove(variable);
                }
            }
            return variableNames;
        }

        var result = RHS;
        bool existsInContext = variableValues.ContainsKey(RHS.ToString());
        if(!existsInContext && !buildersToExpressionContext.ContainsKey(RHS.ToString()))
        {
            result = RHS.ReplaceNodes(nodesRemapping.Keys, (n, _) => nodesRemapping[n]);
        }
        else if(existsInContext)
        {
            var context = variableValues[RHS.ToString()];
            result = context.Node.RewrittenExpression;
        }
        else if(buildersToExpressionContext.ContainsKey(RHS.ToString()))
        {
            result = buildersToExpressionContext[RHS.ToString()].Node.RewrittenExpression;
        }

        if(syntaxKind == SyntaxKind.AndAssignmentExpression)
        {
            var firstOperand = result;
            existsInContext = variableValues.ContainsKey(variableNames.FirstOrDefault());
            if(existsInContext)
            {
                var context = variableValues[variableNames.FirstOrDefault()];
                firstOperand = context.Node.RewrittenExpression;
            }
            result = SyntaxFactory.BinaryExpression(SyntaxKind.BitwiseAndExpression, firstOperand as ExpressionSyntax, result as ExpressionSyntax);
        }
        else if(syntaxKind == SyntaxKind.OrAssignmentExpression)
        {
            var firstOperand = result;
            existsInContext = variableValues.ContainsKey(variableNames.FirstOrDefault());
            if(existsInContext)
            {
                var context = variableValues[variableNames.FirstOrDefault()];
                firstOperand = context.Node.RewrittenExpression;
            }
            result = SyntaxFactory.BinaryExpression(SyntaxKind.BitwiseOrExpression, firstOperand as ExpressionSyntax, result as ExpressionSyntax);
        }

        ExpressionAnalysisContext variableInfo = new ExpressionAnalysisContext(new ExpressionAnalysisNode(RHS, argumentTypeName, result, new ConstantsMapper()));
        foreach(var variableName in variableNames)
        {
            if(!variableValues.ContainsKey(variableName) && canEvaluate)
            {
                variableValues.Add(variableName, variableInfo);
            }
            else if(canEvaluate)
            {
                variableValues[variableName] = variableInfo;
            }
        }
        return variableNames;
    }

    private static List<string> ProcessNodes(List<ExpressionAnalysisContext> analysisContexts,
                                    Dictionary<string, ExpressionAnalysisContext> buildersToExpressionContext,
                                    SyntaxNode node,
                                    ProcessContext current)
    {
        var variables = new List<string>();
        var localMethods = current.functions;
        var semanticModel = current.semanticModel;

        Dictionary<string, ExpressionAnalysisContext> variableValues = current.variableValues;

        if(node is LocalDeclarationStatementSyntax localDeclarationStatementSyntax)
        {
            node = localDeclarationStatementSyntax.Declaration;
        }

        if(node is ExpressionStatementSyntax expressionStatementSyntax)
        {
            node = expressionStatementSyntax.Expression;
        }

        ISymbol typeInfo = null;
        if(node is VariableDeclarationSyntax variableDeclaration)
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

        if(typeInfo is ITypeSymbol typeSymbol
            && SymbolExtensions.IsBuilderDefinition(typeSymbol))
        {
            List<String> variablesValuesChanged = StoreValue(analysisContexts, buildersToExpressionContext, node, current);
            return variablesValuesChanged;
        }

        var childNodes = new List<SyntaxNode>();
        if(localMethods.ContainsKey(node.ToString()) && node.Parent is InvocationExpressionSyntax)
        {
            SyntaxNode localMethod = localMethods[node.ToString()];
            ProcessContext methodProcessContext = new ProcessContext(new Dictionary<string, ExpressionAnalysisContext>(), new Dictionary<string, SyntaxNode>(), semanticModel);
            variables = ProcessNodes(analysisContexts, buildersToExpressionContext, localMethod, methodProcessContext);
            localMethods.Remove(node.ToString());
            return variables;
        }
        var descendantNodes = node.ChildNodes();
        foreach(var childNode in descendantNodes)
        {
            if(childNode is LocalFunctionStatementSyntax localMethod
                && !localMethods.ContainsKey(localMethod.Identifier.ToString()))
            {
                localMethods.Add(localMethod.Identifier.ToString(), localMethod);
            }
            else
            {
                childNodes.Add(childNode);
            }
        }
        foreach(var child in childNodes)
        {
            ProcessContext childProcessContext = new ProcessContext(variableValues, localMethods, semanticModel);
            var list = ProcessNodes(analysisContexts, buildersToExpressionContext, child, childProcessContext);
            variables.AddRange(list);
        }

        foreach(var localMethod in localMethods)
        {
            ProcessContext childProcessContext = new ProcessContext(new Dictionary<string, ExpressionAnalysisContext>(), new Dictionary<string, SyntaxNode>(), semanticModel);
            ProcessNodes(analysisContexts, buildersToExpressionContext, localMethod.Value, childProcessContext);
        }

        foreach(var variable in variables)
        {
            if(variableValues.ContainsKey(variable))
            {
                variableValues.Remove(variable);
            }
        }

        return variables;
    }

    private static void ProcessTree(List<ExpressionAnalysisContext> analysisContexts,
                                    Dictionary<string, ExpressionAnalysisContext> buildersToExpressionContext,
                                    SemanticModel semanticModel,
                                    SyntaxNode node,
                                    ProcessContext current)
    {
        var variableValues = current.variableValues;
        var functions = current.functions;
        if(node is MethodDeclarationSyntax)
        {
            ProcessContext methodProcessContext = new ProcessContext(variableValues, functions, semanticModel);
            ProcessNodes(analysisContexts, buildersToExpressionContext, node, methodProcessContext);
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
        var syntaxTree = semanticModel.SyntaxTree;
        var root = syntaxTree.GetRoot();
        ProcessContext processContext = new ProcessContext(new Dictionary<string, ExpressionAnalysisContext>(),
                                                            new Dictionary<string, SyntaxNode>(), semanticModel);
        ProcessTree(analysisContexts, builderToAnalysisContextMap, semanticModel, root, processContext);
    }
}
