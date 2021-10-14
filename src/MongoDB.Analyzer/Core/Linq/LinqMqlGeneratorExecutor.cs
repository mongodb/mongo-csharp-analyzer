using System;

namespace MongoDB.Analyzer.Core.Linq
{
    internal sealed class LinqMqlGeneratorExecutor
    {
        private readonly Type _testClassType;

        public LinqMqlGeneratorExecutor(Type testClassType)
        {
            _testClassType = testClassType;
        }

        public (Exception exception, string mql) ExecuteMethod(string methodName)
        {
            Exception exception = null;
            string mql = null;

            var testMethod = _testClassType.GetMethod(methodName);

            try
            {
                var executeResult = testMethod.Invoke(null, new object[] { });

                if (executeResult is string executeResultString)
                {
                    mql = executeResultString;
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            return (exception, mql);
        }
    }
}
