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

namespace MongoDB.Analyzer.Helpers.Poco
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
                    ProcessMember(poco, memberInfo, depth, depthLimit);
                }
                catch
                {
                    continue;
                }
            }
        }

        private static object[] GetArgumentList(Type type) =>
            type.GetConstructors().FirstOrDefault()?.GetParameters().Select(parameter => parameter.DefaultValue).ToArray();

        private static object GetMemberValue(MemberInfo memberInfo, Type memberType, int depth, int depthLimit)
        {
            var memberName = memberInfo.Name;
            if (memberType.IsPrimitive)
            {
                return Convert.ChangeType(memberName.Length % 10, memberType);
            }
            else if (memberType.IsString())
            {
                return memberName;
            }
            else if (memberType.IsArray)
            {
                return HandleArray(memberType);
            }
            else if (memberType.IsClass && !memberType.IsAbstract && depth <= depthLimit)
            {
                return HandleClass(memberType, depth, depthLimit);
            }

            return null;
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
                classObject = Activator.CreateInstance(memberType, GetArgumentList(memberType));
                SetAttributes(classObject, depth + 1, depthLimit);
            }

            return classObject;
        }

        private static Array HandleMultiDimensionalArray(Type arrayType) =>
            Array.CreateInstance(arrayType.GetElementType(), Enumerable.Repeat(0, arrayType.GetArrayRank()).ToArray());

        private static Array HandleSingleDimensionalArray(Type arrayType) =>
            Array.CreateInstance(arrayType.GetElementType(), 0);

        private static bool IsString(this Type type) =>
            type == typeof(string);

        private static void ProcessMember(object poco, MemberInfo memberInfo, int depth, int depthLimit)
        {
            if (memberInfo.MemberType == MemberTypes.Property)
            {
                var propertyInfo = poco.GetType().GetProperty(memberInfo.Name);
                var memberValue = GetMemberValue(memberInfo, propertyInfo.PropertyType, depth, depthLimit);
                propertyInfo.SetValue(poco, memberValue);
            }
            else if (memberInfo.MemberType == MemberTypes.Field)
            {
                var fieldInfo = poco.GetType().GetField(memberInfo.Name);
                var memberValue = GetMemberValue(memberInfo, fieldInfo.FieldType, depth, depthLimit);
                fieldInfo.SetValue(poco, memberValue);
            }
        }
    }
}
