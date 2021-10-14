using Microsoft.CodeAnalysis;

namespace MongoDB.Analyzer.Core.Linq
{
    public static class LinqDiagnosticsRules
    {
        private const string DiagnosticIdNotSupportedLinqExpression = "NotSupportedLinqExpression";
        private const string DiagnosticIdLinq2MQL = "MongoLinq2MQL";

        public static readonly DiagnosticDescriptor DiagnosticRuleNotSupportedLinqExpression = new DiagnosticDescriptor(
            DiagnosticIdNotSupportedLinqExpression,
            "Not supported linq expression",
            "{0}",
            "Syntax",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public static DiagnosticDescriptor DiagnosticRuleLinq2MQL { get; } = new DiagnosticDescriptor(
            DiagnosticIdLinq2MQL,
            "Linq to MQL",
            "{0}",
            "Syntax",
            DiagnosticSeverity.Info,
            isEnabledByDefault: true);

        public static DiagnosticDescriptor[] DiagnosticsRules { get; } = new[]
        {
            DiagnosticRuleNotSupportedLinqExpression,
            DiagnosticRuleLinq2MQL
        };
    }
}
