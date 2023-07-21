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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MongoDB.Driver;

namespace MongoDB.Analyzer.Helpers.Json
{
    public static class PropertyAndFieldHandler
    {
        private const string s_collectionNamespace = "System.Collections.Generic";

        public static void SetAttributes(object poco, int depth, int depthLimit)
        {
            foreach (var memberInfo in poco.GetType().GetMembers())
            {
                try
                {
                    if (memberInfo.MemberType == MemberTypes.Property)
                    {
                        ProcessProperty(poco, memberInfo, depth, depthLimit);
                    }
                    else if (memberInfo.MemberType == MemberTypes.Field)
                    {
                        ProcessField(poco, memberInfo, depth, depthLimit);
                    }
                }
                catch
                {
                    continue;
                }
            }
        }

        private static IEnumerable<object> GetArgumentList(Type type) =>
            type.GetConstructors().FirstOrDefault()?.GetParameters().Select(parameter => parameter.DefaultValue);

        private static Array HandleArray(Type arrayType) =>
            arrayType.GetArrayRank() > 1 ? HandleMultiDimensionalArray(arrayType) : HandleSingleDimensionalArray(arrayType);

        private static object HandleClass(Type memberType, int depth, int depthLimit)
        {
            var containingNamespace = memberType.Namespace;
            object classObject;

            if (s_collectionNamespace == containingNamespace &&
                memberType.GenericTypeArguments.Length == 1)
            {
                var elementType = memberType.GenericTypeArguments.First();
                classObject = Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
            }
            else
            {
                classObject = Activator.CreateInstance(memberType, GetArgumentList(memberType).ToArray());
                SetAttributes(classObject, depth + 1, depthLimit);
            }

            return classObject;
        }

        private static Array HandleMultiDimensionalArray(Type arrayType) =>
            Array.CreateInstance(arrayType.GetElementType(), Enumerable.Repeat(0, arrayType.GetArrayRank()).ToArray());

        private static Array HandleSingleDimensionalArray(Type arrayType) =>
            Array.CreateInstance(arrayType.GetElementType(), 0);

        private static void ProcessField(object poco, MemberInfo memberInfo, int depth, int depthLimit)
        {
            var pocoType = poco.GetType();
            var memberName = memberInfo.Name;
            var fieldInfo = pocoType.GetField(memberName);
            var fieldType = fieldInfo.FieldType;

            if (fieldType.IsPrimitive)
            {
                var typeCode = Type.GetTypeCode(fieldType);

                if (typeCode == TypeCode.Boolean)
                {
                    fieldInfo.SetValue(poco, true);
                }
                else
                {
                    fieldInfo.SetValue(poco, memberName.Length % 10);
                }
            }
            else if (fieldType.IsString())
            {
                fieldInfo.SetValue(poco, memberName);
            }
            else if (fieldType.IsArray)
            {
                fieldInfo.SetValue(poco, HandleArray(fieldType));
            }
            else if (fieldType.IsClass && !fieldType.IsAbstract && depth <= depthLimit)
            {
                fieldInfo.SetValue(poco, HandleClass(fieldType, depth, depthLimit));
            }
        }

        private static void ProcessProperty(object poco, MemberInfo memberInfo, int depth, int depthLimit)
        {
            var propertyInfo = poco.GetType().GetProperty(memberInfo.Name);
            var propertyType = propertyInfo.PropertyType;

            if (propertyType.IsPrimitive)
            {
                var typeCode = Type.GetTypeCode(propertyType);

                if (typeCode == TypeCode.Boolean)
                {
                    propertyInfo.SetValue(poco, true);
                }
                else
                {
                    propertyInfo.SetValue(poco, memberInfo.Name.Length % 10);
                }
            }
            else if (propertyType.IsString())
            {
                propertyInfo.SetValue(poco, memberInfo.Name);
            }
            else if (propertyType.IsArray)
            {
                propertyInfo.SetValue(poco, HandleArray(propertyType));
            }
            else if (propertyType.IsClass && !propertyType.IsAbstract && depth <= depthLimit)
            {
                propertyInfo.SetValue(poco, HandleClass(propertyType, depth, depthLimit));
            }
        }

        private static bool IsString(this Type type) => type == typeof(string);
    }
}
