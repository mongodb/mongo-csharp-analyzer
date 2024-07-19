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

using static MongoDB.Analyzer.Core.ExpressionProcessor;

namespace MongoDB.Analyzer.Core.Builders;

internal static class BuilderExpressionProcessor
{
    private enum NodeType
    {
        Unknown = 0,
        Invalid,
        Builders,
        Fluent
    }

    public static ExpressionsAnalysis ProcessSemanticModel(MongoAnalysisContext context)
    {
        var semanticModel = context.SemanticModelAnalysisContext.SemanticModel;
        var syntaxTree = semanticModel.SyntaxTree;
        var root = syntaxTree.GetRoot();

        var analysisContexts = new List<ExpressionAnalysisContext>();
        var invalidExpressionNodes = new List<InvalidExpressionAnalysisNode>();

        var typesProcessor = context.TypesProcessor;
        var nodesProcessed = new HashSet<SyntaxNode>();
        var buildersToAnalysisContextMap = context.Settings.EnableVariableTracking ? new Dictionary<SyntaxNode, ExpressionAnalysisContext>() : null;

        foreach (var node in root.DescendantNodesWithSkipList(nodesProcessed))
        {
            IEnumerable<SyntaxNode> nodesToRewrite = null;
            var (nodeType, namedType, expressionNode) = GetNodeType(semanticModel, node);

            switch (nodeType)
            {
                case NodeType.Builders:
                    {
                        nodesToRewrite = GetBuildersDefinitionNodes(semanticModel, expressionNode);
                        break;
                    }
                case NodeType.Fluent:
                    {
                        var collectionNode = expressionNode
                            .NestedInvocations()
                            .FirstOrDefault(n => semanticModel.GetTypeInfo(n).Type.IsSupportedIMongoCollection());

                        if (collectionNode == null)
                        {
                            continue;
                        }

                        nodesToRewrite = new SyntaxNode[] {collectionNode};

                        break;
                    }
                case NodeType.Invalid:
                    {
                        nodesProcessed.Add(node);
                        continue;
                    }
                default:
                    {
                        continue;
                    }
            }

            nodesProcessed.Add(node);

            try
            {
                if (!ProcessTypeArguments(namedType.TypeArguments, typesProcessor))
                {
                    continue;
                }

                var rewriteContext = RewriteContext.Builders(expressionNode, nodesToRewrite, semanticModel, typesProcessor);
                var (newBuildersExpression, constantsMapper) = RewriteExpression(rewriteContext);

                if (newBuildersExpression != null)
                {
                    var expressionContext = new ExpressionAnalysisContext(new(
                        expressionNode,
                        typesProcessor.GetTypeSymbolToGeneratedTypeMapping(namedType.TypeArguments.First()),
                        newBuildersExpression,
                        constantsMapper,
                        expressionNode.GetLocation()));

                    analysisContexts.Add(expressionContext);
                    buildersToAnalysisContextMap?.Add(expressionNode, expressionContext);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed analyzing {node.NormalizeWhitespace()} with {ex.Message}");
            }
        }

        if (context.Settings.EnableVariableTracking)
        {
            try
            {
                analysisContexts = BuildersVariablesResolver.ResolveVariables(analysisContexts, buildersToAnalysisContextMap, semanticModel);
            }
            catch (Exception ex)
            {
                context.Logger.Log($"Failed resolving variables with {ex.Message}.");
            }
        }

        var builderAnalysis = new ExpressionsAnalysis()
        {
            AnalysisNodeContexts = analysisContexts.ToArray(),
            InvalidExpressionNodes = invalidExpressionNodes.ToArray(),
            TypesDeclarations = typesProcessor.TypesDeclarations
        };

        context.Logger.Log($"Builders: Found {builderAnalysis.AnalysisNodeContexts.Length} expressions.");

        return builderAnalysis;
    }

    private static IEnumerable<SyntaxNode> GetBuildersDefinitionNodes(SemanticModel semanticModel, SyntaxNode expressionNode)
    {
        var nodesProcessed = new HashSet<SyntaxNode>();
        var builderDefinitionNodes = new List<SyntaxNode>();

        foreach (var node in expressionNode.DescendantNodesWithSkipList(nodesProcessed))
        {
            if (semanticModel.GetTypeInfo(node).Type.IsBuilder())
            {
                nodesProcessed.Add(node);

                var isBuildersContainer = false;
                var containsAlias = false;

                // Check if Member Expression is Builders Container
                // Example: Builders<T>
                if (node is MemberAccessExpressionSyntax memberAccessExpressionSyntax &&
                    semanticModel.GetSymbolInfo(memberAccessExpressionSyntax.Expression).Symbol is INamedTypeSymbol namedTypeSymbol &&
                    namedTypeSymbol.IsBuildersContainer())
                {
                    isBuildersContainer = true;
                }

                // Check if Node contains alias
                // Example: driverAlias.Builders<T>, mongoDBNamespaceAlias.Driver.Builders<T>
                var syntaxNode = (node as MemberAccessExpressionSyntax)?.Expression;
                while (syntaxNode != null)
                {
                    if (semanticModel.GetAliasInfo(syntaxNode) != null)
                    {
                        containsAlias = true;
                        break;
                    }

                    syntaxNode = (syntaxNode as MemberAccessExpressionSyntax)?.Expression;
                }

                // Skip Nodes that are Builder Containers and that don't contain alias
                // Examples: MongoDB.Driver.Builders
                if (isBuildersContainer && !containsAlias)
                {
                    continue;
                }

                builderDefinitionNodes.Add(node);
            }
        }

        return builderDefinitionNodes;
    }

    private static (NodeType NodeType, INamedTypeSymbol NamedSymbol, SyntaxNode ExpressionNode) GetNodeType(SemanticModel semanticModel, SyntaxNode node)
    {
        var expressionNode = node;
        var nodeType = NodeType.Unknown;

        if (expressionNode is AssignmentExpressionSyntax assignmentExpressionSyntax)
        {
            expressionNode = assignmentExpressionSyntax.Right;
        }

        expressionNode = expressionNode.TrimParenthesis();

        if (expressionNode is not InvocationExpressionSyntax &&
            expressionNode is not BinaryExpressionSyntax)
        {
            return default;
        }

        if (semanticModel.GetTypeInfo(expressionNode).Type is not INamedTypeSymbol namedType ||
            namedType.TypeArguments.Length == 0)
        {
            return default;
        }

        if (namedType.IsFindFluent())
        {
            nodeType = NodeType.Fluent;
        }
        else if (namedType.IsBuilderDefinition())
        {
            nodeType = NodeType.Builders;

            if (expressionNode is BinaryExpressionSyntax binaryExpressionSyntax)
            {
                var childNodeType = GetNodeType(semanticModel, binaryExpressionSyntax.Left);

                if (childNodeType.NodeType == NodeType.Builders)
                {
                    childNodeType = GetNodeType(semanticModel, binaryExpressionSyntax.Right);

                    if (childNodeType.NodeType == NodeType.Builders)
                    {
                        return (nodeType, namedType, expressionNode);
                    }
                }

                return default;
            }

            foreach (var invocationNode in expressionNode.DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>())
            {
                if (semanticModel.GetSymbolInfo(invocationNode).Symbol is not IMethodSymbol methodSymbol ||
                    methodSymbol.ReturnType.IsBuilderDefinition() && !methodSymbol.IsBuilderMethod())
                {
                    return default;
                }
            }
        }
        else
        {
            return default;
        }

        if (namedType.TypeArguments.Any(t => !t.IsSupportedBuilderType()))
        {
            return (NodeType.Invalid, default, default);
        }

        return (nodeType, namedType, expressionNode);
    }

    private static bool ProcessTypeArguments(IEnumerable<ITypeSymbol> typeArguments, TypesProcessor typesProcessor)
    {
        foreach (var typeArgument in typeArguments)
        {
            var remappedName = typesProcessor.ProcessTypeSymbol(typeArgument);
            if (remappedName == null)
            {
                return false;
            }
        }

        return true;
    }
}
