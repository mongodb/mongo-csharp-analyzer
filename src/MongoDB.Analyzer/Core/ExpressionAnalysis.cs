using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MongoDB.Analyzer.Core
{
    internal sealed class ExpressionsAnalysis
    {
        public MemberDeclarationSyntax[] TypesDeclarations { get; set; }

        public ExpressionAnalysisContext[] AnalysisNodeContexts { get; set; }
        public InvalidExpressionAnalysisNode[] InvalidExpressionNodes { get; set; }
    }

    internal record ExpressionAnalysisContext(ExpressionAnalysisNode Node)
    {
       public string EvaluationMethodName { get; set; }
    }

    internal abstract record ExpressionAnalysisNodeBase(SyntaxNode OriginaExpression);

    internal record ExpressionAnalysisNode(SyntaxNode OriginaExpression, string ArgumentTypeName, SyntaxNode RewrittenExpression) :
        ExpressionAnalysisNodeBase(OriginaExpression);

    internal record InvalidExpressionAnalysisNode(SyntaxNode OriginaExpression, string[] Errors) : 
        ExpressionAnalysisNodeBase(OriginaExpression);
}
