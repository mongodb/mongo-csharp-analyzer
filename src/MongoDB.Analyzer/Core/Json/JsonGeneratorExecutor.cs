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

using MongoDB.Analyzer.Core.HelperResources;

namespace MongoDB.Analyzer.Core.Json;

internal record JsonResult(string Json, Exception exception);

internal sealed class JsonGeneratorExecutor
{
    private readonly Type _testClassType;
    public string DriverVersion { get; }

    public JsonGeneratorExecutor(Type testClassType)
    {
        _testClassType = testClassType;
        DriverVersion = GetDriverVersion();
    }

    private string GetDriverVersion()
    {
        try
        {
            return (string)_testClassType.GetMethod(JsonSyntaxElements.Json.GetDriverVersion).Invoke(null, new object[] { });
        }
        catch { }

        return null;
    }

    public JsonResult GenerateJson(string methodName)
    {
        Exception exception = null;
        string json = null;

        try
        {
            var jsonMethod = _testClassType.GetMethod(methodName);

            var executeResult = jsonMethod.Invoke(null, new object[] { });

            if (executeResult is string executeResultString)
            {
                json = executeResultString;
            }
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        return new JsonResult(json, exception);
    }
}