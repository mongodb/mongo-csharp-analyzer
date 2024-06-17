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
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Linq
{
    public sealed class LinqQuerySyntax: TestCasesBase
    {
        [MQL("aggregate([{ \"$match\" : { \"Name\" : NameComposer(firstName, lastName) } }])")]
        [MQL("aggregate([{ \"$match\" : { \"Age\" : DoubleAge(ageParameter) } }])")]
        [MQL("aggregate([{ \"$match\" : { \"Height\" : DoubleHeight(25) } }])")]
        public void Method_invocations()
        {
            var firstName = "John";
            var lastName = "Doe";
            var personsQueryable = GetMongoQueryable<Person>();
            _ = from person in personsQueryable
                where person.Name.Equals(NameComposer(firstName, lastName))
                select person;

            var ageParameter = 20;
            _ = from user in GetMongoQueryable<User>()
                where user.Age == DoubleAge(ageParameter)
                select user;

            _ = from user in GetMongoCollection<User>().AsQueryable()
                where user.Height == DoubleHeight(25)
                select user;
        }

        [MQL("aggregate([{ \"$project\" : { \"PersonsList\" : { \"$map\" : { \"input\" : { \"$filter\" : { \"input\" : \"$PesonsList\", \"as\" : \"person\", \"cond\" : { \"$lt\" : [\"$$person.SiblingsCount\", 2] } } }, \"as\" : \"person\", \"in\" : { \"Name\" : \"$$person.Name\", \"Address\" : \"$$person.Address\" } } }, \"_id\" : 0 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"IntIList.9\" : { \"$exists\" : true } } }, { \"$match\" : { \"PesonsList.14\" : { \"$exists\" : true } } }, { \"$project\" : { \"ListsHolder\" : { \"$map\" : { \"input\" : { \"$filter\" : { \"input\" : \"$NestedListsHolderList\", \"as\" : \"nestedListsHolder\", \"cond\" : { \"$and\" : [{ \"$eq\" : [{ \"$size\" : \"$$nestedListsHolder.IntIList\" }, { \"$size\" : \"$$nestedListsHolder.IntIList\" }] }, { \"$eq\" : [{ \"$size\" : \"$$nestedListsHolder.PesonsList\" }, { \"$size\" : \"$$nestedListsHolder.PesonsList\" }] }] } } }, \"as\" : \"nestedListsHolder\", \"in\" : { \"PersonsList\" : \"$$nestedListsHolder.PesonsList\", \"IntList\" : \"$$nestedListsHolder.IntIList\" } } }, \"People\" : { \"$filter\" : { \"input\" : \"$PesonsList\", \"as\" : \"person\", \"cond\" : { \"$and\" : [{ \"$lte\" : [\"$$person.SiblingsCount\", maxSiblings] }, { \"$and\" : [{ \"$eq\" : [\"$$person.Vehicle.VehicleType.Type\", 1] }, { \"$eq\" : [\"$$person.Vehicle.VehicleType.Category\", vehicleCategory] }] }] } } }, \"_id\" : maxSiblings } }])")]
        public void Nested_LINQ_Query()
        {
            var maxSiblings = 22;
            var vehicleCategory = "SUV";

            var listsHolderQueryable = GetMongoQueryable<ListsHolder>();
            _ = from listsHolder in listsHolderQueryable
                        select new
                        {
                            PersonsList = from person in listsHolder.PesonsList
                                          where person.SiblingsCount < 2
                                          select new {Name = person.Name, Address = person.Address}
                        };

            var nestedListsHolderQueryable = GetMongoCollection<ListsHolder>().AsQueryable();
            _ = from listsHolder in nestedListsHolderQueryable
                where listsHolder.IntIList.Count() >= 10
                where listsHolder.PesonsList.Count() >= 15
                select new
                {
                    ListsHolder = from nestedListsHolder in listsHolder.NestedListsHolderList
                                  where nestedListsHolder.IntIList.Count() == listsHolder.IntIList.Count() &&
                                        nestedListsHolder.PesonsList.Count() == listsHolder.PesonsList.Count()
                                  select new
                                  {
                                      PersonsList = nestedListsHolder.PesonsList,
                                      IntList = nestedListsHolder.IntIList
                                  },
                    People = from person in listsHolder.PesonsList
                             where person.SiblingsCount <= maxSiblings
                             where person.Vehicle.VehicleType.Type == VehicleTypeEnum.Car
                             where person.Vehicle.VehicleType.Category == vehicleCategory
                             select person
                };
        }

        [MQL("aggregate([{ \"$match\" : { \"Age\" : user.Age, \"LastName\" : person.Address.City } }])")]
        [MQL("aggregate([{ \"$match\" : { \"Age\" : 22, \"$or\" : [{ \"LastName\" : \"Doe\" }, { \"Name\" : \"John\" }] } }])")]
        [MQL("aggregate([{ \"$match\" : { \"$and\" : [{ \"$or\" : [{ \"Age\" : 22 }, { \"Age\" : 25 }] }, { \"$or\" : [{ \"LastName\" : \"Doe\" }, { \"Name\" : \"John\" }] }] } }, { \"$project\" : { \"Age\" : \"$Age\", \"_id\" : 0 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"Age\" : 22 }, { \"Age\" : 25 }] } }, { \"$match\" : { \"$or\" : [{ \"LastName\" : \"Doe\" }, { \"Name\" : \"John\" }] } }, { \"$project\" : { \"Age\" : \"$Age\", \"Address\" : \"$Address\", \"_id\" : 0 } }])")]
        [MQL("aggregate([{ \"$match\" : { \"$or\" : [{ \"Age\" : 22 }, { \"Age\" : 25 }] } }, { \"$match\" : { \"$or\" : [{ \"LastName\" : \"Doe\" }, { \"Name\" : \"John\" }] } }, { \"$match\" : { \"Address\" : /^Drive/s } }, { \"$project\" : { \"Age\" : \"$Age\", \"Address\" : \"$Address\", \"Name\" : \"$Name\", \"_id\" : 0 } }])")]
        public void Simple_Linq_queries()
        {
            User user = new User();
            user.Age = 25;
            Person person = new Person();

            var userQueryable = GetMongoQueryable();
            _ = from item in userQueryable
                where item.Age == user.Age && item.LastName == person.Address.City
                select item;

            _ = from item in GetMongoQueryable()
                where item.Age == 22 && (item.LastName == "Doe" || item.Name == "John")
                select item;

            _ = from item in GetMongoQueryable()
                where (item.Age == 22 || item.Age == 25) && (item.LastName == "Doe" || item.Name == "John")
                select item.Age;

            _ = from item in GetMongoQueryable()
                where (item.Age == 22 || item.Age == 25)
                where (item.LastName == "Doe" || item.Name == "John")
                select new { item.Age, item.Address };

            _ = from item in GetMongoQueryable()
                where (item.Age == 22 || item.Age == 25)
                where (item.LastName == "Doe" || item.Name == "John")
                where item.Address.StartsWith("Drive")
                select new { item.Age, item.Address, item.Name };
        }

        private int DoubleAge(int age) => 2 * age;
        private int DoubleHeight(int height) => 2 * height;
        private string NameComposer(string firstName, string lastName) => $"{firstName}{lastName}";
    }
}

