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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BasicSample
{
    public class BuildersSample
    {
        public async Task<List<Movie>> GetMovies(double minScore, Genre genre, string titleSearchTerm)
        {
            var mongoClient = new MongoClient(@"mongodb://localhost:27017");
            var db = mongoClient.GetDatabase("testdb");
            var moviesCollection = db.GetCollection<Movie>("movies");

            // Filter definition (analyzer provides mql as information message)
            var filter = Builders<Movie>.Filter.Eq(m => m.Genre, genre) &
                Builders<Movie>.Filter.Gte(m => m.Score, minScore) &
                Builders<Movie>.Filter.Regex(m => m.Score, titleSearchTerm);

            // Sort definition (analyzer provides mql as information message)
            var sort = Builders<Movie>.Sort
                .Ascending(m => m.Score)
                .Descending(m => m.Title);

            var movies = await moviesCollection.Find(filter).Sort(sort).ToListAsync();

            return movies;
        }

        public void NotSupportedFilter()
        {
            // Not supported filter expression (analyzer provides warning)
            var filter = Builders<Movie>.Filter.Gt(m => m.Reviews.Length, 2);
        }
    }
}
