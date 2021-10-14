using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MongoDB.Analyzer.Tests.Infrastructure
{
    public sealed class CodeBasedTestCasesSourceAttribute : Attribute, ITestDataSource
    {
        public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        {
            var testBaseType = methodInfo.DeclaringType.BaseType;
            var testCasesClassType = testBaseType.GenericTypeArguments.Single();

            var testCasesMethods = testCasesClassType.
                GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).
                Where(m => m.MemberType == MemberTypes.Method && m.DeclaringType == testCasesClassType);

            return testCasesMethods.Select(m => new object[] { m.Name });
        }

        public string GetDisplayName(MethodInfo methodInfo, object[] data) => data.Single().ToString();
    }
}
