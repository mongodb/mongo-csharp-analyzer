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
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace BasicSample
{
    public class LinqSample
    {
        public async Task<List<Movie>> GetMovies(double minScore, Genre genre, string titleSearchTerm)
        {
            var mongoClient = new MongoClient(@"mongodb://localhost:27017");
            var db = mongoClient.GetDatabase("testdb");
            var moviesCollection = db.GetCollection<Movie>("movies").AsQueryable();

            // Linq filter query (analyzer provides mql as information message)
            var movies = await moviesCollection
                .Where(m =>
                    m.Genre == genre &&
                    m.Score >= minScore &&
                    m.Title.Contains(titleSearchTerm))
                .OrderBy(m => m.Score)
                .ToListAsync();

            return movies;
        }

        public async Task<List<IGrouping<Genre, Movie>>> GetMoviesGroupedByGenre()
        {
            var mongoClient = new MongoClient(@"mongodb://localhost:27017");
            var db = mongoClient.GetDatabase("testdb");
            var moviesCollection = db.GetCollection<Movie>("movies").AsQueryable();

            // Linq group by (analyzer provides mql as information message)
            var groups = await moviesCollection
                .GroupBy(m => m.Genre)
                .ToListAsync();

            return groups;
        }

        public async Task<dynamic> GetGenreStatistics()
        {
            var mongoClient = new MongoClient(@"mongodb://localhost:27017");
            var db = mongoClient.GetDatabase("testdb");
            var moviesCollection = db.GetCollection<Movie>("movies").AsQueryable();

            // Linq group by and select (analyzer provides mql as information message)
            var groups = await moviesCollection
                .GroupBy(m => m.Genre)
                .Select(g => new { Genre = g.Key, MoviesCount = g.Count(), AverageScore = g.Average(m => m.Score) })
                .ToListAsync();

            return groups;
        }

        public async Task<IGrouping<Genre, Movie>> GetActionMoviesReviewsByProducer(string producerName)
        {
            var mongoClient = new MongoClient(@"mongodb://localhost:27017");
            var db = mongoClient.GetDatabase("testdb");
            var moviesCollection = db.GetCollection<Movie>("movies").AsQueryable();

            // Linq filter and select query (analyzer provides mql as information message)
            var reviews = await moviesCollection
                .Where(m => m.Producer == producerName)
                .Where(m => m.Genre == Genre.Action)
                .SelectMany(m => m.Reviews)
                .ToListAsync();

            return null;
        }

        public void NotSupportedQuery(double minScore, Genre genre)
        {
            var mongoClient = new MongoClient(@"mongodb://localhost:27017");
            var db = mongoClient.GetDatabase("testdb");
            var moviesCollection = db.GetCollection<Movie>("movies").AsQueryable();

            // GetHashCode is not supported (analyzer provides warning)
            _ = moviesCollection.Where(m => m.GetHashCode() == 1234);

            // External method referencing linq lambda parameter is not supported (analyzer provides warning)
            _ = moviesCollection
                .Where(m => CalculateMoviesReviewsNumber(m) == 512)
                .Where(m => GetProducerAndTitle(m) == "Christopher Nolan: Dunkirk");
        }

        private int CalculateMoviesReviewsNumber(Movie movie) => movie.Reviews.Length;
        private string GetProducerAndTitle(Movie movie) => $"{movie.Producer}-{movie.Title}";
    }
}
