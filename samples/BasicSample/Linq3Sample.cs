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

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace BasicSample
{
    public class Linq3Sample
    {
        public void NotSupportedInLinq2Expressions()
        {
            var mongoClient = new MongoClient(@"mongodb://localhost:27017");
            var db = mongoClient.GetDatabase("testdb");
            var moviesCollection = db.GetCollection<Movie>("movies").AsQueryable();

            // Trim() supported in LINQ3 but not in LINQ2 (analyzer provides warning and LINQ3 mql)
            _ = moviesCollection.Where(m => m.Title.Trim() == "Avatar");

            // Substring() supported in LINQ3 but not in LINQ2 (analyzer provides warning and LINQ3 mql)
            _ = moviesCollection.Where(m => m.Producer.Substring(0, 6) == "Steven");
        }
    }
}
