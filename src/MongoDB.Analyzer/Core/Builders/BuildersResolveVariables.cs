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
        Dictionary<string, SyntaxNode> functions,
        SemanticModel semanticModel);

    private static void FindLocalMethods(SyntaxNode syntaxNode, Dictionary<string, SyntaxNode> localMethods)
    {
        var childNodes = syntaxNode.ChildNodes();
        foreach (var childNode in childNodes)
        {
            string childNodeName = childNode.ToString();
            if (!localMethods.ContainsKey(childNodeName) && childNode is LocalFunctionStatementSyntax localFunction)
            {
                localMethods.Add(localFunction.Identifier.ToString(), childNode);
            }
        }
    }

    private static void DisplayDiagnostic(List<ExpressionAnalysisContext> analysisContexts,
                                          ProcessContext processContext,
                                          SyntaxNode syntaxNode)
    {
        var variableValues = processContext.variableValues;
        if (!variableValues.ContainsKey(syntaxNode.ToString()))
        {
            return;
        }
        var expressionAnalysisContext = variableValues[syntaxNode.ToString()].analysisContext;
        var rewrittenExpression = expressionAnalysisContext.Node.RewrittenExpression;
        var argumentTypeName = expressionAnalysisContext.Node.ArgumentTypeName;
        var constantsMapper = expressionAnalysisContext.Node.ConstantsRemapper;
        ExpressionAnalysisContext diagnostic = new ExpressionAnalysisContext(new ExpressionAnalysisNode(syntaxNode, argumentTypeName, rewrittenExpression, constantsMapper));
        analysisContexts.Add(diagnostic);
    }

    private static bool CanEvaluate(ProcessContext processContext,
                                    SyntaxNode syntaxNode,
                                    Dictionary<string, ExpressionAnalysisContext> buildersToExpressionContext,
                                    List<SyntaxNode> buildersNodes)
    {
        var canEvaluate = true;
        var variableValues = processContext.variableValues;

        while (syntaxNode is ParenthesizedExpressionSyntax parenthesizedExpression)
        {
            syntaxNode = parenthesizedExpression.Expression;
        }
        if (syntaxNode is not InvocationExpressionSyntax &&
           syntaxNode is not BinaryExpressionSyntax &&
           syntaxNode is not IdentifierNameSyntax)
        {
            canEvaluate = default;
        }

        if (syntaxNode is InvocationExpressionSyntax || syntaxNode is BinaryExpressionSyntax)
        {
            canEvaluate = buildersToExpressionContext.ContainsKey(syntaxNode.ToString());
        }

        if (syntaxNode is IdentifierNameSyntax)
        {
            bool existsInContext = variableValues.ContainsKey(syntaxNode.ToString());
            canEvaluate = existsInContext;
        }

        if (canEvaluate)
        {
            buildersNodes.Add(syntaxNode);
        }
        else
        {
            var descendantNodes = syntaxNode.ChildNodes();
            var canEvaluateChildNode = true;
            foreach (var descendantNode in descendantNodes)
            {
                canEvaluateChildNode = CanEvaluate(processContext, descendantNode,
                                                    buildersToExpressionContext, buildersNodes) && canEvaluateChildNode;
            }
            if (syntaxNode is BinaryExpressionSyntax)
            {
                canEvaluate = canEvaluateChildNode;
            }
        }
        return canEvaluate;
    }

    private static List<String> StoreValue(List<ExpressionAnalysisContext> analysisContexts,
                                           Dictionary<string, ExpressionAnalysisContext> buildersToExpressionContext,
                                           SyntaxNode syntaxNode,
                                           ProcessContext processContext,
                                           List<SyntaxNode> buildersNodes)
    {
        var variableNames = new List<string>();
        var variablesList = new List<string>();
        var RHS = syntaxNode;
        var variableValues = processContext.variableValues;
        var semanticModel = processContext.semanticModel;
        bool canEvaluate = true;
        var level = processContext.level;
        var methodLevels = new List<int>();

        SyntaxKind syntaxKind = syntaxNode.Kind();
        if (RHS is VariableDeclaratorSyntax variableDeclaratorSyntax)
        {
            var LHS = variableDeclaratorSyntax.Identifier;
            variableNames.Add(LHS.ToString());
            RHS = variableDeclaratorSyntax.Initializer.Value;
            methodLevels.Add(level);
        }
        else if (RHS is AssignmentExpressionSyntax)
        {
            syntaxKind = RHS.Kind();
            while (RHS is AssignmentExpressionSyntax assignmentExpression)
            {
                var LHS = assignmentExpression.Left;
                variableNames.Add(LHS.ToString());
                RHS = assignmentExpression.Right;
                if (variableValues.ContainsKey(LHS.ToString()))
                {
                    methodLevels.Add(level);
                    var variableLevel = variableValues[LHS.ToString()].level;
                    if(variableLevel < level)
                    {
                        variablesList.Add(LHS.ToString());
                    }
                }
                else
                {
                    methodLevels.Add(-1);
                    variablesList.Add(LHS.ToString());
                }
            }
        }

        var functionArguments = new List<string>();

        Dictionary<SyntaxNode, SyntaxNode> nodesRemapping = new Dictionary<SyntaxNode, SyntaxNode>();
        var argumentTypeName = "";
        foreach (var builderNode in buildersNodes)
        {
            string builderNodeName = builderNode.ToString();
            if (buildersToExpressionContext.ContainsKey(builderNodeName))
            {
                var rewrittenExpression = buildersToExpressionContext[builderNodeName].Node.RewrittenExpression;
                argumentTypeName = buildersToExpressionContext[builderNodeName].Node.ArgumentTypeName;
                nodesRemapping.Add(builderNode, rewrittenExpression);
            }
            else
            {
                var analysisContext = variableValues[builderNodeName].analysisContext;
                var rewrittenExpression = analysisContext.Node.RewrittenExpression;
                argumentTypeName = analysisContext.Node.ArgumentTypeName;
                var constantsMapper = analysisContext.Node.ConstantsRemapper;
                nodesRemapping.Add(builderNode, rewrittenExpression);
                ExpressionAnalysisContext diagnostic = new ExpressionAnalysisContext(new ExpressionAnalysisNode(builderNode, argumentTypeName, rewrittenExpression, constantsMapper));
                analysisContexts.Add(diagnostic);
            }
        }

        var result = RHS;
        bool existsInContext = variableValues.ContainsKey(RHS.ToString());
        if (!existsInContext && !buildersToExpressionContext.ContainsKey(RHS.ToString()))
        {
            result = RHS.ReplaceNodes(nodesRemapping.Keys, (n, _) => nodesRemapping[n]);
        }
        else if (existsInContext)
        {
            var context = variableValues[RHS.ToString()].analysisContext;
            result = context.Node.RewrittenExpression;
        }
        else if (buildersToExpressionContext.ContainsKey(RHS.ToString()))
        {
            result = buildersToExpressionContext[RHS.ToString()].Node.RewrittenExpression;
        }

        if (syntaxKind == SyntaxKind.AndAssignmentExpression)
        {
            var firstOperand = result;
            existsInContext = variableValues.ContainsKey(variableNames.FirstOrDefault());
            if (existsInContext)
            {
                var context = variableValues[variableNames.FirstOrDefault()].analysisContext;
                firstOperand = context.Node.RewrittenExpression;
            }
            result = SyntaxFactory.BinaryExpression(SyntaxKind.BitwiseAndExpression, firstOperand as ExpressionSyntax, result as ExpressionSyntax);
        }
        else if (syntaxKind == SyntaxKind.OrAssignmentExpression)
        {
            var firstOperand = result;
            existsInContext = variableValues.ContainsKey(variableNames.FirstOrDefault());
            if (existsInContext)
            {
                var context = variableValues[variableNames.FirstOrDefault()].analysisContext;
                firstOperand = context.Node.RewrittenExpression;
            }
            result = SyntaxFactory.BinaryExpression(SyntaxKind.BitwiseOrExpression, firstOperand as ExpressionSyntax, result as ExpressionSyntax);
        }

        ExpressionAnalysisContext variableInfo = new ExpressionAnalysisContext(new ExpressionAnalysisNode(RHS, argumentTypeName, result, new ConstantsMapper()));
        for (int index = 0; index < variableNames.Count; index ++)
        {
            var variableName = variableNames.ElementAt(index);
            var methodLevel = methodLevels.ElementAt(index);
            if (!variableValues.ContainsKey(variableName))
            {
                variableValues.Add(variableName, new VariableContext(methodLevel, variableInfo));
            }
            else
            {
                variableValues[variableName] = new VariableContext(methodLevel, variableInfo);
            }
        }
        return variablesList;
    }

    private static List<string> ProcessNodes(List<ExpressionAnalysisContext> analysisContexts,
                                    Dictionary<string, ExpressionAnalysisContext> buildersToExpressionContext,
                                    SyntaxNode node,
                                    SyntaxNode statement,
                                    ProcessContext current)
    {
        var level = current.level;
        var localMethods = current.functions;
        var semanticModel = current.semanticModel;
        var variableValues = current.variableValues;
        var variables = new List<string>();
        var buildersNodes = new List<SyntaxNode>();
        var canEvaluate = false;
        bool displayDiagnostic = false;

        SyntaxNode displayNode = null;
        SyntaxNode statementNode = null;

        if (node is LocalDeclarationStatementSyntax localDeclarationStatementSyntax)
        {
            node = localDeclarationStatementSyntax.Declaration;
            statementNode = localDeclarationStatementSyntax;

        }

        if (node is ExpressionStatementSyntax expressionStatementSyntax)
        {
            node = expressionStatementSyntax.Expression;
            statementNode = expressionStatementSyntax;
        }

        if (node is VariableDeclarationSyntax variableDeclaration)
        {
            foreach (var declaration in variableDeclaration.Variables)
            {
                ProcessContext childProcessContext = new ProcessContext(level, variableValues, localMethods, semanticModel);
                var list = ProcessNodes(analysisContexts, buildersToExpressionContext, declaration, statementNode, childProcessContext);
                variables.AddRange(list);
            }
            return variables;
        }

        if (node is VariableDeclaratorSyntax variableDeclarator)
        {
            canEvaluate = CanEvaluate(current, variableDeclarator.Initializer.Value, buildersToExpressionContext, buildersNodes);
            if (!canEvaluate)
            {
                var variableRemove = variableDeclarator.Identifier.ToString();
                node = variableDeclarator.Initializer.Value;
                variables.Add(variableRemove);
            }
        }
        if (node is AssignmentExpressionSyntax assignmentExpression)
        {
            var variableNames = new List<string>();
            ExpressionSyntax RHS = assignmentExpression;
            while (RHS is AssignmentExpressionSyntax assignment)
            {
                variableNames.Add(assignment.Left.ToString());
                RHS = assignment.Right;
            }
            canEvaluate = CanEvaluate(current, RHS, buildersToExpressionContext, buildersNodes);
            if (!canEvaluate)
            {
                node = assignmentExpression.Right;
                variables.AddRange(variableNames);
            }
        }

        if (node is IdentifierNameSyntax identifierName)
        {
            ISymbol typeInfo = semanticModel.GetTypeInfo(identifierName).Type;

            var symbolInfo = semanticModel.GetSymbolInfo(identifierName);
            bool isMemberAccess = identifierName.Parent is MemberAccessExpressionSyntax;
            bool existsInContext = variableValues.ContainsKey(identifierName.ToString());
            bool isBuilders = typeInfo is ITypeSymbol typeSymbol && SymbolExtensions.IsBuilderDefinition(typeSymbol);
            bool containedInLambda = false;
            bool isInvocation = identifierName.Parent is InvocationExpressionSyntax;
            bool isLocalMethodIdentifier = localMethods.ContainsKey(identifierName.ToString());
            if (statement != null)
            {
                containedInLambda = symbolInfo.Symbol.IsContainedInLambda(statement);
            }
            displayDiagnostic = !isMemberAccess && existsInContext && isBuilders && !containedInLambda;
            displayNode = identifierName;
            if (isLocalMethodIdentifier && isInvocation)
            {
                SyntaxNode localMethod = localMethods[identifierName.ToString()];
                ProcessContext methodProcessContext = new ProcessContext(level + 1, new Dictionary<string, VariableContext>(), new Dictionary<string, SyntaxNode>(), semanticModel);
                variables = ProcessNodes(analysisContexts, buildersToExpressionContext, localMethod, null, methodProcessContext);
                localMethods.Remove(identifierName.ToString());
                return variables;
            }
        }

        if (canEvaluate)
        {
            List<String> variablesValuesChanged = StoreValue(analysisContexts, buildersToExpressionContext, node, current, buildersNodes);
            return variablesValuesChanged;
        }

        if (displayDiagnostic)
        {
            DisplayDiagnostic(analysisContexts, current, displayNode);
            variables.Add(displayNode.ToString());
            return variables;
        }

        var childNodes = node.ChildNodes();
        FindLocalMethods(node, localMethods);

        foreach (var child in childNodes)
        {
            if (child is LocalFunctionStatementSyntax)
            {
                continue;
            }
            ProcessContext childProcessContext = new ProcessContext(level + 1, variableValues, localMethods, semanticModel);
            var list = ProcessNodes(analysisContexts, buildersToExpressionContext, child, statementNode, childProcessContext);
            variables.AddRange(list);
        }
        foreach (var localMethod in localMethods)
        {
            var localFunctionName = localMethod.Key;
            var functionSyntaxNode = localMethod.Value;
            if (!localMethods.ContainsKey(localFunctionName))
            {
                continue;
            }
            ProcessContext childProcessContext = new ProcessContext(level + 1, new Dictionary<string, VariableContext>(), new Dictionary<string, SyntaxNode>(), semanticModel);
            ProcessNodes(analysisContexts, buildersToExpressionContext, functionSyntaxNode, statementNode, childProcessContext);
        }


        foreach (var variable in variables)
        {
            if (variableValues.ContainsKey(variable))
            {
                variableValues.Remove(variable);
            }
        }

        return variables;
    }

    private static void ProcessTree(List<ExpressionAnalysisContext> analysisContexts,
                                    Dictionary<string, ExpressionAnalysisContext> buildersToExpressionContext,
                                    SyntaxNode node,
                                    ProcessContext current)
    {
        var variableValues = current.variableValues;
        var functions = current.functions;
        var semanticModel = current.semanticModel;
        var level = current.level;
        if (node is MethodDeclarationSyntax)
        {
            ProcessContext methodProcessContext = new ProcessContext(level + 1, variableValues, functions, semanticModel);
            ProcessNodes(analysisContexts, buildersToExpressionContext, node, null, methodProcessContext);
            return;
        }

        var childNodes = node.ChildNodes();
        foreach (var childNode in childNodes)
        {
            ProcessTree(analysisContexts, buildersToExpressionContext, childNode, current);
        }
    }

    public static void ResolveVariables(List<ExpressionAnalysisContext> analysisContexts,
                                        Dictionary<string, ExpressionAnalysisContext> builderToAnalysisContextMap,
                                        SemanticModel semanticModel)
    {
        var syntaxTree = semanticModel.SyntaxTree;
        var root = syntaxTree.GetRoot();
        ProcessContext processContext = new ProcessContext(0, new Dictionary<string, VariableContext>(),
                                                            new Dictionary<string, SyntaxNode>(), semanticModel);
        ProcessTree(analysisContexts, builderToAnalysisContextMap, root, processContext);
    }
}
