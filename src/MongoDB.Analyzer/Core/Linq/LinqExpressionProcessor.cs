using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MongoDB.Analyzer.Core.Linq
{
    internal static class LinqExpressionProcessor
    {
        public static ExpressionsAnalysis ProcessSemanticModel(SemanticModelAnalysisContext context)
        {
            var semanticModel = context.SemanticModel;
            var syntaxTree = semanticModel.SyntaxTree;
            var root = syntaxTree.GetRoot();

            var processedSyntaxNodes = new HashSet<SyntaxNode>();
            var analysisContexts = new List<ExpressionAnalysisContext>();
            var invalidExpressionNodes = new List<InvalidExpressionAnalysisNode>();

            var typesProcessor = new TypesProcessor();

            Func<SyntaxNode, bool> descendToChildrenPredicate = n =>
            {
                return !processedSyntaxNodes.Contains(n.Parent);
            };

            foreach (var node in root.DescendantNodes(descendToChildrenPredicate).OfType<InvocationExpressionSyntax>())
            {
                var methodSymbol = semanticModel.GetSymbolInfo(node).Symbol as IMethodSymbol;

                // Filter out non IMongoQueryable source or target invocations or 
                if (methodSymbol == null ||
                    !methodSymbol.ReceiverType.IsIMongoQueryable() ||
                    !methodSymbol.ReturnType.IsIMongoQueryable())
                {
                    continue;
                }

                processedSyntaxNodes.Add(node);

                // Find the deepest node that supplies IMongoQueryable symbol type
                var deepestMongoQueryableNode = GetNextNestedInvocation(node);

                while (deepestMongoQueryableNode != null)
                {
                    var currentMethodSymbol = semanticModel.GetSymbolInfo(deepestMongoQueryableNode).Symbol as IMethodSymbol;
                    
                    if (currentMethodSymbol?.ReducedFrom?.ReceiverType.IsMongoQueryable() != true)
                        break;

                    deepestMongoQueryableNode = GetNextNestedInvocation(deepestMongoQueryableNode);
                }

                // Validate IMongoQueryable node candidate
                if (deepestMongoQueryableNode == null)
                    continue;

                var mongoQueryableTypeInfo = semanticModel.GetTypeInfo(deepestMongoQueryableNode);
                if (!mongoQueryableTypeInfo.Type.IsIMongoQueryable() ||
                    mongoQueryableTypeInfo.Type is not INamedTypeSymbol mongoQueryableNamedType ||
                    mongoQueryableNamedType.TypeArguments.Length != 1)
                {
                    continue;
                }

                if (PreanalyzeLinqExpression(node, semanticModel, invalidExpressionNodes))
                {
                    var generatedMongoQueryableTypeName = typesProcessor.ProcessTypeSymbol(mongoQueryableNamedType.TypeArguments[0]);

                    var newLinqExpression = RewriteLinqExpression(node, deepestMongoQueryableNode, typesProcessor, semanticModel);

                    var linqContext = new ExpressionAnalysisContext(new ExpressionAnalysisNode(
                        node,
                        generatedMongoQueryableTypeName,
                        newLinqExpression));

                    analysisContexts.Add(linqContext);
                }
            }

            var linqAnalysis = new ExpressionsAnalysis()
            {
                AnalysisNodeContexts = analysisContexts.ToArray(),
                InvalidExpressionNodes = invalidExpressionNodes.ToArray(),
                TypesDeclarations = typesProcessor.TypesDeclarations
            };

            return linqAnalysis;
        }

        private static ExpressionSyntax GetNextNestedInvocation(ExpressionSyntax expressionSyntax) =>
            ((expressionSyntax as InvocationExpressionSyntax)?.Expression as MemberAccessExpressionSyntax)?.Expression;

        private static bool PreanalyzeLinqExpression(SyntaxNode linqExpressionNode, SemanticModel semanticModel, List<InvalidExpressionAnalysisNode> invalidLinqExpressionNodes)
        {
            var result = true;

            foreach (var lambdaExpression in linqExpressionNode.DescendantNodes().OfType<SimpleLambdaExpressionSyntax>())
            {
                foreach (var methodInvocation in lambdaExpression.DescendantNodes().OfType<InvocationExpressionSyntax>())
                {
                    var symbolInfo = semanticModel.GetSymbolInfo(methodInvocation);

                    if (symbolInfo.Symbol is IMethodSymbol methodSymbol &&
                        methodSymbol.ContainingType.SpecialType != SpecialType.System_String)
                    {
                        invalidLinqExpressionNodes.Add(new InvalidExpressionAnalysisNode(
                            methodInvocation,
                            new[] { LinqAnalysisErrorMessages.MethodInvocationNotSupported }));

                        result = false;
                    }
                }
            }

            return result;
        }

        private static SyntaxNode RewriteLinqExpression(SyntaxNode linqExpressionNode, SyntaxNode deepestMongoQueryableNode, TypesProcessor typesProcessor, SemanticModel semanticModel)
        {
            var result = linqExpressionNode;

            var mockCollectionNode = SyntaxFactory.IdentifierName(SyntaxFactory.Identifier(LinqAnalysisConsts.MockCollectionIdentifierName));
            var nodesRemapping = new Dictionary<SyntaxNode, SyntaxNode>()
            {
                { deepestMongoQueryableNode, mockCollectionNode }
            };

            foreach (var identifierNode in linqExpressionNode.DescendantNodes(n => n != deepestMongoQueryableNode).OfType<IdentifierNameSyntax>())
            {
                if (identifierNode == deepestMongoQueryableNode)
                {
                    continue;
                }

                var symbolInfo = semanticModel.GetSymbolInfo(identifierNode);

                if (symbolInfo.Symbol.Kind != SymbolKind.NamedType)
                    continue;

                var typeInfo = semanticModel.GetTypeInfo(identifierNode);
                var remmapedType = typesProcessor.GetTypeSymbolToGeneratedTypeMapping(typeInfo.Type);

                if (remmapedType == null)
                    continue;

                SyntaxNode nodeToReplace = identifierNode;
                while (nodeToReplace.Parent.IsKind(SyntaxKind.SimpleMemberAccessExpression))
                {
                    nodeToReplace = nodeToReplace.Parent;
                }

                var newNode = SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName(remmapedType),
                    SyntaxFactory.IdentifierName(identifierNode.Identifier.Text));

                if (nodesRemapping == null)
                {
                    nodesRemapping = new Dictionary<SyntaxNode, SyntaxNode>();
                }

                nodesRemapping[nodeToReplace] = newNode;
            }

            result = linqExpressionNode.ReplaceNodes(
                nodesRemapping.Keys,
                (n, _) => nodesRemapping[n]);

            return result;
        }
    }
}
