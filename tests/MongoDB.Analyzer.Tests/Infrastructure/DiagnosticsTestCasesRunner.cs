using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoDB.Analyzer.Tests.Infrastructure
{
    public abstract class DiagnosticsTestCasesRunner
    {
        private readonly string _textCasesFileName;
        private IDictionary<string, DiagnosticsTestCase> _testCases;

        public DiagnosticsTestCasesRunner(string testCasesFileName)
        {
            _textCasesFileName = testCasesFileName;
        }

        [TestInitialize]
        public async Task TestInit()
        {
            _testCases = await TestCasesProvider.ExtractTestCaseFromFile(_textCasesFileName);
        }

        protected DiagnosticsTestCase GetTestCase(string key)
        {
            if (!_testCases.TryGetValue(key, out var testCase))
            {
                throw new Exception("Diagnostics not found");
            }

            return testCase;
        }

        protected void VerifyTestCase(string testCaseName)
        {
            var testCase = GetTestCase(testCaseName);

            Assert.AreEqual(testCase.Expected.Length, testCase.Actual.Length);

            Assert.AreEqual(testCase.Expected[0].Descriptor, testCase.Actual[0].Descriptor);
            Assert.AreEqual(testCase.Expected[0].GetMessage(), testCase.Actual[0].GetMessage());
        }
    }
}
