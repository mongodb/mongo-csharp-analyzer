using System;
using System.Text;

namespace MongoDB.Analyzer.Core.Linq
{
    internal sealed class LinqGeneratorTemplateBuilder
    {
        private readonly StringBuilder _stringBuilder;
        private int _testMethodsCount;
        private bool _isFinalized = false;

        public LinqGeneratorTemplateBuilder()
        {
            _testMethodsCount = 0;
            _stringBuilder = new StringBuilder();

            _stringBuilder.Append($@"
                using System.Dynamic;
                using System.Linq;
                using MongoDB.Driver;
                using MongoDB.Analyzer.Core.Linq;

                namespace {LinqAnalysisConsts.TestNamespace}
                {{
                    public static class {LinqAnalysisConsts.TestClass}
                    {{");
        }

        public string AddTest(string collectionType, string linqExpression)
        {
            if (_isFinalized)
            {
                throw new InvalidOperationException("Template is already finalized.");
            }

            var testMethodName = $"Test_{ _testMethodsCount++}";
            _stringBuilder.Append($@"
                        public static string {testMethodName}()
                        {{
                            var {LinqAnalysisConsts.MockCollectionIdentifierName} = new MongoCollectionMock<{collectionType}>().AsQueryable();
                            var mongoQueryable = {linqExpression};
                            mongoQueryable.FirstOrDefault();
                            return mongoQueryable.ToString();
                        }}
            ");

            return testMethodName;
        }

        public string Finalize()
        {
            _stringBuilder.Append(@"}
                }");

            _isFinalized = true;
            return _stringBuilder.ToString();
        }
    }
}
