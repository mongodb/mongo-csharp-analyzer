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

namespace MongoDB.Analyzer.Tests.Common.DataModel
{
    public class SingleTypeGeneric<T>
    {
        public int Data { get; set; }
        public T DataT { get; set; }
    }

    public class DualTypeGeneric<T1, T2>
    {
        public int Data { get; set; }
        public T1 DataT1 { get; set; }
        public T2 DataT2 { get; set; }
    }

    public class MultipleTypeGeneric<T1, T2, T3, T4>
    {
        public int Data { get; set; }
        public T1 DataT1 { get; set; }
        public T2 DataT2 { get; set; }
        public T3 DataT3 { get; set; }
        public T4 DataT4 { get; set; }
    }
}
