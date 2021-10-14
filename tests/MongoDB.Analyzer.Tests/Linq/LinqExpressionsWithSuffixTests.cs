﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Analyzer.Tests.Common.TestCases.Linq;
using MongoDB.Analyzer.Tests.Infrastructure;

namespace MongoDB.Analyzer.Tests.Linq
{
    [TestClass]
    public sealed class LinqExpressionsWithSuffixTests : LinqTestCasesRunner<LinqExpressionsWithSuffixTestCases>
    {
        [DataTestMethod]
        [CodeBasedTestCasesSource]
        public void TestCase(string testCaseName) => VerifyTestCase(testCaseName);
    }
}