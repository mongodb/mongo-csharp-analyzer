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

using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace MongoDB.Analyzer.Core.Poco;

public static class PocoDataFiller
{
    private const string CollectionNamespace = "System.Collections.Generic";
    private const string JsonDataResource = "MongoDB.Analyzer.Core.Poco.Data.Data.json";
    private const int MaxDepth = 3;

    private static readonly ConcurrentDictionary<string, string[]> s_jsonData;
    private static readonly Regex s_jsonDataRegexPattern;
    private static readonly HashSet<string> s_supportedSystemTypes = new()
    {
        "System.DateTime",
        "System.TimeSpan"
    };

    static PocoDataFiller()
    {
        s_jsonData = LoadJsonData();
        s_jsonDataRegexPattern = new Regex(string.Join("|", s_jsonData.Keys.OrderBy(key => key)), RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }

    public static void PopulatePoco(object poco) =>
        SetPropertiesAndFields(poco, MaxDepth);

    private static object GetPropertyOrFieldValue(Type memberType, string memberName, int levelsLeft)
    {
        if (memberType.IsPrimitive)
        {
            return HandlePrimitive(memberName, memberType);
        }
        else if (memberType.IsString())
        {
            return HandleString(memberName);
        }
        else if (memberType.IsArray)
        {
            return HandleArray(memberType);
        }
        else if (memberType.IsSupportedSystemType())
        {
            return HandleSystemType(memberType, memberName);
        }
        else if (memberType.IsSupportedCollection())
        {
            return Activator.CreateInstance(typeof(List<>)
                .MakeGenericType(memberType.GenericTypeArguments.First()));
        }
        else if (memberType.IsClass && levelsLeft > 0)
        {
            return HandleClass(memberType, levelsLeft);
        }

        return null;
    }

    private static Array HandleArray(Type arrayType) =>
        arrayType.GetArrayRank() > 1 ? HandleMultiDimensionalArray(arrayType) : HandleSingleDimensionalArray(arrayType);

    private static object HandleClass(Type memberType, int levelsLeft)
    {
        var classObject = Activator.CreateInstance(memberType);
        SetPropertiesAndFields(classObject, levelsLeft - 1);
        return classObject;
    }

    private static Array HandleMultiDimensionalArray(Type arrayType) =>
        Array.CreateInstance(arrayType.GetElementType(), Enumerable.Repeat(0, arrayType.GetArrayRank()).ToArray());

    private static object HandlePrimitive(string memberName, Type memberType)
    {
        var match = s_jsonDataRegexPattern.Match(memberName);
        if (match.Success)
        {
            var data = s_jsonData[match.Value];
            return Convert.ChangeType(data[memberName.Length % data.Length], memberType);
        }

        return Convert.ChangeType(memberName.Length % 10, memberType);
    }

    private static Array HandleSingleDimensionalArray(Type arrayType) =>
        Array.CreateInstance(arrayType.GetElementType(), 0);

    private static object HandleString(string memberName)
    {
        var match = s_jsonDataRegexPattern.Match(memberName);
        if (match.Success)
        {
            var data = s_jsonData[match.Value];
            return data[memberName.Length % data.Length];
        }

        return $"{memberName}_val";
    }

    private static object HandleSystemType(Type systemType, string memberName) =>
        systemType.FullName switch
        {
            "System.DateTime" => new DateTime(memberName.Length * 100, memberName.Length % 12, memberName.Length % 30),
            "System.TimeSpan" => new TimeSpan(memberName.Length % 24, memberName.Length % 60, memberName.Length % 60),
            _ => throw new ArgumentOutOfRangeException(nameof(systemType), systemType, "Unsupported system type")
        };

    private static bool IsString(this Type type) =>
        type == typeof(string);

    private static bool IsSupportedCollection(this Type type) =>
        CollectionNamespace == type.Namespace &&
        type.GenericTypeArguments.Length == 1;

    private static bool IsSupportedSystemType(this Type type) =>
        s_supportedSystemTypes.Contains(type.FullName);

    private static ConcurrentDictionary<string, string[]> LoadJsonData()
    {
        using (var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(JsonDataResource))
        using (var streamReader = new StreamReader(resourceStream))
        {
            return JsonConvert.DeserializeObject<ConcurrentDictionary<string, string[]>>(streamReader.ReadToEnd());
        }
    }

    private static void SetPropertiesAndFields(object poco, int levelsLeft)
    {
        var pocoType = poco.GetType();

        foreach (var propertyInfo in pocoType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            try
            {
                var memberValue = GetPropertyOrFieldValue(propertyInfo.PropertyType, propertyInfo.Name, levelsLeft);
                propertyInfo.SetValue(poco, memberValue);
            }
            catch
            {
                continue;
            }
        }

        foreach (var fieldInfo in pocoType.GetFields(BindingFlags.Instance | BindingFlags.Public))
        {
            try
            {
                var memberValue = GetPropertyOrFieldValue(fieldInfo.FieldType, fieldInfo.Name, levelsLeft);
                fieldInfo.SetValue(poco, memberValue);
            }
            catch
            {
                continue;
            }
        }
    }
}
