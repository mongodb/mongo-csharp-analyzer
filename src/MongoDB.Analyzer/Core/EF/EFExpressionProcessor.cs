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

namespace MongoDB.Analyzer.Core.EF;

internal static class EFExpressionProcessor
{
    public static ExpressionsAnalysis ProcessSemanticModel(MongoAnalysisContext context)
    {
        var semanticModel = context.SemanticModelAnalysisContext.SemanticModel; //Semantic Model
        var syntaxTree = semanticModel.SyntaxTree; //Syntax Tree
        var root = syntaxTree.GetRoot(); //Root of Syntax Tree

        var processedSyntaxNodes = new HashSet<SyntaxNode>(); //Store Processed Syntax Nodes
        var analysisContexts = new List<ExpressionAnalysisContext>(); //Store Analysis Contexts
        var invalidExpressionNodes = new List<InvalidExpressionAnalysisNode>(); //Store Invalid Expression Analysis Nodes
        var typesProcessor = context.TypesProcessor; //Store the Types Processor

        //Preorder Document Traversal
        foreach (var node in root.DescendantNodesWithSkipList<ExpressionSyntax>(processedSyntaxNodes))
        {
            var deepestEFQueryableNode = node;

            //EF-Based Queries are Assumed to be Using Invocation Expressions Only
            if (node is InvocationExpressionSyntax invocationNode)
            {
                var methodSymbol = invocationNode.GetMethodSymbol(semanticModel); //Get Method Symbol

                //The Method must be a valid LINQ Method and receive/return IQueryable
                if (!methodSymbol.IsDefinedInMongoLinqOrSystemLinq() ||
                    !methodSymbol.ReceiverType.IsIQueryable() ||
                    !methodSymbol.ReturnType.IsIQueryable())
                {
                    continue;
                }

                //Find Deepest MongoQueryable Node
                deepestEFQueryableNode = invocationNode
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

            //Add node to the processed nodes hashset
            processedSyntaxNodes.Add(node);

            // Validate IMongoQueryable node candidate
            if (deepestEFQueryableNode == null)
            {
                continue;
            }

            //Get TypeInfo from the Mongo Queryable Node
            var EFQueryableTypeInfo = semanticModel.GetTypeInfo(deepestEFQueryableNode);

            //Determine if the QueryableType is EF
            if (!EFQueryableTypeInfo.Type.IsEF() ||
                EFQueryableTypeInfo.Type is not INamedTypeSymbol EFQueryableNamedType ||
                EFQueryableNamedType.TypeArguments.Length != 1 ||
                !EFQueryableNamedType.TypeArguments[0].IsSupportedMongoCollectionType())
            {
                continue;
            }

            try
            {
                if (PreanalyzeEFExpression(node, semanticModel, invalidExpressionNodes))
                {
                    var generatedMongoQueryableTypeName = typesProcessor.ProcessTypeSymbol(EFQueryableNamedType.TypeArguments[0]);

                    var rewriteContext = RewriteContext.EF(node, deepestEFQueryableNode, semanticModel, typesProcessor);
                    var (newEFExpression, constantsMapper) = RewriteExpression(rewriteContext);

                    if (newEFExpression != null)
                    {
                        var EFContext = new ExpressionAnalysisContext(new(
                            node,
                            generatedMongoQueryableTypeName,
                            newEFExpression,
                            constantsMapper,
                            node.GetLocation()));

                        analysisContexts.Add(EFContext);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed analyzing {node.NormalizeWhitespace()} with {ex.Message}");
            }
        }

        var EFAnalysis = new ExpressionsAnalysis()
        {
            AnalysisNodeContexts = analysisContexts.ToArray(),
            InvalidExpressionNodes = invalidExpressionNodes.ToArray(),
            TypesDeclarations = typesProcessor.TypesDeclarations
        };

        context.Logger.Log($"EF: Found {EFAnalysis.AnalysisNodeContexts.Length} expressions");

        return EFAnalysis;
    }

    private static bool PreanalyzeEFExpression(SyntaxNode EFExpressionNode, SemanticModel semanticModel, List<InvalidExpressionAnalysisNode> invalidEFExpressionNodes)
    {
        var result = true;

        foreach (var expression in EFExpressionNode.DescendantNodes().Where(node => node is QueryClauseSyntax || node is SimpleLambdaExpressionSyntax))
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

                        if (argSymbol.IsContainedInLambdaOrQueryParameter(EFExpressionNode))
                        {
                            invalidEFExpressionNodes.Add(new(
                                methodInvocation,
                                EFAnalysisErrorMessages.MethodInvocationNotSupported));

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

