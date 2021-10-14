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

namespace MongoDB.Analyzer.Tests.Common
{
    public static class MongoDBProvider
    {
        private static MongoClient s_mongoClient;

        public static IMongoDatabase MongoDatabase
        {
            get
            {
                if (s_mongoClient == null)
                {
                    s_mongoClient = new MongoClient(@"mongodb://localhost:27017");
                }

                return s_mongoClient.GetDatabase("TestDatabase");
            }
        }
    }
}
