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

internal static class BuildersVariablesResolver
{
    private sealed record VariableValue(
        ExpressionAnalysisContext ExpressionAnalysisContext,
        List<Location> Locations);

    private sealed record ResolutionContext(
        SemanticModel SemanticModel,
        List<VariableValue> ProcessedVariables,
        Dictionary<string, VariableValue> CurrentVariablesValues,
        HashSet<SyntaxNode> BuilderNodesProcessed,
        Dictionary<SyntaxNode, ExpressionAnalysisContext> BuilderToAnalysisContext);

    private sealed record EvaluationResult(
        bool CanEvaluate,
        Dictionary<SyntaxNode, VariableValue> SyntaxNodeToVariableValueMap);

    public static List<ExpressionAnalysisContext> ResolveVariables(
        List<ExpressionAnalysisContext> expressionAnalysisContexts,
        Dictionary<SyntaxNode, ExpressionAnalysisContext> builderToAnalysisContextMap,
        SemanticModel semanticModel)
    {
        var variableValuesList = new List<VariableValue>();
        var builderNodesProcessed = new HashSet<SyntaxNode>();
        var resolutionContext = new ResolutionContext(semanticModel, variableValuesList, new Dictionary<string, VariableValue>(), builderNodesProcessed, builderToAnalysisContextMap);

        ProcessTree(semanticModel.SyntaxTree.GetRoot(), resolutionContext);

        if (variableValuesList.Any())
        {
            expressionAnalysisContexts = new List<ExpressionAnalysisContext>();
            var newAnalysisContexts = variableValuesList.Select(v => v.ExpressionAnalysisContext with { Node = v.ExpressionAnalysisContext.Node with { Locations = v.Locations.ToArray() } });
            expressionAnalysisContexts.AddRange(newAnalysisContexts);

            var previousAnalysisContexts = builderToAnalysisContextMap.Where(p => !builderNodesProcessed.ContainsSafe(p.Key)).Select(p => p.Value);
            expressionAnalysisContexts.AddRange(previousAnalysisContexts);
        }

        return expressionAnalysisContexts;
    }

    private static bool IsCompoundExpression(SyntaxNode syntaxNode) =>
        syntaxNode.Kind() switch
        {
            SyntaxKind.AndAssignmentExpression or
            SyntaxKind.OrAssignmentExpression => true,
            _ => false
        };

    private static SyntaxKind GenerateBinaryExpressionSyntaxKind(SyntaxNode syntaxNode) =>
        syntaxNode.Kind() switch
        {
            SyntaxKind.AndAssignmentExpression => SyntaxKind.BitwiseAndExpression,
            SyntaxKind.OrAssignmentExpression => SyntaxKind.BitwiseOrExpression,
            _ => SyntaxKind.None
        };

    private static EvaluationResult PreEvaluate(
        SyntaxNode syntaxNode,
        ResolutionContext resolutionContext,
        Dictionary<SyntaxNode, VariableValue> syntaxNodeToVariableValueMap,
        bool displayDiagnostics)
    {
        var builderToAnalysisContext = resolutionContext.BuilderToAnalysisContext;
        var variableValues = resolutionContext.CurrentVariablesValues;
        var canEvaluate = false;
        var rightHandSide = syntaxNode switch
        {
            ParenthesizedExpressionSyntax @parenthesizedExpression => @parenthesizedExpression.TrimParenthesis(),
            AssignmentExpressionSyntax  @assignmentExpression => @assignmentExpression.Right,
            VariableDeclaratorSyntax @variableDeclarator => @variableDeclarator.Initializer.Value,
            _ => syntaxNode
        };

        VariableValue evaluatedVariableExpression = null;

        if (rightHandSide is IdentifierNameSyntax identifierNameSyntax)
        {
            canEvaluate = variableValues.TryGetValueSafe(identifierNameSyntax.Identifier.ValueText, out evaluatedVariableExpression);
        }
        else if (rightHandSide is InvocationExpressionSyntax invocationExpression)
        {
            if (builderToAnalysisContext.TryGetValueSafe(invocationExpression, out var expressionAnalysisContext))
            {
                canEvaluate = true;
                evaluatedVariableExpression = new VariableValue(expressionAnalysisContext, expressionAnalysisContext.Node.Locations.ToList());
            }
        }
        else if (rightHandSide is BinaryExpressionSyntax binaryExpressionSyntax)
        {
            if (builderToAnalysisContext.TryGetValueSafe(binaryExpressionSyntax, out var expressionAnalysisContext))
            {
                evaluatedVariableExpression = new VariableValue(expressionAnalysisContext, expressionAnalysisContext.Node.Locations.ToList());
            }
            else
            {
                var leftEvaluationResult = PreEvaluate(binaryExpressionSyntax.Left, resolutionContext, syntaxNodeToVariableValueMap: null, displayDiagnostics);

                if (leftEvaluationResult.CanEvaluate)
                {
                    var rightEvaluationResult = PreEvaluate(binaryExpressionSyntax.Right, resolutionContext, syntaxNodeToVariableValueMap: leftEvaluationResult.SyntaxNodeToVariableValueMap, displayDiagnostics);
                    canEvaluate = leftEvaluationResult.CanEvaluate && rightEvaluationResult.CanEvaluate;
                    syntaxNodeToVariableValueMap = rightEvaluationResult.SyntaxNodeToVariableValueMap;
                }
            }
        }

        if (canEvaluate && IsCompoundExpression(syntaxNode))
        {
            var assignmentExpression = syntaxNode as AssignmentExpressionSyntax;
            var leftHandSideIdentifier = assignmentExpression.Left.TrimParenthesis() as IdentifierNameSyntax;
            canEvaluate = variableValues.TryGetValueSafe(leftHandSideIdentifier.Identifier.ValueText, out var leftVariableValue);

            if (leftVariableValue != null)
            {
                syntaxNodeToVariableValueMap ??= new Dictionary<SyntaxNode, VariableValue>();
                syntaxNodeToVariableValueMap.Add(leftHandSideIdentifier, leftVariableValue);
            }
        }

        if (canEvaluate && evaluatedVariableExpression != null)
        {
            syntaxNodeToVariableValueMap ??= new Dictionary<SyntaxNode, VariableValue>();
            syntaxNodeToVariableValueMap.Add(rightHandSide, evaluatedVariableExpression);

            if (rightHandSide is IdentifierNameSyntax && displayDiagnostics)
            {
                evaluatedVariableExpression.Locations.Add(rightHandSide.GetLocation());
            }
        }

        return new EvaluationResult(canEvaluate, syntaxNodeToVariableValueMap);
    }

    private static ExpressionAnalysisContext GenerateNewExpressionAnalysisContext(
        SyntaxNode syntaxNode,
        Dictionary<SyntaxNode, VariableValue> builderNodeToVariableValue)
    {
        var argumentTypeName = builderNodeToVariableValue.Values.First().ExpressionAnalysisContext.Node.ArgumentTypeName;
        var rewrittenSyntaxNode = syntaxNode.ReplaceNodes(builderNodeToVariableValue.Keys, (n, _) => builderNodeToVariableValue[n].ExpressionAnalysisContext.Node.RewrittenExpression);

        if (IsCompoundExpression(rewrittenSyntaxNode))
        {
            var rewrittenAssignmentExpression = rewrittenSyntaxNode as AssignmentExpressionSyntax;
            var binaryExpressionSyntaxKind = GenerateBinaryExpressionSyntaxKind(syntaxNode);
            rewrittenSyntaxNode = SyntaxFactory.BinaryExpression(binaryExpressionSyntaxKind, rewrittenAssignmentExpression.Left, rewrittenAssignmentExpression.Right);
        }

        var rewrittenBuildersExpression = rewrittenSyntaxNode switch
        {
            AssignmentExpressionSyntax @assignmentExpression => @assignmentExpression.Right,
            VariableDeclaratorSyntax @variableDeclarator => @variableDeclarator.Initializer.Value,
            _ => rewrittenSyntaxNode
        };

        var (finalBuildersExpression, finalConstantMapper) = ConstantsMapperComposer.Compose(rewrittenBuildersExpression, builderNodeToVariableValue.Values.Select(v => v.ExpressionAnalysisContext.Node.ConstantsRemapper));
        return new ExpressionAnalysisContext(new ExpressionAnalysisNode(rewrittenBuildersExpression, argumentTypeName, finalBuildersExpression, finalConstantMapper));
    }

    private static VariableValue GetOrGenerateVariableValue(SyntaxNode syntaxNode, EvaluationResult evaluationResult)
    {
        VariableValue variableValue;

        if (evaluationResult.SyntaxNodeToVariableValueMap.Count > 1)
        {
            var expressionAnalysisContext = GenerateNewExpressionAnalysisContext(syntaxNode, evaluationResult.SyntaxNodeToVariableValueMap);
            variableValue = new VariableValue(expressionAnalysisContext, new List<Location>());
        }
        else
        {
            variableValue = evaluationResult.SyntaxNodeToVariableValueMap.Values.First();
        }

        return variableValue;
    }

    private static void AddOrRemoveVariableValueFromContext(SyntaxNode syntaxNode, VariableValue variableValue, ResolutionContext resolutionContext, HashSet<string> dirtyVariables)
    {
        var variableName = syntaxNode switch
        {
            VariableDeclaratorSyntax @declarator => @declarator.Identifier.ValueText,
            //For Assignments, LHS must be Identifier since this check was already completed in TryProcessBuildersExpression
            AssignmentExpressionSyntax @assignment => (@assignment.Left.TrimParenthesis() as IdentifierNameSyntax).Identifier.ValueText,
            _ => null
        };

        dirtyVariables.Add(variableName);
        resolutionContext.CurrentVariablesValues[variableName] = variableValue;

        var originalExpression = variableValue.ExpressionAnalysisContext.Node.OriginalExpression;

        if (resolutionContext.BuilderNodesProcessed.Add(originalExpression))
        {
            resolutionContext.ProcessedVariables.Add(variableValue);
        }

        if (syntaxNode.Parent is ConditionalExpressionSyntax)
        {
            resolutionContext.CurrentVariablesValues.Remove(variableName);
        }
    }

    private static void ProcessDeclarationOrAssignment(SyntaxNode syntaxNode, ResolutionContext resolutionContext, HashSet<string> dirtyVariables, bool displayDiagnostics)
    {
        var evaluationResult = PreEvaluate(syntaxNode, resolutionContext, null, displayDiagnostics);

        if (evaluationResult.CanEvaluate)
        {
            var variableValue = GetOrGenerateVariableValue(syntaxNode, evaluationResult);
            AddOrRemoveVariableValueFromContext(syntaxNode, variableValue, resolutionContext, dirtyVariables);
        }
        else
        {
            var variableName = syntaxNode switch
            {
                VariableDeclaratorSyntax @declarator => @declarator.Identifier.ValueText,
                //For Assignments, LHS must be Identifier since this check was already completed in TryProcessBuildersExpression
                AssignmentExpressionSyntax @assignment => (@assignment.Left.TrimParenthesis() as IdentifierNameSyntax).Identifier.ValueText,
                _ => null
            };

            resolutionContext.CurrentVariablesValues.Remove(variableName);
        }
    }

    private static bool TryProcessBuildersExpression(SyntaxNode syntaxNode, ResolutionContext resolutionContext, HashSet<string> dirtyVariables)
    {
        var semanticModel = resolutionContext.SemanticModel;

        if (!IsValidBuilderExpression(syntaxNode))
        {
            if (syntaxNode is VariableDeclaratorSyntax declarator)
            {
                resolutionContext.CurrentVariablesValues.Remove(declarator.Identifier.ValueText);
            }
            else if (syntaxNode is AssignmentExpressionSyntax assignment)
            {
                resolutionContext.CurrentVariablesValues.RemoveRange(ScrapeVariables(assignment));
            }

            return false;
        }

        if (syntaxNode is VariableDeclaratorSyntax variableDeclarator)
        {
            var displayDiagnostics = true;
            if (variableDeclarator.Initializer.Value is AssignmentExpressionSyntax rightHandAssignment)
            {
                if (!TryProcessBuildersExpression(rightHandAssignment, resolutionContext, dirtyVariables))
                {
                    resolutionContext.CurrentVariablesValues.Remove(variableDeclarator.Identifier.ValueText);
                    return false;
                }

                variableDeclarator = SyntaxFactory.VariableDeclarator(variableDeclarator.Identifier,
                    variableDeclarator.ArgumentList,
                    SyntaxFactory.EqualsValueClause(SyntaxFactory.Token(SyntaxKind.EqualsToken),
                    rightHandAssignment.Left));

                displayDiagnostics = false;
            }

            ProcessDeclarationOrAssignment(variableDeclarator, resolutionContext, dirtyVariables, displayDiagnostics);
        }
        else if (syntaxNode is AssignmentExpressionSyntax assignmentExpression)
        {
            var displayDiagnostics = true;
            if (assignmentExpression.Right is AssignmentExpressionSyntax rightHandAssignment)
            {
                if (!TryProcessBuildersExpression(rightHandAssignment, resolutionContext, dirtyVariables))
                {
                    resolutionContext.CurrentVariablesValues.RemoveRange(ScrapeVariables(assignmentExpression));
                    return false;
                }

                assignmentExpression = SyntaxFactory.AssignmentExpression(assignmentExpression.Kind(), assignmentExpression.Left, rightHandAssignment.Left);
                displayDiagnostics = false;
            }

            ProcessDeclarationOrAssignment(assignmentExpression, resolutionContext, dirtyVariables, displayDiagnostics);
        }
        
        return true;

        bool ContainsConditionalExpression(SyntaxNode syntaxNode)
        {
            syntaxNode = syntaxNode switch
            {
                VariableDeclaratorSyntax @variableDeclarator => @variableDeclarator.Initializer.Value,
                AssignmentExpressionSyntax @assignmentExpression => @assignmentExpression.Right,
                _ => syntaxNode
            };

            syntaxNode = syntaxNode.TrimParenthesis();
            return syntaxNode is SwitchExpressionSyntax || syntaxNode is ConditionalExpressionSyntax;
        }

        bool IsValidBuilderExpression(SyntaxNode syntaxNode)
        {
            var isValidBuilderExpression = false;

            if (syntaxNode is VariableDeclaratorSyntax declarator)
            {
                isValidBuilderExpression = (semanticModel.GetSymbolInfo((declarator.Parent as VariableDeclarationSyntax).Type).Symbol as ITypeSymbol).IsBuilderDefinition() && !ContainsConditionalExpression(syntaxNode);
            }
            else if (syntaxNode is AssignmentExpressionSyntax assignment)
            {
                var leftHandSide = assignment.Left.TrimParenthesis();
                isValidBuilderExpression = (leftHandSide is TupleExpressionSyntax tuple ?
                    tuple.Arguments.Any(u => semanticModel.GetTypeInfo(u.TrimParenthesis()).Type.IsBuilderDefinition()) :
                    semanticModel.GetTypeInfo(leftHandSide).Type.IsBuilderDefinition()) && !ContainsConditionalExpression(syntaxNode) && (assignment.Right is AssignmentExpressionSyntax || leftHandSide is IdentifierNameSyntax);
            }

            return isValidBuilderExpression;
        }
        
        bool IsPartOfAccessExpression(IdentifierNameSyntax identifierName) => identifierName.Parent is MemberAccessExpressionSyntax || identifierName.Parent is ElementAccessExpressionSyntax;

        IEnumerable<string> ScrapeVariables(AssignmentExpressionSyntax assignment) => assignment.Left.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>().Where(n => !IsPartOfAccessExpression(n)).Select(n => n.Identifier.ValueText);
    }

    private static bool IsConditionalBlock(SyntaxNode syntaxNode) =>
        syntaxNode switch
        {
            LocalFunctionStatementSyntax => false,
            SwitchSectionSyntax or
            SwitchExpressionArmSyntax or
            ElseClauseSyntax or
            ForStatementSyntax or
            CommonForEachStatementSyntax or
            DoStatementSyntax or
            WhileStatementSyntax or
            CatchClauseSyntax => true,
            _ => syntaxNode.Parent is TryStatementSyntax || syntaxNode.Parent is IfStatementSyntax
        };

    private static void ProcessBlock(SyntaxNode syntaxNode, ResolutionContext resolutionContext)
    {
        var dirtyVariables = new HashSet<string>();
        HashSet<SyntaxNode> nodesProcessed = null;

        foreach (var childNode in syntaxNode.DescendantNodes(n => ShouldDescendIntoChildren(n)))
        {
            if (nodesProcessed.ContainsSafe(childNode.Parent))
            {
                continue;
            }

            if (IsConditionalBlock(childNode))
            {
                ProcessBlock(childNode, resolutionContext);
                MarkNodeAsProcessed(childNode);
            }
            else if (TryProcessBuildersExpression(childNode, resolutionContext, dirtyVariables))
            {
                MarkNodeAsProcessed(childNode);
            }
        }

        resolutionContext.CurrentVariablesValues.RemoveRange(dirtyVariables);

        bool ShouldDescendIntoChildren(SyntaxNode syntaxNode) =>
            !nodesProcessed.ContainsSafe(syntaxNode.Parent) && syntaxNode is not LocalFunctionStatementSyntax;
        
        void MarkNodeAsProcessed(SyntaxNode syntaxNode)
        {
            nodesProcessed ??= new HashSet<SyntaxNode>();
            nodesProcessed.Add(syntaxNode);
        }
    }

    private static void ProcessTree(SyntaxNode node, ResolutionContext resolutionContext)
    {
        var childNodes = node.DescendantNodesAndSelf().OfType<MethodDeclarationSyntax>();

        foreach (var childNode in childNodes)
        {
            ProcessBlock(childNode, resolutionContext);
        }
    }
}

