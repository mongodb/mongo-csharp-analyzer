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
// Do not include MongoDB.Driver namespace here, as it is part of the test case.

namespace MongoDB.Analyzer.Tests.Common.TestCases.Linq
{
    public sealed class LinqVerbosity : TestCasesBase
    {
        [MQL("Aggregate([{ \"$match\" : { \"IsRetired\" : true } }])", linqAnalysisVerbosity: LinqAnalysisVerbosity.All)]
        [MQL("Aggregate([{ \"$match\" : { \"IsRetired\" : true } }])", linqAnalysisVerbosity: LinqAnalysisVerbosity.All)]
        public void VerbosityAll_diagnostics_is_generated_for_IQueryable()
        {
            _ = GetMongoQueryable<Person>().Where(person => person.IsRetired == true);

            _ = from person in GetMongoQueryable<Person>()
                where person.IsRetired == true
                select person;
        }

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

        [NoDiagnostics(linqAnalysisVerbosity: LinqAnalysisVerbosity.Medium)]
        public void VerbosityMedium_no_diagnostics_for_IQueryable()
        {
            _ = GetMongoQueryable<Person>().Where(person => person.IsRetired == true);
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

        [NoDiagnostics(linqAnalysisVerbosity: LinqAnalysisVerbosity.None)]
        public void VerbosityNone_no_diagnostics()
        {
            _ = GetMongoQueryable<Person>().Where(person => person.IsRetired == true);

            _ = from person in GetMongoQueryable<Person>()
                where person.IsRetired == true
                select person;
        }

        [NoDiagnostics(linqAnalysisVerbosity: LinqAnalysisVerbosity.Undefined)]
        public void VerbosityUndefined_defaults_to_medium_no_diagnostics_for_IQueryable_without_MBD_namespace()
        {
            _ = GetMongoQueryable<Person>().Where(person => person.IsRetired == true);
        }
    }
}
