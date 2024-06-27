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

using MongoDB.Analyzer.Core.Builders;
using MongoDB.Analyzer.Core.Linq;
using static MongoDB.Analyzer.Core.ExpressionRewriter;
using TypeInfo = Microsoft.CodeAnalysis.TypeInfo;

namespace MongoDB.Analyzer.Core;

internal static class ExpressionProcessor
{
    private enum NodeType
    {
        Unknown = 0,
        Ignore,
        Invalid,
        Builders,
        Fluent,
        EF,
        Linq,
        Poco
    }

    public static ExpressionsAnalysis ProcessSemanticModel(MongoAnalysisContext context, AnalysisType analysisType)
    {
        //Early Exit for Poco Analysis
        if (analysisType == AnalysisType.Poco && 
            (context.Settings.PocoAnalysisVerbosity == PocoAnalysisVerbosity.None ||
            context.Settings.PocoLimit <= 0))
        {
            return default;
        }

        //Context Parameters
        var semanticModel = context.SemanticModelAnalysisContext.SemanticModel;
        var syntaxTree = semanticModel.SyntaxTree;
        var rootNode = syntaxTree.GetRoot();

        //Expression Processing Variables
        var processedSyntaxNodes = new HashSet<SyntaxNode>();
        var analysisContexts = new List<ExpressionAnalysisContext>();
        var invalidExpressionNodes = new List<InvalidExpressionAnalysisNode>();
        var typesProcessor = context.TypesProcessor;

        //Builders Specific Parameters
        var buildersToAnalysisContextMap = context.Settings.EnableVariableTracking ? new Dictionary<SyntaxNode, ExpressionAnalysisContext>() : null;

        //Get Candidate Nodes
        var candidateNodes = GetDescendantNodes(rootNode, analysisType, processedSyntaxNodes);

        //Iterate through Descendant Nodes
        foreach (var node in candidateNodes)
        {
            if (analysisType == AnalysisType.Poco &&
                analysisContexts.Count == context.Settings.PocoLimit)
            {
                break;
            }

            IEnumerable<SyntaxNode> nodesToRewrite = null;
            var (nodeType, namedType, expressionNode) = GetNodeType(semanticModel, node, analysisType);

            switch (nodeType)
            {
                case NodeType.Builders:
                    nodesToRewrite = GetBuildersDefinitionNodes(semanticModel, expressionNode);
                    processedSyntaxNodes.Add(node);
                    break;
                case NodeType.Fluent:
                    var collectionNode = expressionNode
                        .NestedInvocations()
                        .FirstOrDefault(n => semanticModel.GetTypeInfo(n).Type.IsSupportedIMongoCollection());

                    if (collectionNode == null)
                    {
                        continue;
                    }

                    nodesToRewrite = new SyntaxNode[] { collectionNode };
                    processedSyntaxNodes.Add(node);
                    break;
                case NodeType.Linq or NodeType.EF:
                    var (deepestMongoQueryableNode, isValid) = ValidateMethodChain(context, node);
                    processedSyntaxNodes.Add(node);

                    var mongoQueryableTypeInfo = semanticModel.GetTypeInfo(deepestMongoQueryableNode);
                    if (IsInvalidCollectionOrQueryableType(mongoQueryableTypeInfo, analysisType))
                    {
                        continue;
                    }

                    nodesToRewrite = new SyntaxNode[] { deepestMongoQueryableNode };
                    break;
                case NodeType.Poco:
                    break;
                case NodeType.Invalid:
                    {
                        processedSyntaxNodes.Add(node);
                        continue;
                    }
                default:
                    {
                        continue;
                    }
            }

            try
            {
                if (Preanalyze(context, analysisType, expressionNode, invalidExpressionNodes))
                {
                    if (analysisType == AnalysisType.Poco)
                    {
                        var generatedClassName = typesProcessor.ProcessTypeSymbol(namedType);
                        var generatedClassNode = (ClassDeclarationSyntax)(typesProcessor.GetTypeSymbolToMemberDeclarationMapping(namedType));
                        var expressionContext = new ExpressionAnalysisContext(new ExpressionAnalysisNode(node, null, generatedClassNode, null, node.GetLocation()));
                        analysisContexts.Add(expressionContext);
                        continue;
                    }

                    foreach (var typeArgument in namedType?.TypeArguments)
                    {
                        typesProcessor.ProcessTypeSymbol(typeArgument);
                    }

                    var rewriteContext = GenerateRewriteContext(expressionNode, nodesToRewrite, analysisType, context);
                    var (newExpression, constantsMapper) = RewriteExpression(rewriteContext);

                    if (newExpression != null)
                    {
                        var expressionContext = new ExpressionAnalysisContext(new(
                            expressionNode,
                            typesProcessor.GetTypeSymbolToGeneratedTypeMapping(namedType.TypeArguments.First()),
                            newExpression,
                            constantsMapper,
                            expressionNode.GetLocation()));

                        analysisContexts.Add(expressionContext);
                        buildersToAnalysisContextMap?.Add(expressionNode, expressionContext);
                    }
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

        var analysis = new ExpressionsAnalysis()
        {
            AnalysisNodeContexts = analysisContexts.ToArray(),
            InvalidExpressionNodes = invalidExpressionNodes.ToArray(),
            TypesDeclarations = typesProcessor.TypesDeclarations
        };

        context.Logger.Log($"MQL/JSON: Found {analysis.AnalysisNodeContexts.Length} expressions.");

        return analysis;
    }

    private static bool Preanalyze(MongoAnalysisContext context, AnalysisType analysisType, SyntaxNode expressionNode, List<InvalidExpressionAnalysisNode> invalidExpressionNodes) =>
        analysisType switch
        {
            AnalysisType.Linq => PreanalyzeLinqExpression(expressionNode, context.SemanticModelAnalysisContext.SemanticModel, invalidExpressionNodes),
            AnalysisType.EF => PreanalyzeLinqExpression(expressionNode, context.SemanticModelAnalysisContext.SemanticModel, invalidExpressionNodes),
            AnalysisType.Poco => PreanalyzeClassDeclaration(context, context.SemanticModelAnalysisContext.SemanticModel.GetDeclaredSymbol(expressionNode as ClassDeclarationSyntax)),
            _ => true
        };
    
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

    private static bool ContainsBsonAttributes(MongoAnalysisContext context, INamedTypeSymbol classSymbol) =>
        classSymbol.GetAttributes().Where(attribute => IsValidBsonAttribute(context, attribute)).AnySafe();

    private static bool ContainsFieldsWithBsonAttributes(MongoAnalysisContext context, INamedTypeSymbol classSymbol) =>
        classSymbol.GetMembers().OfType<IFieldSymbol>().SelectMany(field => field.GetAttributes().Where(attribute => IsValidBsonAttribute(context, attribute))).AnySafe();

    private static bool ContainsPropertiesWithBsonAttributes(MongoAnalysisContext context, INamedTypeSymbol classSymbol) =>
        classSymbol.GetMembers().OfType<IPropertySymbol>().SelectMany(property => property.GetAttributes().Where(attribute => IsValidBsonAttribute(context, attribute))).AnySafe();

    private static bool IsValidBsonAttribute(MongoAnalysisContext context, AttributeData attribute) =>
        attribute.AttributeClass.IsSupportedBsonAttribute() || context.TypesProcessor.GetTypeSymbolToMemberDeclarationMapping(attribute.AttributeClass) != null;


    private static bool PreanalyzeClassDeclaration(MongoAnalysisContext context, INamedTypeSymbol classSymbol) =>
        context.Settings.PocoAnalysisVerbosity == PocoAnalysisVerbosity.All ||
        (classSymbol != null &&
        (context.TypesProcessor.IsUserTypeProcessed(classSymbol) ||
         ContainsBsonAttributes(context, classSymbol) ||
         ContainsPropertiesWithBsonAttributes(context, classSymbol) ||
         ContainsFieldsWithBsonAttributes(context, classSymbol)));

    private static RewriteContext GenerateRewriteContext(SyntaxNode expression, IEnumerable<SyntaxNode> rootNodes, AnalysisType analysisType, MongoAnalysisContext context) =>
        analysisType switch
        {
            AnalysisType.Linq => RewriteContext.Linq(expression, rootNodes.First(), context.SemanticModelAnalysisContext.SemanticModel, context.TypesProcessor),
            AnalysisType.EF => RewriteContext.EF(expression, rootNodes.First(), context.SemanticModelAnalysisContext.SemanticModel, context.TypesProcessor),
            AnalysisType.Builders => RewriteContext.Builders(expression, rootNodes, context.SemanticModelAnalysisContext.SemanticModel, context.TypesProcessor),
            _ => null,
        };

    //Get Nodes that are Builder Definitions: DONE
    private static IEnumerable<SyntaxNode> GetBuildersDefinitionNodes(SemanticModel semanticModel, SyntaxNode expressionNode)
    {
        var nodesProcessed = new HashSet<SyntaxNode>();
        var builderDefinitionNodes = new List<SyntaxNode>();

        foreach (var node in expressionNode.DescendantNodesWithSkipList(nodesProcessed))
        {
            if (semanticModel.GetTypeInfo(node).Type.IsBuilder())
            {
                nodesProcessed.Add(node);

                // Skip MongoDB.Driver.Builders<T> nodes
                if (node is MemberAccessExpressionSyntax memberAccessExpressionSyntax &&
                    semanticModel.GetSymbolInfo(memberAccessExpressionSyntax.Expression).Symbol is INamedTypeSymbol namedTypeSymbol &&
                    namedTypeSymbol.IsBuildersContainer() &&
                    semanticModel.GetAliasInfo(memberAccessExpressionSyntax.Expression) == null)
                {
                    continue;
                }

                builderDefinitionNodes.Add(node);
            }
        }

        return builderDefinitionNodes;
    }

    private static IEnumerable<SyntaxNode> GetDescendantNodes(SyntaxNode rootNode, AnalysisType analysisType, HashSet<SyntaxNode> processedSyntaxNodes) =>
        analysisType switch
        {
            AnalysisType.Linq => rootNode.DescendantNodesWithSkipList<ExpressionSyntax>(processedSyntaxNodes),
            AnalysisType.EF => rootNode.DescendantNodesWithSkipList<ExpressionSyntax>(processedSyntaxNodes),
            AnalysisType.Builders => rootNode.DescendantNodesWithSkipList(processedSyntaxNodes),
            AnalysisType.Poco => rootNode.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>(),
            _ => rootNode.DescendantNodesWithSkipList(processedSyntaxNodes)
        };

    //Get the Type of Node, Symbol Corresponding to the Query Definition, and the Expression itself
    private static (NodeType NodeType, INamedTypeSymbol NamedSymbol, SyntaxNode ExpressionNode) GetNodeType(SemanticModel semanticModel, SyntaxNode node, AnalysisType analysisType)
    {
        if (analysisType == AnalysisType.Linq || analysisType == AnalysisType.EF)
        {
            //LINQ Query Expression Syntax
            if (node is QueryExpressionSyntax queryExpressionSyntax)
            {
                var expression = queryExpressionSyntax.FromClause.Expression;
                var queryMethodSymbol = semanticModel.GetTypeInfo(expression).Type;
                if (!queryMethodSymbol.IsIMongoQueryable())
                {
                    return (NodeType.Ignore, default, default);
                }

                var deepestMongoQueryableNode = expression;
                var mongoQueryableNamedType = semanticModel.GetTypeInfo(deepestMongoQueryableNode).Type as INamedTypeSymbol;
                return (NodeType.Linq, mongoQueryableNamedType, node);
            }

            else if (node is InvocationExpressionSyntax invocationExpressionSyntax)
            {
                var methodSymbol = invocationExpressionSyntax.GetMethodSymbol(semanticModel);

                if (!methodSymbol.IsDefinedInMongoLinqOrSystemLinq() ||
                    !methodSymbol.ReceiverType.IsIQueryable() ||
                    !methodSymbol.ReturnType.IsIQueryable())
                {
                    return (NodeType.Ignore, default, default);
                }

                var deepestMongoQueryableNode = invocationExpressionSyntax
                    .NestedInvocations()
                    .FirstOrDefault(n =>
                    {
                        var currentMethodSymbol = n.GetMethodSymbol(semanticModel);

                        // Find the first method that is not receiving IQueryable or is not defined in System.Linq or MongoDB.Driver.Linq
                        return !((currentMethodSymbol?.ReceiverType).IsIQueryable() && currentMethodSymbol.IsDefinedInMongoLinqOrSystemLinq());
                    });

                var mongoQueryableNamedType = semanticModel.GetTypeInfo(deepestMongoQueryableNode).Type as INamedTypeSymbol;
                return (NodeType.Linq, mongoQueryableNamedType, node);
            }

            return (NodeType.Ignore, default, default);
        }
        else if (analysisType == AnalysisType.Poco)
        {
            var namedTypeSymbol = semanticModel.GetDeclaredSymbol(node as ClassDeclarationSyntax);
            return (NodeType.Poco, namedTypeSymbol, node);
        }
        else if (analysisType == AnalysisType.Builders)
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
                    var childNodeType = GetNodeType(semanticModel, binaryExpressionSyntax.Left, analysisType);

                    if (childNodeType.NodeType == NodeType.Builders)
                    {
                        childNodeType = GetNodeType(semanticModel, binaryExpressionSyntax.Right, analysisType);

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

        return default;
    }

    //validate IMongoQueryable, DBSet, etc
    private static bool IsInvalidCollectionOrQueryableType(TypeInfo queryableTypeInfo, AnalysisType analysisType) =>
        analysisType switch
        {
            AnalysisType.EF => !queryableTypeInfo.Type.IsEF() ||
                queryableTypeInfo.Type is not INamedTypeSymbol EFQueryableNamedType ||
                EFQueryableNamedType.TypeArguments.Length != 1 ||
                !EFQueryableNamedType.TypeArguments[0].IsSupportedMongoCollectionType(),
            AnalysisType.Linq => !queryableTypeInfo.Type.IsIMongoQueryable() ||
                queryableTypeInfo.Type is not INamedTypeSymbol mongoQueryableNamedType ||
                mongoQueryableNamedType.TypeArguments.Length != 1 ||
                !mongoQueryableNamedType.TypeArguments[0].IsSupportedMongoCollectionType(),
            _ => false
        };

    //Validate the LINQ Method chain
    private static (SyntaxNode queryableNode, bool isValid) ValidateMethodChain(MongoAnalysisContext context, SyntaxNode node)
    {
        var semanticModel = context.SemanticModelAnalysisContext.SemanticModel;

        if (node is QueryExpressionSyntax queryNode)
        {
            var expression = queryNode.FromClause.Expression;
            var queryMethodSymbol = semanticModel.GetTypeInfo(expression).Type;
            if (!queryMethodSymbol.IsIMongoQueryable())
            {
                return default;
            }

            return (queryNode.FromClause.Expression, true);
        }
        else if (node is InvocationExpressionSyntax invocationNode)
        {
            var methodSymbol = invocationNode.GetMethodSymbol(semanticModel);

            if (!methodSymbol.IsDefinedInMongoLinqOrSystemLinq() ||
                !methodSymbol.ReceiverType.IsIQueryable() ||
                !methodSymbol.ReturnType.IsIQueryable())
            {
                return default;
            }

            var deepestMongoQueryableNode = invocationNode
                .NestedInvocations()
                .FirstOrDefault(n =>
                {
                    var currentMethodSymbol = n.GetMethodSymbol(semanticModel);

                    // Find the first method that is not receiving IQueryable or is not defined in System.Linq or MongoDB.Driver.Linq
                    return !((currentMethodSymbol?.ReceiverType).IsIQueryable() && currentMethodSymbol.IsDefinedInMongoLinqOrSystemLinq());
                });

            return (deepestMongoQueryableNode, true);
        }

        return default;
    }
}

