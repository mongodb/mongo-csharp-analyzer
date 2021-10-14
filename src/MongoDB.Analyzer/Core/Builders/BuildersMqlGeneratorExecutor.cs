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

namespace MongoDB.Analyzer.Core.Builders;

internal record MQLResult(string Mql, Exception Exception);

internal sealed class BuildersMqlGeneratorExecutor
{
    private readonly Type _testClassType;

    public string DriverVersion { get; }

    public BuildersMqlGeneratorExecutor(Type testClassType)
    {
        _testClassType = testClassType;

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
        Exception exception = null;
        string mql = null;

        try
        {
            var mqlMethod = _testClassType.GetMethod(methodName);

            var executeResult = mqlMethod.Invoke(null, new object[] { });

            if (executeResult is string executeResultString)
            {
                mql = executeResultString;
            }
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        return new MQLResult(mql, exception);
    }
}
