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

using MongoDB.Bson;
using System.Collections.Generic;

namespace MongoDB.Analyzer.Tests.Common.DataModel
{
    public abstract class AbstractBase
    {
        public string AbstractBaseData { get; set; }
    }

    public abstract class NestedClass1 : AbstractBase
    {
        public string NestedClass1Data { get; set; }
    }

    public abstract class NestedClass2 : NestedClass1
    {
        public string NestedClass2Data { get; set; }
    }

    public abstract class NestedClass3 : NestedClass2
    {
        public string NestedClass3Data { get; set; }
    }

    public abstract class AbstractGenericBase<T1, T2>
    {
        public string AbstractBaseData { get; set; }

        public T1 AbstractBaseDataT1 { get; set; }
        public T2 AbstractBaseDataT2 { get; set; }
    }

    public abstract class NestedGenericClass1<T1, T2> : AbstractGenericBase<T1, T2>
    {
        public T1 NestedGenericClass1T1 { get; set; }
        public T2 NestedGenericClass1T2 { get; set; }
    }

    public abstract class NestedGenericClass2<T1, T2> : NestedGenericClass1<T1, T2>
    {
        public T1 NestedGenericClass2T1 { get; set; }
        public T2 NestedGenericClass2T2 { get; set; }
    }

    public interface IDummyInterface
    {
    }

    public class ClassWithNonTrivialProperties
    {
        public int IntProp { get; set; }

        public IDummyInterface InterfaceProp { get; set; }
        public IEnumerable<string> IEnumerableProp { get; set; }

        public int this[int index]
        {
            get { return IntProp; }
        }
    }

    public class ClassWithBsonTypes
    {
        public int IntProp { get; set; }

        public BsonDocument BsonDocument { get; set; }
        public BsonValue BsonValue { get; set; }
        public BsonObjectId BsonObjectId { get; set; }
    }
}
