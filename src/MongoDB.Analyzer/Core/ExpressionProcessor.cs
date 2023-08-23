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

using TypeInfo = Microsoft.CodeAnalysis.TypeInfo;

namespace MongoDB.Analyzer.Core;

internal static class ExpressionProcessor
{
    internal record RewriteContext(
        AnalysisType AnalysisType,
        SyntaxNode Expression,
        SemanticModel SemanticModel,
        TypesProcessor TypesProcessor,
        ConstantsMapper ConstantsMapper);

    internal enum RewriteAction
    {
        Rewrite,
        Ignore,
        Invalid
    }

    internal record RewriteResult(
        RewriteAction RewriteAction,
        SyntaxNode NodeToReplace,
        SyntaxNode NewNode)
    {
        public RewriteResult(SyntaxNode NodeToReplace, SyntaxNode NewNode) :
            this(RewriteAction.Rewrite, NodeToReplace, NewNode)
        {
        }

        public static RewriteResult Ignore = new(RewriteAction.Ignore, null, null);
        public static RewriteResult Invalid = new(RewriteAction.Invalid, null, null);
    }

    public static SyntaxNode GetConstantReplacementNode(
       RewriteContext rewriteContext,
       ITypeSymbol typeSymbol,
       string originalNodeFullName = null)
    {
        ExpressionSyntax replacementNode = null;
        var (isNullable, underlingTypeSymbol) = typeSymbol.DiscardNullable();

        if (underlingTypeSymbol.TypeKind == TypeKind.Enum)
        {
            var underlyingEnumType = (underlingTypeSymbol as INamedTypeSymbol).EnumUnderlyingType.SpecialType;
            var literalSyntax = rewriteContext.ConstantsMapper.GetExpressionByType(underlyingEnumType, originalNodeFullName);
            replacementNode = GetEnumCastNode(underlingTypeSymbol, literalSyntax.Token.Value, rewriteContext.TypesProcessor, isNullable);
        }
        else if (underlingTypeSymbol.SpecialType != SpecialType.None)
        {
            replacementNode = rewriteContext.ConstantsMapper.GetExpressionByType(underlingTypeSymbol.SpecialType, originalNodeFullName);

            if (isNullable)
            {
                replacementNode = SyntaxFactory.CastExpression(
                    SyntaxFactoryUtilities.GetNullableType(underlingTypeSymbol.Name),
                    replacementNode);
            }
        }

        return replacementNode;
    }

    public static ExpressionSyntax GetEnumCastNode(
        ITypeSymbol typeSymbol,
        object constantValue,
        TypesProcessor typesProcessor,
        bool isNullable = false)
    {
        var remappedEnumTypeName = typesProcessor.GetTypeSymbolToGeneratedTypeMapping(typeSymbol);

        if (remappedEnumTypeName.IsNullOrWhiteSpace())
        {
            return null;
        }

        return SyntaxFactoryUtilities.GetCastConstantExpression(remappedEnumTypeName, constantValue, isNullable);
    }

    public static RewriteResult HandleField(
       RewriteContext rewriteContext,
       SyntaxNode identifierNode,
       SymbolInfo symbolInfo,
       TypeInfo typeInfo)
    {
        var fieldSymbol = symbolInfo.Symbol as IFieldSymbol;
        if (fieldSymbol?.HasConstantValue != true)
        {
            return SubstituteExpressionWithConstant(rewriteContext, identifierNode, symbolInfo, typeInfo);
        }

        if (!identifierNode.Parent.IsKind(SyntaxKind.SimpleMemberAccessExpression))
        {
            return RewriteResult.Ignore;
        }

        SyntaxNode replacementNode;
        if (fieldSymbol.Type.TypeKind == TypeKind.Enum)
        {
            replacementNode = GetEnumCastNode(fieldSymbol.Type, fieldSymbol.ConstantValue, rewriteContext.TypesProcessor);
        }
        else if (fieldSymbol.Type.SpecialType != SpecialType.None)
        {
            replacementNode = rewriteContext.ConstantsMapper.GetExpressionForConstant(fieldSymbol.Type.SpecialType, fieldSymbol.ConstantValue);
        }
        else
        {
            return RewriteResult.Invalid;
        }

        return new RewriteResult(identifierNode.Parent, replacementNode);
    }

    public static RewriteResult HandleMethod(
        RewriteContext rewriteContext,
        SyntaxNode identifierNode,
        SymbolInfo symbolInfo,
        TypeInfo typeInfo)
    {
        var methodSymbol = symbolInfo.Symbol as IMethodSymbol;
        if (methodSymbol.ReturnType == null)
        {
            return RewriteResult.Ignore;
        }

        switch (rewriteContext.AnalysisType)
        {
            case AnalysisType.Builders:
                {
                    if (methodSymbol.ReceiverType.IsIMongoQueryable() ||
                        methodSymbol.ReturnType.IsIMongoQueryable() ||
                        methodSymbol.IsBuilderMethod() ||
                        methodSymbol.IsFindFluentMethod())
                    {
                        return RewriteResult.Ignore;
                    }
                    break;
                }
            case AnalysisType.Linq:
                {
                    if (methodSymbol.ReceiverType.IsIQueryable() ||
                        methodSymbol.ReturnType.IsIQueryable())
                    {
                        return RewriteResult.Ignore;
                    }

                    break;
                }
        }

        if (IsChildOfLambdaOrQueryOrBuildersParameter(rewriteContext, identifierNode, symbolInfo))
        {
            return RewriteResult.Ignore;
        }

        var typeSymbol = typeInfo.ConvertedType ?? methodSymbol.ReturnType;
        var nodeToReplace = SyntaxFactoryUtilities.ResolveAccessExpressionNode(identifierNode);
        var replacementNode = GetConstantReplacementNode(
            rewriteContext,
            typeSymbol,
            nodeToReplace.ToString());

        if (replacementNode == null)
        {
            return RewriteResult.Invalid;
        }

        return new RewriteResult(nodeToReplace, replacementNode);
    }

    public static bool IsChildOfLambdaOrQueryOrBuildersParameter(
        RewriteContext rewriteContext,
        SyntaxNode identifier,
        SymbolInfo symbolInfo)
    {
        if (symbolInfo.Symbol.IsContainedInLambdaOrQueryParameter(rewriteContext.Expression))
        {
            return true;
        }

        var underlyingIdentifier = SyntaxFactoryUtilities.GetUnderlyingIdentifier(identifier);
        if (underlyingIdentifier == null)
        {
            return false;
        }

        if (rewriteContext.AnalysisType == AnalysisType.Builders &&
            underlyingIdentifier.Identifier.Text == "Builders")
        {
            return true;
        }

        var result = rewriteContext.SemanticModel
            .GetSymbolInfo(underlyingIdentifier)
            .Symbol?
            .IsContainedInLambdaOrQueryParameter(rewriteContext.Expression) == true;

        return result;
    } 

    public static GenericNameSyntax ProcessGenericType(RewriteContext rewriteContext, GenericNameSyntax genericNameSyntax)
    {
        var typeInfo = rewriteContext.SemanticModel.GetTypeInfo(genericNameSyntax);
        var remappedType = rewriteContext.TypesProcessor.ProcessTypeSymbol(typeInfo.Type);

        if (remappedType == null)
        {
            return null;
        }

        var typeArguments = new List<TypeSyntax>();

        foreach (var typeArgument in genericNameSyntax.TypeArgumentList.Arguments)
        {
            TypeSyntax typeSyntax;

            if (typeArgument is GenericNameSyntax nestedGenericNameSyntax)
            {
                typeSyntax = ProcessGenericType(rewriteContext, nestedGenericNameSyntax);

                if (typeSyntax == null)
                {
                    return null;
                }
            }
            else
            {
                var typeArgumentTypeInfo = rewriteContext.SemanticModel.GetTypeInfo(typeArgument);
                var typeArgumentRemappedType = rewriteContext.TypesProcessor.ProcessTypeSymbol(typeArgumentTypeInfo.Type);

                if (typeArgumentRemappedType == null)
                {
                    return null;
                }

                typeSyntax = SyntaxFactory.IdentifierName(typeArgumentRemappedType);
            }

            typeArguments.Add(typeSyntax);
        }

        var newGenericNameSyntax = SyntaxFactory.GenericName(
            SyntaxFactory.Identifier(remappedType),
            SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList(typeArguments)));

        return newGenericNameSyntax;
    }

    public static RewriteResult RemoveNonFluentParams(
        SyntaxNode syntaxNode,
        SymbolInfo symbolInfo,
        TypeInfo typeInfo)
    {
        var typeSymbol = symbolInfo.Symbol.Kind switch
        {
            SymbolKind.Field => (symbolInfo.Symbol as IFieldSymbol).Type,
            SymbolKind.Method => (symbolInfo.Symbol as IMethodSymbol).ReturnType,
            SymbolKind.NamedType => symbolInfo.Symbol as INamedTypeSymbol,
            SymbolKind.Local or
            SymbolKind.Parameter or
            SymbolKind.Property => typeInfo.Type,
            _ => null
        };

        if (typeSymbol.IsFindOptions())
        {
            var argumentNode = syntaxNode.GetParentArgumentSyntaxIfExists();
            if (argumentNode == null)
            {
                return RewriteResult.Invalid;
            }

            return new RewriteResult(argumentNode, SyntaxFactoryUtilities.NewFindOptionsArgument);
        }

        return null;
    }

    public static RewriteResult SubstituteExpressionWithConstant(
        RewriteContext rewriteContext,
        SyntaxNode simpleNameSyntax,
        SymbolInfo symbolInfo,
        TypeInfo typeInfo)
    {
        if (IsChildOfLambdaOrQueryOrBuildersParameter(rewriteContext, simpleNameSyntax, symbolInfo) ||
            simpleNameSyntax.IsMemberOfAnonymousObject() ||
            typeInfo.Type == null)
        {
            return RewriteResult.Ignore;
        }

        var nodeToReplace = SyntaxFactoryUtilities.ResolveAccessExpressionNode(simpleNameSyntax);
        var type = rewriteContext.AnalysisType == AnalysisType.Builders ? typeInfo.Type : typeInfo.ConvertedType;

        var replacementNode = GetConstantReplacementNode(
            rewriteContext,
            type,
            nodeToReplace.ToString());

        if (replacementNode == null)
        {
            return RewriteResult.Invalid;
        }

        return new RewriteResult(nodeToReplace, replacementNode);
    }
}
