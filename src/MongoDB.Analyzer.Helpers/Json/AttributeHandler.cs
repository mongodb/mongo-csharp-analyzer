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
    public static class AttributeHandler
    {
        private const string s_collectionNamespace = "System.Collections.Generic";

        public static void SetAttributes(object poco, int depth, int depthLimit)
        {
            var pocoType = poco.GetType();
            var pocoMembers = pocoType.GetMembers();

            foreach (var memberInfo in pocoMembers)
            {
                if (memberInfo.MemberType == MemberTypes.Property)
                {
                    try
                    {
                        ProcessProperty(poco, memberInfo, depth, depthLimit);
                    }
                    catch
                    {
                        continue;
                    }
                }
                else if (memberInfo.MemberType == MemberTypes.Field)
                {
                    try
                    {
                        ProcessField(poco, memberInfo, depth, depthLimit);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
        }

        private static void ProcessProperty(object poco, MemberInfo memberInfo, int depth, int depthLimit)
        {
            var pocoType = poco.GetType();
            var memberName = memberInfo.Name;
            var propertyInfo = pocoType.GetProperty(memberName);

            if (propertyInfo.GetValue(poco) != null)
            {
                return;
            }

            if (propertyInfo.PropertyType == typeof(string))
            {
                propertyInfo.SetValue(poco, memberName);
            }
            else if (propertyInfo.PropertyType.GetTypeInfo().IsArray)
            {
                propertyInfo.SetValue(poco, HandleArray(propertyInfo.PropertyType));
            }
            else if (propertyInfo.PropertyType.GetTypeInfo().IsClass && depth <= depthLimit)
            {
                propertyInfo.SetValue(poco, HandleClass(propertyInfo.PropertyType, depth, depthLimit));
            }
        }

        private static void ProcessField(object poco, MemberInfo memberInfo, int depth, int depthLimit)
        {
            var pocoType = poco.GetType();
            var memberName = memberInfo.Name;
            var fieldInfo = pocoType.GetField(memberName);

            if (fieldInfo.GetValue(poco) != null)
            {
                return;
            }

            if (fieldInfo.FieldType == typeof(string))
            {
                fieldInfo.SetValue(poco, memberName);
            }
            else if (fieldInfo.FieldType.GetTypeInfo().IsArray)
            {
                fieldInfo.SetValue(poco, HandleArray(fieldInfo.FieldType));
            }
            else if (fieldInfo.FieldType.GetTypeInfo().IsClass && depth <= depthLimit)
            {
                fieldInfo.SetValue(poco, HandleClass(fieldInfo.FieldType, depth, depthLimit));
            }
        }

        private static List<object> GetArgumentList(Type type)
        {
            var parameterValues = new List<object>();
            var constructors = type.GetConstructors();

            foreach (var constructor in constructors)
            {
                var parameterList = constructor.GetParameters();
                foreach (var parameter in parameterList)
                {
                    var defaultValue = parameter.DefaultValue;
                    parameterValues.Add(defaultValue);
                }
            }

            return parameterValues;
        }

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

        private static Array HandleMultiDimensionalArray(Type arrayType)
        {
            var arrayRank = arrayType.GetArrayRank();
            var elementType = arrayType.GetElementType();

            int[] dimensionLengths = Enumerable.Repeat(0, arrayRank).ToArray();
            Array array = Array.CreateInstance(elementType, dimensionLengths);
            return array;
        }

        private static Array HandleSingleDimensionalArray(Type arrayType)
        {
            var elementType = arrayType.GetElementType();
            var array = Array.CreateInstance(elementType, 0);
            return array;
        }
    }
}

