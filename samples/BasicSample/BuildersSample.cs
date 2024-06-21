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

using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace BasicSample
{
    public class BuildersSample
    {
        public void AtlasSearch()
        {
            var mongoClient = new MongoClient(@"mongodb://localhost:27017");
            var db = mongoClient.GetDatabase("testdb");
            var moviesCollection = db.GetCollection<Movie>("movies");

            // Search definition (analyzer provides mql as information message)
            var searchTitle = Builders<Movie>.Search.Wildcard(p => p.Title, "Green D*");

            // MQL is displayed for 'searchTitle' variables
            moviesCollection.Aggregate().Search(searchTitle);
        }

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

            var projection = Builders<Movie>.Projection
                .Include(m => m.Title)
                .Exclude(m => m.Genre);

            // MQL is displayed for 'filter' and 'sort' variables
            var movies = await moviesCollection
                .Find(filter)
                .Sort(sort)
                .Project(projection)
                .As<Movie>()
                .ToListAsync();

            // Fluent API
            _ = moviesCollection
                .Find(u => u.Producer.Contains("Nolan"))
                .SortBy(u => u.Score)
                .ThenBy(u => u.Title);

            return movies;
        }

        public void VariablesTracking()
        {
            var mongoClient = new MongoClient(@"mongodb://localhost:27017");
            var db = mongoClient.GetDatabase("testdb");
            var moviesCollection = db.GetCollection<Movie>("movies");

            var filterScore = Builders<Movie>.Filter.Gt(p => p.Score, 5);
            var filterTitle = Builders<Movie>.Filter.Regex(p => p.Title, "Summer");
            var filterGenre = Builders<Movie>.Filter.Eq(p => p.Genre, Genre.Comedy);

            // Each filter variable tracks its MQL
            var filterCombined = filterTitle | filterScore | filterGenre;

            // MQL for the combined filter
            moviesCollection.Find(filterCombined);

            // MQL for a builder defined in variable
            var movieFilterBuilder = Builders<Movie>.Filter;
            var filterReviews = movieFilterBuilder.Size(p => p.Reviews, 5);
        }

        public void NotSupportedFilter()
        {
            // Not supported filter expression (analyzer provides warning)
            var filter = Builders<Movie>.Filter.Gt(m => m.Reviews.Length, 2);

            // Not supported filter expression (analyzer provides warning)
            filter = Builders<Movie>.Filter.AnyNin(t => t.Reviews, null);
        }
    }
}
