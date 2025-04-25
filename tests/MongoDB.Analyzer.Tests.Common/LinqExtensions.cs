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

using System.Linq;
using MongoDB.Driver.Linq;

namespace MongoDB.Analyzer.Tests.Common
{
    public static class LinqExtensions
    {
        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, int page, int pageSize) =>
            query.Skip((page - 1) * pageSize).Take(pageSize);

        public static IQueryable<T> ApplyPagingIQueryable<T>(this IQueryable<T> query, int page, int pageSize) =>
            query.Skip((page - 1) * pageSize).Take(pageSize);
    }
}
