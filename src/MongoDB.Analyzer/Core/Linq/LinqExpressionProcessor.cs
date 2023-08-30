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

namespace MongoDB.Analyzer.Core.Linq;

internal static class LinqExpressionProcessor
{
    public static ExpressionsAnalysis ProcessSemanticModel(MongoAnalysisContext context)
    {
        var semanticModel = context.SemanticModelAnalysisContext.SemanticModel;
        var syntaxTree = semanticModel.SyntaxTree;
        var root = syntaxTree.GetRoot();

        var processedSyntaxNodes = new HashSet<SyntaxNode>();
        var analysisContexts = new List<ExpressionAnalysisContext>();
        var invalidExpressionNodes = new List<InvalidExpressionAnalysisNode>();
        var typesProcessor = context.TypesProcessor;

        foreach (var node in root.DescendantNodesWithSkipList<ExpressionSyntax>(processedSyntaxNodes))
        {
            var deepestMongoQueryableNode = node;

            if (node is QueryExpressionSyntax queryNode)
            {
                var expression = queryNode.FromClause.Expression;
                var queryMethodSymbol = semanticModel.GetTypeInfo(expression).Type;
                if (!queryMethodSymbol.IsIMongoQueryable())
                {
                    continue;
                }

                deepestMongoQueryableNode = queryNode.FromClause.Expression;
            }
            else if (node is InvocationExpressionSyntax invocationNode)
            {
                var methodSymbol = invocationNode.GetMethodSymbol(semanticModel);

                if (!methodSymbol.IsDefinedInMongoLinqOrSystemLinq() ||
                    !methodSymbol.ReceiverType.IsIQueryable() ||
                    !methodSymbol.ReturnType.IsIQueryable())
                {
                    continue;
                }

                deepestMongoQueryableNode = invocationNode
                    .NestedInvocations()
                    .FirstOrDefault(n =>
                        {
                            var currentMethodSymbol = n.GetMethodSymbol(semanticModel);

                            // Find the first method that is not receiving IQueryable or is not defined in System.Linq or MongoDB.Driver.Linq
                            return !((currentMethodSymbol?.ReceiverType).IsIQueryable() && currentMethodSymbol.IsDefinedInMongoLinqOrSystemLinq());
                        });
            }
            else
            {
                continue;
            }

            processedSyntaxNodes.Add(node);

            // Validate IMongoQueryable node candidate
            if (deepestMongoQueryableNode == null)
            {
                continue;
            }

            var mongoQueryableTypeInfo = semanticModel.GetTypeInfo(deepestMongoQueryableNode);
            if (!mongoQueryableTypeInfo.Type.IsIMongoQueryable() ||
                mongoQueryableTypeInfo.Type is not INamedTypeSymbol mongoQueryableNamedType ||
                mongoQueryableNamedType.TypeArguments.Length != 1 ||
                !mongoQueryableNamedType.TypeArguments[0].IsSupportedMongoCollectionType())
            {
                continue;
            }

            try
            {
                if (PreanalyzeLinqExpression(node, semanticModel, invalidExpressionNodes))
                {
                    var generatedMongoQueryableTypeName = typesProcessor.ProcessTypeSymbol(mongoQueryableNamedType.TypeArguments[0]);

                    var rewriteContext = RewriteContext.Linq(node, deepestMongoQueryableNode, semanticModel, typesProcessor);
                    var (newLinqExpression, constantsMapper) = RewriteExpression(rewriteContext);

                    if (newLinqExpression != null)
                    {
                        var linqContext = new ExpressionAnalysisContext(new(
                            node,
                            generatedMongoQueryableTypeName,
                            newLinqExpression,
                            constantsMapper,
                            node.GetLocation()));

                        analysisContexts.Add(linqContext);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed analyzing {node.NormalizeWhitespace()} with {ex.Message}");
            }
        }

        var linqAnalysis = new ExpressionsAnalysis()
        {
            AnalysisNodeContexts = analysisContexts.ToArray(),
            InvalidExpressionNodes = invalidExpressionNodes.ToArray(),
            TypesDeclarations = typesProcessor.TypesDeclarations
        };

        context.Logger.Log($"LINQ: Found {linqAnalysis.AnalysisNodeContexts.Length} expressions");

        return linqAnalysis;
    }

    private static bool PreanalyzeLinqExpression(SyntaxNode linqExpressionNode, SemanticModel semanticModel, List<InvalidExpressionAnalysisNode> invalidLinqExpressionNodes)
    {
        var result = true;

        foreach (var expression in linqExpressionNode.DescendantNodes().Where(node => node is QueryClauseSyntax || node is SimpleLambdaExpressionSyntax))
        {
            foreach (var methodInvocation in expression.DescendantNodes().OfType<InvocationExpressionSyntax>())
            {
                var symbolInfo = semanticModel.GetSymbolInfo(methodInvocation);

                // find methods referencing lambda parameter
                if (symbolInfo.Symbol is IMethodSymbol methodSymbol)
                {
                    foreach (var arg in methodInvocation.ArgumentList.Arguments)
                    {
                        var underlyingNode = SyntaxFactoryUtilities.GetUnderlyingNameSyntax(arg.Expression);
                        if (underlyingNode == null)
                        {
                            continue;
                        }

                        var argSymbol = semanticModel.GetSymbolInfo(underlyingNode).Symbol;

                        if (argSymbol.IsContainedInLambdaOrQueryParameter(linqExpressionNode))
                        {
                            invalidLinqExpressionNodes.Add(new(
                                methodInvocation,
                                LinqAnalysisErrorMessages.MethodInvocationNotSupported));

                            result = false;
                            break;
                        }
                    }
                }
            }
        }

        return result;
    }
}
