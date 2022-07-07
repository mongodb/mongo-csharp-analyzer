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
//

using System.Linq;
using MongoDB.Analyzer.Tests.Common.DataModel;
using MongoDB.Driver.Linq;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Linq
{
    public sealed class LinqQuery: TestCasesBase
    {
        [MQL("aggregate([{ \"$match\" : { \"Age\" : user.Age, \"LastName\" : person.Address.City } }])")]
        public void Query_Syntax_1()
        {
            User user = new User();
            user.Age = 25;
            Person person = new Person();

            var queryable = GetMongoQueryable();
            var query = from item in queryable
                        where item.Age == user.Age && item.LastName == person.Address.City
                        select item;
        }

        [MQL("aggregate([{ \"$match\" : { \"Age\" : 22, \"LastName\" : \"Doe\" } }])")]
        public void Query_Syntax_2()
        {
            var query = from item in GetMongoQueryable()
                        where item.Age == 22 && item.LastName == "Doe"
                        select item;
        }

        [MQL("aggregate([{ \"$match\" : { \"Age\" : 22, \"$or\" : [{ \"LastName\" : \"Doe\" }, { \"Name\" : \"John\" }] } }])")]
        public void Query_Syntax_3()
        {
            var query = from item in GetMongoQueryable()
                        where item.Age == 22 && (item.LastName == "Doe" || item.Name == "John")
                        select item;
        }

        [MQL("aggregate([{ \"$match\" : { \"Age\" : 22, \"$or\" : [{ \"LastName\" : \"Doe\" }, { \"Name\" : \"John\" }] } }, { \"$project\" : { \"Address\" : \"$Address\", \"_id\" : 0 } }])")]
        public void Query_Syntax_4()
        {
            var query = from item in GetMongoQueryable()
                        where item.Age == 22 && (item.LastName == "Doe" || item.Name == "John")
                        select item.Address;
        }

        [MQL("aggregate([{ \"$match\" : { \"$and\" : [{ \"$or\" : [{ \"Age\" : 22 }, { \"Age\" : 25 }] }, { \"$or\" : [{ \"LastName\" : \"Doe\" }, { \"Name\" : \"John\" }] }] } }, { \"$project\" : { \"Age\" : \"$Age\", \"_id\" : 0 } }])")]
        public void Query_Syntax_5()
        {
            var query = from item in GetMongoQueryable()
                        where (item.Age == 22 || item.Age == 25) && (item.LastName == "Doe" || item.Name == "John")
                        select item.Age;
        }

        [MQL("aggregate([{ \"$match\" : { \"$and\" : [{ \"$or\" : [{ \"Age\" : 22 }, { \"Age\" : 25 }] }, { \"$or\" : [{ \"LastName\" : \"Doe\" }, { \"Name\" : \"John\" }] }] } }, { \"$project\" : { \"Age\" : \"$Age\", \"Address\" : \"$Address\", \"_id\" : 0 } }])")]
        public void Query_Syntax_6()
        {
            var query = from item in GetMongoQueryable()
                        where (item.Age == 22 || item.Age == 25) && (item.LastName == "Doe" || item.Name == "John")
                        select new { item.Age, item.Address };
        }

        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"Age\" : 22 }, { \"Age\" : 25 }] } }, { \"$match\" : { \"$or\" : [{ \"LastName\" : \"Doe\" }, { \"Name\" : \"John\" }] } }, { \"$project\" : { \"Age\" : \"$Age\", \"Address\" : \"$Address\", \"_id\" : 0 } }])")]
        public void Query_Syntax_7()
        {
            var query = from item in GetMongoQueryable()
                        where (item.Age == 22 || item.Age == 25)
                        where (item.LastName == "Doe" || item.Name == "John")
                        select new { item.Age, item.Address };
        }

        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"Age\" : 22 }, { \"Age\" : 25 }] } }, { \"$match\" : { \"$or\" : [{ \"LastName\" : \"Doe\" }, { \"Name\" : \"John\" }] } }, { \"$match\" : { \"Address\" : /Drive$/s } }, { \"$project\" : { \"Age\" : \"$Age\", \"Address\" : \"$Address\", \"_id\" : 0 } }])")]
        public void Query_Syntax_8()
        {
            var query = from item in GetMongoQueryable()
                        where (item.Age == 22 || item.Age == 25)
                        where (item.LastName == "Doe" || item.Name == "John")
                        where item.Address.EndsWith("Drive")
                        select new { item.Age, item.Address };
        }

        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"Age\" : 22 }, { \"Age\" : 25 }] } }, { \"$match\" : { \"$or\" : [{ \"LastName\" : \"Doe\" }, { \"Name\" : \"John\" }] } }, { \"$match\" : { \"Address\" : /^Drive/s } }, { \"$project\" : { \"Age\" : \"$Age\", \"Address\" : \"$Address\", \"_id\" : 0 } }])")]
        public void Query_Syntax_9()
        {
            var query = from item in GetMongoQueryable()
                        where (item.Age == 22 || item.Age == 25)
                        where (item.LastName == "Doe" || item.Name == "John")
                        where item.Address.StartsWith("Drive")
                        select new { item.Age, item.Address };
        }

        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"Age\" : 22 }, { \"Age\" : 25 }] } }, { \"$match\" : { \"$or\" : [{ \"LastName\" : \"Doe\" }, { \"Name\" : \"John\" }] } }, { \"$match\" : { \"Address\" : /^Drive/s } }, { \"$project\" : { \"Age\" : \"$Age\", \"Address\" : \"$Address\", \"Name\" : \"$Name\", \"_id\" : 0 } }])")]
        public void Query_Syntax_10()
        {
            var query = from item in GetMongoQueryable()
                        where (item.Age == 22 || item.Age == 25)
                        where (item.LastName == "Doe" || item.Name == "John")
                        where item.Address.StartsWith("Drive")
                        select new { item.Age, item.Address, item.Name };
        }

        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"Age\" : 22 }, { \"Age\" : 25 }] } }, { \"$match\" : { \"$or\" : [{ \"LastName\" : \"Doe\" }, { \"Name\" : \"John\" }] } }, { \"$match\" : { \"Address\" : /^Drive/s } }, { \"$project\" : { \"Age\" : \"$Age\", \"Address\" : \"$Address\", \"Name\" : \"$Name\", \"_id\" : 0 } }])")]
        public void Query_Syntax_11()
        {
            var queryable = GetMongoQueryable();
            var query = from item in queryable
                        where (item.Age == 22 || item.Age == 25)
                        where (item.LastName.Equals("Doe") || item.Name.Equals("John"))
                        where item.Address.StartsWith("Drive")
                        select new { item.Age, item.Address, item.Name };
        }
    }
}

