using Microsoft.CodeAnalysis;

namespace MongoDB.Analyzer.Tests.Infrastructure
{
    public record DiagnosticsTestCase(string Name, Diagnostic[] Actual, Diagnostic[] Expected);
}
