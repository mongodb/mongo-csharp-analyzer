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
using MongoDB.Driver;
using MongoDB.Bson;
using BsonDocumentCustom123 = MongoDB.Bson.BsonDocument;
using BsonValueCustom123 = MongoDB.Bson.BsonValue;
using BsonObjectIdCustom123 = MongoDB.Bson.BsonObjectId;
using BsonTypeCustom123 = MongoDB.Bson.BsonType;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace MongoDB.Analyzer.Helpers.Json
{
    public static class JsonGenerator
    {
#pragma warning disable CS0169 // The field is never used
#pragma warning disable IDE0051
        private static readonly BsonDocumentCustom123 s_dummyRef1;
        private static readonly BsonValueCustom123 s_dummyRef2;
        private static readonly BsonObjectIdCustom123 s_dummyRef3;
        private static readonly BsonTypeCustom123 s_dummyRef4;
#pragma warning restore IDE0051 // The field is never used
#pragma warning restore CS0169

        private static string s_collectionNamespace = "System.Collections.Generic";
        
        public static string GetDriverVersion() => typeof(Builders<>).Assembly.GetName().Version.ToString(3);

        public static string GetJson()
        {
            object poco = new object();
            SetAttributes(poco, new List<string>());
            return poco.ToJson();
        }

        private static void SetAttributes(object poco, List<string> typesAnalyzed)
        {
            var pocoType = poco.GetType();
            var pocoMembers = pocoType.GetMembers();

            if (typesAnalyzed.Contains(pocoType.Name))
            {
                return;
            }

            typesAnalyzed.Add(pocoType.Name);

            foreach (var memberInfo in pocoMembers)
            {
                if (memberInfo.MemberType == MemberTypes.Property)
                {
                    try
                    {
                        ProcessProperty(poco, memberInfo, typesAnalyzed);
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
                        ProcessField(poco, memberInfo, typesAnalyzed);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
        }

        private static void ProcessProperty(object poco, MemberInfo memberInfo, List<string> typesAnalyzed)
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
                propertyInfo.SetValue(poco, HandleArray(propertyInfo.PropertyType, memberInfo, typesAnalyzed));
            }
            else if (propertyInfo.PropertyType.GetTypeInfo().IsClass)
            {
                propertyInfo.SetValue(poco, HandleClass(propertyInfo.PropertyType, memberInfo, typesAnalyzed));
            }
        }

        private static void ProcessField(object poco, MemberInfo memberInfo, List<string> typesAnalyzed)
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
                fieldInfo.SetValue(poco, HandleArray(fieldInfo.FieldType, memberInfo, typesAnalyzed));
            }
            else if (fieldInfo.FieldType.GetTypeInfo().IsClass)
            {
                fieldInfo.SetValue(poco, HandleClass(fieldInfo.FieldType, memberInfo, typesAnalyzed));
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

        private static Array HandleArray(Type arrayType, MemberInfo memberInfo, List<string> typesAnalyzed) =>
            arrayType.GetArrayRank() > 1 ? HandleMultiDimensionalArray(arrayType, memberInfo, typesAnalyzed) :
            HandleSingleDimensionalArray(arrayType, memberInfo, typesAnalyzed);

        private static object HandleClass(Type memberType, MemberInfo memberInfo, List<string> typesAnalyzed)
        {
            var containingNamespace = memberType.Namespace;
            object classObject;

            if (s_collectionNamespace == containingNamespace &&
                memberType.GenericTypeArguments.Length == 1)
            {
                var elementType = memberType.GenericTypeArguments.First();
                classObject = Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
                AddToCollection(classObject, memberInfo, typesAnalyzed);
            }
            else
            {
                classObject = Activator.CreateInstance(memberType, GetArgumentList(memberType).ToArray());
                SetAttributes(classObject, typesAnalyzed);
            }

            return classObject;
        }

        private static Array HandleMultiDimensionalArray(Type arrayType, MemberInfo memberInfo, List<string> typesAnalyzed)
        {
            var arrayLength = 2;
            var arrayRank = arrayType.GetArrayRank();
            var elementType = arrayType.GetElementType();

            int[] dimensionLengths = Enumerable.Repeat(arrayLength, arrayRank).ToArray();
            int[] indices = new int[dimensionLengths.Length];

            Array array = Array.CreateInstance(elementType, dimensionLengths);

            if (elementType == typeof(string))
            {
                var arrayObject = memberInfo.Name;
                PopulateMultiDimensionalArray(arrayObject, dimensionLengths, 0, indices, array);
            }
            else if (elementType.IsClass)
            {
                var arrayObject = Activator.CreateInstance(elementType, GetArgumentList(elementType).ToArray());
                SetAttributes(arrayObject, typesAnalyzed);
                PopulateMultiDimensionalArray(arrayObject, dimensionLengths, 0, indices, array);
            }

            return array;
        }

        private static void PopulateMultiDimensionalArray(object obj, int[] dimensions, int dimensionIndex, int[] indices, Array array)
        {
            if (dimensionIndex == dimensions.Length)
            {
                array.SetValue(obj, indices);
            }
            else
            {
                var currentDimensionSize = dimensions[dimensionIndex];
                for (int i = 0; i < currentDimensionSize; i++)
                {
                    indices[dimensionIndex] = i;
                    PopulateMultiDimensionalArray(obj, dimensions, dimensionIndex + 1, indices, array);
                }
            }
        }

        private static Array HandleSingleDimensionalArray(Type arrayType, MemberInfo memberInfo, List<string> typesAnalyzed)
        {
            var elementType = arrayType.GetElementType();
            var array = Array.CreateInstance(elementType, 2);

            if (!elementType.IsArray)
            {
                if (elementType == typeof(string))
                {
                    array.SetValue(memberInfo.Name, 0);
                    array.SetValue(memberInfo.Name, 1);
                }
                else if (elementType.IsClass)
                {
                    var elementObject = Activator.CreateInstance(elementType, GetArgumentList(elementType).ToArray());
                    SetAttributes(elementObject, typesAnalyzed);
                    array.SetValue(elementObject, 0);
                    array.SetValue(elementObject, 1);
                }

                return array;
            }

            array.SetValue(HandleSingleDimensionalArray(elementType, memberInfo, typesAnalyzed), 0);
            array.SetValue(HandleSingleDimensionalArray(elementType, memberInfo, typesAnalyzed), 1);
            return array;
        }

        private static void AddToCollection(object collectionObject, MemberInfo memberInfo, List<string> typesAnalyzed)
        {
            var listType = collectionObject.GetType();
            var elementType = listType.GenericTypeArguments.First();

            if (elementType == typeof(string))
            {
                listType.GetMethod("Add").Invoke(collectionObject, new[] { memberInfo.Name as object });
            }
            else if (elementType.IsArray)
            {
                listType.GetMethod("Add").Invoke(collectionObject, new[] { HandleArray(elementType, memberInfo, typesAnalyzed) });
            }
            else if (elementType.Namespace == s_collectionNamespace &&
                     elementType.GenericTypeArguments.Length == 1)
            {
                var subElementType = elementType.GenericTypeArguments.First();
                var subCollectionObject = Activator.CreateInstance(typeof(List<>).MakeGenericType(subElementType));
                AddToCollection(subCollectionObject, memberInfo, typesAnalyzed);
                listType.GetMethod("Add").Invoke(collectionObject, new[] { subCollectionObject });
            }
            else if (elementType.IsClass)
            {
                var classObject = Activator.CreateInstance(elementType, GetArgumentList(elementType).ToArray());
                SetAttributes(classObject, typesAnalyzed);
                listType.GetMethod("Add").Invoke(collectionObject, new[] { classObject });
            }
            else if (elementType.IsValueType)
            {
                listType.GetMethod("Add").Invoke(collectionObject, new[] { Activator.CreateInstance(elementType) });
            }
        }
    }
}

