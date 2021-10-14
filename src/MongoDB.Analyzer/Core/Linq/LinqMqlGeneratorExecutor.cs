// Copyright 2021-present MongoDB Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace MongoDB.Analyzer.Core.Linq;

internal record MQLResult(string Mql, bool Linq3Only, Exception Exception);

internal sealed class LinqMqlGeneratorExecutor
{
    private readonly Type _testClassType;
    private readonly bool _isLinq3Driver;
    private readonly bool _isLinq3Default;

    public string DriverVersion { get; }

    public LinqMqlGeneratorExecutor(Type testClassType, LinqVersion maxSupportedLinqVerison, LinqVersion defaultLinqVersion)
    {
        _testClassType = testClassType;
        _isLinq3Driver = maxSupportedLinqVerison == LinqVersion.V3;
        _isLinq3Default = defaultLinqVersion == LinqVersion.V3;

        DriverVersion = GetDriverVersion();
    }

    private string GetDriverVersion()
    {
        try
        {
            return (string)_testClassType.GetMethod(MqlGeneratorSyntaxElements.GetDriverVersion).Invoke(null, new object[] { });
        }
        catch { }

        return null;
    }

    public MQLResult GenerateMql(string methodName)
    {
        MQLResult result;

        var mqlMethod = _testClassType.GetMethod(methodName);

        if (!_isLinq3Driver) // not LINQ3 driver
        {
            result = Execute(mqlMethod, isLinq3: false);
        }
        else if (_isLinq3Default) // LINQ3 is default in LINQ3 driver
        {
            result = Execute(mqlMethod, isLinq3: true);
        }
        else // LINQ2 is default in LINQ3 driver
        {
            var resultLinq2 = Execute(mqlMethod, isLinq3: false);

            if (resultLinq2.Exception != null)
            {
                var resultLinq3 = Execute(mqlMethod, isLinq3: true);

                result = new MQLResult(resultLinq3.Mql, true, resultLinq2.Exception);
            }
            else
            {
                result = resultLinq2;
            }
        }

        return result;
    }

    private MQLResult Execute(MethodInfo mqlMethod, bool isLinq3)
    {
        Exception exception = null;
        string mql = null;

        try
        {
            var executeResult = mqlMethod.Invoke(null, new object[] { isLinq3 });

            if (executeResult is string executeResultString)
            {
                mql = executeResultString;
            }
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        return new MQLResult(mql, false, exception);
    }
}
