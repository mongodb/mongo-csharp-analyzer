namespace MongoDB.Analyzer.Tests.Infrastructure
{
    public abstract class LinqTestCasesRunner<TTestCasesClassName> : DiagnosticsTestCasesRunner
    {
        public LinqTestCasesRunner() : 
            base($"Linq\\{typeof(TTestCasesClassName).Name}.cs")
        {
        }
    }
}
