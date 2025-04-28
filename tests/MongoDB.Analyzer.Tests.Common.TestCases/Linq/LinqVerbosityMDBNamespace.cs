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
using MongoDB.Analyzer.Tests.Common.DataModel;
using MongoDB.Driver;

namespace MongoDB.Analyzer.Tests.Common.TestCases.Linq
{
    public sealed class LinqVerbosityMDBNamespace : TestCasesBase
    {
        [NoDiagnostics(linqAnalysisVerbosity: LinqAnalysisVerbosity.All)]
        public void VerbosityAll_no_diagnostics_for_system_collections()
        {
            var personsArray = new Person[] { null, null };
            _ = personsArray.AsQueryable().Where(person => person.IsRetired == true);
            _ = personsArray.AsQueryable().AsQueryable().Where(person => person.IsRetired == true);
            _ = personsArray.AsEnumerable().AsQueryable().Where(person => person.IsRetired == true);

            var personsList = new List<Person> { null, null };
            _ = personsList.AsQueryable().Where(person => person.IsRetired == true);

            var personsHashset = new HashSet<Person> { null, null };
            _ = personsHashset.AsQueryable().Where(person => person.IsRetired == true);
        }

        [MQL("Aggregate([{ \"$match\" : { \"IsRetired\" : true } }])", linqAnalysisVerbosity: LinqAnalysisVerbosity.Medium)]
        [MQL("Aggregate([{ \"$match\" : { \"IsRetired\" : true } }])", linqAnalysisVerbosity: LinqAnalysisVerbosity.Medium)]
        public void VerbosityMedium_diagnostics_is_generated_for_IQueryable()
        {
            IMongoCollection<Person> collection = GetMongoCollection<Person>();

            _ = collection.AsQueryable().Where(person => person.IsRetired == true);

            _ = from person in collection.AsQueryable()
                where person.IsRetired == true
                select person;
        }

        [NoDiagnostics(linqAnalysisVerbosity: LinqAnalysisVerbosity.Medium)]
        public void VerbosityMedium_no_diagnostics_for_system_collections()
        {
            var personsArray = new Person[] { null, null };
            _ = personsArray.AsQueryable().Where(person => person.IsRetired == true);
            _ = personsArray.AsQueryable().AsQueryable().Where(person => person.IsRetired == true);
            _ = personsArray.AsEnumerable().AsQueryable().Where(person => person.IsRetired == true);

            var personsList = new List<Person> { null, null };
            _ = personsList.AsQueryable().Where(person => person.IsRetired == true);

            var personsHashset = new HashSet<Person> { null, null };
            _ = personsHashset.AsQueryable().Where(person => person.IsRetired == true);
        }

        [MQL("Aggregate([{ \"$match\" : { \"IsRetired\" : true } }])", linqAnalysisVerbosity: LinqAnalysisVerbosity.Undefined)]
        [MQL("Aggregate([{ \"$match\" : { \"IsRetired\" : true } }])", linqAnalysisVerbosity: LinqAnalysisVerbosity.Undefined)]
        public void VerbosityUndefined_defaults_to_medium_diagnostics_is_generated_for_IQueryable_with_MBD_namespace()
        {
            IMongoCollection<Person> collection = GetMongoCollection<Person>();

            _ = collection.AsQueryable().Where(person => person.IsRetired == true);

            _ = from person in collection.AsQueryable()
                where person.IsRetired == true
                select person;
        }
    }
}
