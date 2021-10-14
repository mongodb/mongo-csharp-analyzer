namespace MongoDB.Analyzer.Core.Linq
{
    internal static class LinqAnalysisConsts
    {
        public const string AnalysisAssemblyName = nameof(AnalysisAssemblyName);

        public const string MockCollectionFilename = "MongoDB.Analyzer.Core.Linq.MongoCollectionMock.cs";
        public const string MockCollectionIdentifierName = "mockCollection";

        public const string TestClass = "LinqTestClass";
        public const string TestNamespace = "LinqTestNamespace";
        public const string TestClassFullName = TestNamespace + "." + TestClass;

        public const string GeneratedTypeName = "GenType";
    }

    internal static class LinqAnalysisErrorMessages
    {
        public const string MethodInvocationNotSupported = "Method invocation is not supported linq expression";
    }
}
