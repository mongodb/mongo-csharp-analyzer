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

namespace MongoDB.Analyzer.Core.Poco;

public static class PocoDataFiller
{
    private const string CollectionNamespace = "System.Collections.Generic";
    private const int MaxDepth = 3;

    public static void PopulatePoco(object poco) =>
        SetPropertiesAndFields(poco, MaxDepth);

    private static object GetPropertyOrFieldValue(Type memberType, string memberName, int levelsLeft)
    {
        if (memberType.IsPrimitive)
        {
            return Convert.ChangeType(memberName.Length % 10, memberType);
        }
        else if (memberType.IsString())
        {
            return $"{memberName}_val";
        }
        else if (memberType.IsArray)
        {
            return HandleArray(memberType);
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

    private static Array HandleSingleDimensionalArray(Type arrayType) =>
        Array.CreateInstance(arrayType.GetElementType(), 0);

    private static bool IsString(this Type type) =>
        type == typeof(string);

    private static bool IsSupportedCollection(this Type type) =>
        CollectionNamespace == type.Namespace &&
        type.GenericTypeArguments.Length == 1;

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
